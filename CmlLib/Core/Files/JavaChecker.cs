using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CmlLib.Core.Downloader;
using CmlLib.Core.Installer;
using CmlLib.Core.Version;
using CmlLib.Utils;
using Newtonsoft.Json.Linq;

namespace CmlLib.Core.Files
{
    public class JavaChecker : IFileChecker
    {
        public string JavaManifestServer { get; set; } = MojangServer.JavaManifest;
        public string JavaBinaryName { get; set; } = MJava.GetDefaultBinaryName();
        public bool CheckHash { get; set; } = true;
        
        public DownloadFile[]? CheckFiles(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs>? downloadProgress)
        {
            if (!string.IsNullOrEmpty(version.JavaBinaryPath) && File.Exists(version.JavaBinaryPath))
                return null;
            
            var javaVersion = version.JavaVersion;
            if (string.IsNullOrEmpty(javaVersion))
                javaVersion = "jre-legacy";

            var files = internalCheckFile(
                javaVersion, path, downloadProgress, out string binPath);

            version.JavaBinaryPath = binPath;
            return files;
        }

        public Task<DownloadFile[]?> CheckFilesTaskAsync(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs>? downloadProgress)
        {
            return Task.Run(() => CheckFiles(path, version, downloadProgress));
        }

        private DownloadFile[]? internalCheckFile(string javaVersion, MinecraftPath path,
            IProgress<DownloadFileChangedEventArgs>? downloadProgress, out string binPath)
        {
            binPath = Path.Combine(path.Runtime, javaVersion, "bin", JavaBinaryName);

            try
            {
                var osName = getJavaOSName(); // safe
                var javaVersions = getJavaVersionsForOs(osName); // Net, JsonParse Exception
                if (javaVersions != null)
                {
                    var javaManifest = getJavaVersionManifest(javaVersions, javaVersion); // Net, JsonParse

                    if (javaManifest == null)
                        javaManifest = getJavaVersionManifest(javaVersions, "jre-legacy");
                    if (javaManifest == null)
                        return legacyJavaChecker(path, out binPath);

                    var files = javaManifest["files"] as JObject;
                    if (files == null)
                        return legacyJavaChecker(path, out binPath);

                    return toDownloadFiles(javaVersion, files, path, downloadProgress);
                }
                else
                    return legacyJavaChecker(path, out binPath);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return legacyJavaChecker(path, out binPath);
            }
        }

        private string getJavaOSName()
        {
            string osName = "";
            
            if (MRule.OSName == MRule.Windows)
            {
                if (MRule.Arch == "64")
                    osName = "windows-x64";
                else
                    osName = "windows-x86";
            }
            else if (MRule.OSName == MRule.Linux)
            {
                if (MRule.Arch == "64")
                    osName = "linux";
                else
                    osName = "linux-i386";
            }
            else if (MRule.OSName == MRule.OSX)
            {
                osName = "mac-os";
            }

            return osName;
        }
        
        private JObject? getJavaVersionsForOs(string osName)
        {
            string response;
            
            using (var wc = new WebClient())
            {
                response = wc.DownloadString(JavaManifestServer); // ex
            }

            var job = JObject.Parse(response); // ex
            return job[osName] as JObject;
        }

        private JObject? getJavaVersionManifest(JObject job, string version)
        {
            var versionArr = job[version] as JArray;
            if (versionArr == null || versionArr.Count == 0)
                return null;
            
            var manifestUrl = versionArr[0]["manifest"]?["url"]?.ToString();
            if (string.IsNullOrEmpty(manifestUrl))
                return null;

            string response;
            using (var wc = new WebClient())
            {
                response = wc.DownloadString(manifestUrl); // ex
            }

            return JObject.Parse(response); // ex
        }

        private DownloadFile[] toDownloadFiles(string javaVersionName, JObject manifest, MinecraftPath path,
            IProgress<DownloadFileChangedEventArgs>? progress)
        {
            var progressed = 0;
            var files = new List<DownloadFile>(manifest.Count);
            foreach (var prop in manifest)
            {
                var name = prop.Key;
                var value = prop.Value;

                var type = value?["type"]?.ToString();
                if (type == "file")
                {
                    var filePath = Path.Combine(path.Runtime, javaVersionName, name);
                    filePath = IOUtil.NormalizePath(filePath);
                    
                    bool.TryParse(value?["executable"]?.ToString(), out bool executable);
                    
                    var file = checkJavaFile(value, filePath);
                    if (file != null)
                    {
                        file.Name = name;
                        if (executable)
                            file.AfterDownload = new Func<Task>[]
                            {
                                () => Task.Run(() => tryChmod755(filePath))
                            };
                        files.Add(file);
                    }
                }
                else
                {
                    if (type != "directory")
                        Debug.WriteLine(type);
                }
                
                progressed++;
                progress?.Report(new DownloadFileChangedEventArgs(
                    MFile.Runtime, false, name, manifest.Count, progressed));
            }
            return files.ToArray();
        }

        private DownloadFile? checkJavaFile(JToken? value, string filePath)
        {
            var downloadObj = value?["downloads"]?["raw"];
            if (downloadObj == null)
                return null;
                    
            var url = downloadObj["url"]?.ToString();
            if (string.IsNullOrEmpty(url))
                return null;
                    
            var hash = downloadObj["sha1"]?.ToString();
            var sizeStr = downloadObj["size"]?.ToString();
            long.TryParse(sizeStr, out long size);

            if (IOUtil.CheckFileValidation(filePath, hash, CheckHash))
                return null;
            
            return new DownloadFile(filePath, url)
            {
                Size = size
            };
        }

        private DownloadFile[] legacyJavaChecker(MinecraftPath path, out string binPath)
        {
            string legacyJavaPath = Path.Combine(path.Runtime, "m-legacy");
            MJava mJava = new MJava(legacyJavaPath);
            binPath = mJava.GetBinaryPath();
            
            if (mJava.CheckJavaExistence())
                return new DownloadFile[] {};

            string javaUrl = mJava.GetJavaUrl(); 
            string lzmaPath = Path.Combine(Path.GetTempPath(), "jre.lzma");
            string zipPath = Path.Combine(Path.GetTempPath(), "jre.zip");
                            
            DownloadFile file = new DownloadFile(lzmaPath, javaUrl)
            {
                Name = "jre.lzma",
                Type = MFile.Runtime,
                AfterDownload = new Func<Task>[]
                {
                    () => Task.Run(() =>
                    {
                        SevenZipWrapper.DecompressFileLZMA(lzmaPath, zipPath);

                        var z = new SharpZip(zipPath);
                        z.Unzip(legacyJavaPath);

                        tryChmod755(mJava.GetBinaryPath());
                    })
                }
            };

            return new[] {file};
        }

        private void tryChmod755(string path)
        {
            try
            {
                if (MRule.OSName != MRule.Windows)
                    IOUtil.Chmod(path, IOUtil.Chmod755);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}