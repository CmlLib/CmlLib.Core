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
        public bool CheckHash { get; set; }
        
        public DownloadFile[]? CheckFiles(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs>? downloadProgress)
        {
            var javaVersion = version.JavaVersion;
            if (string.IsNullOrEmpty(javaVersion))
                javaVersion = "jre-legacy";

            version.JavaBinaryPath = Path.Combine(path.Runtime, javaVersion, "bin", getDefaultBinaryName());

            var osName = getJavaOSName();
            var javaVersions = getJavaVersionsForOS(osName);
            if (javaVersions != null)
            {
                var javaManifest = getJavaVersionManifest(javaVersions, javaVersion);

                if (javaManifest == null)
                    javaManifest = getJavaVersionManifest(javaVersions, "jre-legacy");
                if (javaManifest == null)
                    return legacyJavaChecker(path);

                var files = javaManifest?["files"] as JObject;
                if (files == null)
                    return legacyJavaChecker(path);

                return toDownloadFiles(javaVersion, files, path, downloadProgress);
            }
            else
                return legacyJavaChecker(path);
        }

        public Task<DownloadFile[]?> CheckFilesTaskAsync(MinecraftPath path, MVersion version, 
            IProgress<DownloadFileChangedEventArgs>? downloadProgress)
        {
            return Task.Run(() => CheckFiles(path, version, downloadProgress));
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
        
        private string getDefaultBinaryName()
        {
            string binaryName = "java";
            if (MRule.OSName == MRule.Windows)
                binaryName = "javaw.exe";
            return binaryName;
        }
        
        private JObject? getJavaVersionsForOS(string osName)
        {
            var url =
                "https://launchermeta.mojang.com/v1/products/java-runtime/2ec0cc96c44e5a76b9c8b7c39df7210883d12871/all.json";
            string response;
            
            using (var wc = new WebClient())
            {
                response = wc.DownloadString(url);
            }

            var job = JObject.Parse(response);
            return job[osName] as JObject;
        }

        private JObject? getJavaVersionManifest(JObject job, string version)
        {
            var versionArr = job[version] as JArray;
            if (versionArr == null || versionArr.Count == 0)
                return null;
            
            var manifestUrl = versionArr[0]?["manifest"]?["url"]?.ToString();
            if (string.IsNullOrEmpty(manifestUrl))
                return null;

            string response;
            using (var wc = new WebClient())
            {
                response = wc.DownloadString(manifestUrl);
            }

            return JObject.Parse(response);
        }

        private DownloadFile[]? toDownloadFiles(string javaVersionName, JObject manifest, MinecraftPath path,
            IProgress<DownloadFileChangedEventArgs>? progress)
        {
            var progressed = 0;
            var files = new List<DownloadFile>(manifest.Count);
            string? exeFileName = null;
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

        private DownloadFile[]? legacyJavaChecker(MinecraftPath path)
        {
            string legacyJavaPath = Path.Combine(path.Runtime, "m-legacy");
            MJava mJava = new MJava(legacyJavaPath);
            
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
                    })
                }
            };

            return new[] {file};
        }
    }
}