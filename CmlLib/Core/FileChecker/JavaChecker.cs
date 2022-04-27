using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CmlLib.Core.Downloader;
using CmlLib.Core.Installer;
using CmlLib.Core.Java;
using CmlLib.Core.Version;
using CmlLib.Utils;

namespace CmlLib.Core.FileChecker
{
    public class JavaChecker : IFileChecker
    {
        class JavaCheckResult
        {
            public string? JavaBinaryPath { get; set; }
            public DownloadFile[]? JavaFiles { get; set; }
        }

        public IJavaPathResolver? JavaPathResolver { get; set; }
        public string JavaManifestServer { get; set; } = MojangServer.JavaManifest;
        public bool CheckHash { get; set; } = true;

        public JavaChecker()
        {

        }

        public JavaChecker(IJavaPathResolver javaPathResolver)
        {
            JavaPathResolver = javaPathResolver;
        }

        public DownloadFile[]? CheckFiles(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs>? downloadProgress)
        {
            return CheckFilesTaskAsync(path, version, downloadProgress)
                .ConfigureAwait(false)
                .GetAwaiter().GetResult();
        }

        public async Task<DownloadFile[]?> CheckFilesTaskAsync(MinecraftPath path, MVersion version,
            IProgress<DownloadFileChangedEventArgs>? downloadProgress)
        {
            if (!string.IsNullOrEmpty(version.JavaBinaryPath) && File.Exists(version.JavaBinaryPath))
                return null;
            
            var javaVersion = version.JavaVersion;
            if (string.IsNullOrEmpty(javaVersion))
                javaVersion = MinecraftJavaPathResolver.JreLegacyVersionName;

            var result = await internalCheckJava(javaVersion, path, downloadProgress)
                .ConfigureAwait(false);

            version.JavaBinaryPath = result.JavaBinaryPath;
            return result.JavaFiles;
        }

        private async Task<JavaCheckResult> internalCheckJava(string javaVersion, MinecraftPath path,
            IProgress<DownloadFileChangedEventArgs>? downloadProgress)
        {
            if (JavaPathResolver == null)
                throw new InvalidOperationException("JavaPathResolver was null");

            var binPath = JavaPathResolver.GetJavaBinaryPath(javaVersion, MRule.OSName);
            DownloadFile[]? downloadFiles;

            try
            {
                var osName = getJavaOSName(); // safe
                
                var response = await HttpUtil.HttpClient.GetAsync(JavaManifestServer); // get all java versions
                var str = await response.Content.ReadAsStringAsync();
                using var jsonDocument = JsonDocument.Parse(str);
                
                var root = jsonDocument.RootElement;
                var javaVersions = root.SafeGetProperty(osName); // get os specific java version
                
                // try three versions: latest, JreLegacyVersionName(jre-legacy), legacy java (MJava)
                if (javaVersions != null)
                {
                    var javaManifest = await getJavaVersionManifest(javaVersions.Value, javaVersion);

                    if (javaManifest == null && javaVersion != MinecraftJavaPathResolver.JreLegacyVersionName)
                        javaManifest = await getJavaVersionManifest(javaVersions.Value, MinecraftJavaPathResolver.JreLegacyVersionName);
                    if (javaManifest == null)
                        return await legacyJavaChecker(path);

                    // get file objects
                    using var manifestDocument = JsonDocument.Parse(javaManifest);
                    var files = manifestDocument.RootElement.SafeGetProperty("files");
                    if (files == null)
                        return await legacyJavaChecker(path);

                    downloadFiles = toDownloadFiles(files.Value, JavaPathResolver.GetJavaDirPath(javaVersion), downloadProgress);
                }
                else // no java version for osName
                    return await legacyJavaChecker(path);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);

                if (string.IsNullOrEmpty(binPath))
                    return await legacyJavaChecker(path);
                else
                    downloadFiles = new DownloadFile[] { };
            }

            return new JavaCheckResult()
            {
                JavaFiles = downloadFiles,
                JavaBinaryPath = binPath
            };
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
        
        // get java files of the specific version
        private async Task<string?> getJavaVersionManifest(JsonElement job, string version)
        {
            var versionArr = job.SafeGetProperty(version)?.EnumerateArray();
            if (versionArr == null)
                return null;

            var firstManifest = versionArr.Value.FirstOrDefault();
            var manifestUrl = firstManifest.SafeGetProperty("manifest")?.SafeGetProperty("url")?.GetString();
            if (string.IsNullOrEmpty(manifestUrl))
                return null;

            return await HttpUtil.HttpClient.GetStringAsync(manifestUrl);
        }

        // compare local files with `manifest`
        private DownloadFile[] toDownloadFiles(JsonElement manifest, string path,
            IProgress<DownloadFileChangedEventArgs>? progress)
        {
            var objects = manifest.EnumerateObject();
            var total = objects.Count();
            objects.Reset();

            var progressed = 0;
            var files = new List<DownloadFile>(total);
            foreach (var prop in objects)
            {
                var name = prop.Name;
                var value = prop.Value;

                var type = value.GetPropertyValue("type");
                if (type == "file")
                {
                    var filePath = Path.Combine(path, name);
                    filePath = IOUtil.NormalizePath(filePath);

                    var executable = value.SafeGetProperty("executable")?.GetBoolean() ?? false;
                    
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
                    MFile.Runtime, this, name, total: total, progressed: progressed));
            }
            return files.ToArray();
        }

        private DownloadFile? checkJavaFile(JsonElement value, string filePath)
        {
            var downloadObj = value.SafeGetProperty("downloads")?.SafeGetProperty("raw");
            if (downloadObj == null)
                return null;

            var url = downloadObj.Value.GetPropertyValue("url");
            if (string.IsNullOrEmpty(url))
                return null;

            var hash = downloadObj.Value.GetPropertyValue("sha1");
            var size = downloadObj.Value.SafeGetProperty("size")?.GetInt64() ?? 0;

            if (IOUtil.CheckFileValidation(filePath, hash, CheckHash))
                return null;
            
            return new DownloadFile(filePath, url)
            {
                Size = size
            };
        }

        // legacy java checker that use MJava
        private async Task<JavaCheckResult> legacyJavaChecker(MinecraftPath path)
        {
            if (JavaPathResolver == null)
                throw new InvalidOperationException("JavaPathResolver was null");

            string legacyJavaPath = JavaPathResolver.GetJavaDirPath(MinecraftJavaPathResolver.CmlLegacyVersionName);
            
            MJava mJava = new MJava(legacyJavaPath);
            var result = new JavaCheckResult()
            {
                JavaBinaryPath = mJava.GetBinaryPath(),
                JavaFiles = null
            };
            
            try
            {
                if (mJava.CheckJavaExistence())
                    return result;

                string javaUrl = await mJava.GetJavaUrlAsync(); 
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
                result.JavaFiles = new[] { file };

                return result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return result;
            }
        }

        private void tryChmod755(string path)
        {
            try
            {
                if (MRule.OSName != MRule.Windows)
                    NativeMethods.Chmod(path, NativeMethods.Chmod755);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
    }
}