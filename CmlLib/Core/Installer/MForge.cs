using CmlLib.Core.Downloader;
using CmlLib.Core.Files;
using CmlLib.Utils;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace CmlLib.Core.Installer
{
    public class MForge
    {
        private const string MavenServer = "https://maven.minecraftforge.net/net/minecraftforge/forge/";

        public static string GetOldForgeName(string mcVersion, string forgeVersion)
        {
            return $"{mcVersion}-forge{mcVersion}-{forgeVersion}";
        }

        public static string GetForgeName(string mcVersion, string forgeVersion)
        {
            return $"{mcVersion}-forge-{forgeVersion}";
        }

        public MForge(MinecraftPath mc, string java)
        {
            this.minecraftPath = mc;
            JavaPath = java;
            downloader = new SequenceDownloader();
        }

        public string JavaPath { get; private set; }
        private readonly MinecraftPath minecraftPath;
        private readonly IDownloader downloader;
        public event DownloadFileChangedHandler? FileChanged;
        public event EventHandler<string>? InstallerOutput;

        public string InstallForge(string mcVersion, string forgeVersion)
        {
            var minecraftJar = minecraftPath.GetVersionJarPath(mcVersion);
            if (!File.Exists(minecraftJar))
                throw new IOException($"Install {mcVersion} first");

            var installerPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            var installerStream = getInstallerStream(mcVersion, forgeVersion); // download installer
            if (installerStream == null)
                throw new InvalidOperationException("cannot open installer stream");
            var extractedFile = extractInstaller(installerStream, installerPath); // extract installer

            var profileObj = extractedFile.Item1; // version profile json
            var installerObj = extractedFile.Item2; // installer info

            // copy forge libraries to minecraft
            extractMaven(installerPath); // new installer

            var universalPath = installerObj["filePath"]?.ToString();
            if (string.IsNullOrEmpty(universalPath))
                throw new InvalidOperationException("filePath property in installer was null");
            
            var destPath = installerObj["path"]?.ToString();
            if (string.IsNullOrEmpty(destPath))
                throw new InvalidOperationException("path property in installer was null");
            
            extractUniversal(installerPath, universalPath, destPath); // old installer

            // download libraries and processors
            checkLibraries(installerObj["libraries"] as JArray);

            // mapping client data
            var installerData = installerObj["data"] as JObject;
            var mapData = (installerData == null) 
                ? new Dictionary<string, string?>()
                : mapping(installerData, "client", minecraftJar, installerPath);

            // process
            process(installerObj["processors"] as JArray, mapData);

            // version name like 1.16.2-forge-33.0.20
            var versionName = installerObj["target"]?.ToString()
                           ?? installerObj["version"]?.ToString()
                           ?? GetForgeName(mcVersion, forgeVersion);

            var versionPath = minecraftPath.GetVersionJsonPath(versionName);

            // write version profile json
            writeProfile(profileObj, versionPath);

            return versionName;
        }

        private Stream? getInstallerStream(string mcVersion, string forgeVersion)
        {
            fireEvent(MFile.Library, "installer", 1, 0);

            var url = $"{MavenServer}{mcVersion}-{forgeVersion}/" +
                $"forge-{mcVersion}-{forgeVersion}-installer.jar";

            return WebRequest.Create(url).GetResponse().GetResponseStream();
        }

        private Tuple<JToken, JToken> extractInstaller(Stream stream, string extractPath)
        {
            // extract installer
            string? installProfile = null;
            string? versionsJson = null;

            using (stream)
            using (var s = new ZipInputStream(stream))
            {
                ZipEntry e;
                while ((e = s.GetNextEntry()) != null)
                {
                    if (e.Name.Length <= 0)
                        continue;

                    var realpath = Path.Combine(extractPath, e.Name);

                    if (e.IsFile)
                    {
                        if (e.Name == "install_profile.json")
                            installProfile = readStreamString(s);
                        else if (e.Name == "version.json")
                            versionsJson = readStreamString(s);
                        else
                        {
                            var dirPath = Path.GetDirectoryName(realpath);
                            if (!string.IsNullOrEmpty(dirPath))
                                Directory.CreateDirectory(dirPath);

                            using var fs = File.OpenWrite(realpath);
                            s.CopyTo(fs);
                        }
                    }
                }
            }

            if (installProfile == null)
                throw new InvalidOperationException("no install_profile.json in installer");
            if (versionsJson == null)
                throw new InvalidOperationException("no version.json in installer");
            
            JToken profileObj;
            var installObj = JObject.Parse(installProfile); // installer info
            var versionInfo = installObj["versionInfo"]; // version profile

            if (versionInfo == null)
                profileObj = JObject.Parse(versionsJson);
            else
            {
                installObj = installObj["install"] as JObject;
                profileObj = versionInfo;
            }

            if (installObj == null)
                throw new InvalidOperationException("no 'install' object in install_profile.json");

            return new Tuple<JToken, JToken>(profileObj, installObj);
        }

        private string readStreamString(Stream s)
        {
            var str = new StringBuilder();
            var buffer = new byte[1024];
            while (true)
            {
                int size = s.Read(buffer, 0, buffer.Length);
                if (size == 0)
                    break;

                str.Append(Encoding.UTF8.GetString(buffer, 0, size));
            }

            return str.ToString();
        }

        // for new installer
        private void extractMaven(string installerPath)
        {
            fireEvent(MFile.Library, "maven", 1, 0);

            // copy all libraries in maven (include universal) to minecraft
            var org = Path.Combine(installerPath, "maven");
            if (Directory.Exists(org))
                IOUtil.CopyDirectory(org, minecraftPath.Library, true);
        }

        // for old installer
        private void extractUniversal(string installerPath, string universalPath, string destinyName)
        {
            fireEvent(MFile.Library, "universal", 1, 0);

            if (string.IsNullOrEmpty(universalPath) || string.IsNullOrEmpty(destinyName))
                return;

            // copy universal library to minecraft
            var universal = Path.Combine(installerPath, universalPath);
            var desPath = PackageName.Parse(destinyName).GetPath();
            var des = Path.Combine(minecraftPath.Library, desPath);

            if (File.Exists(universal))
            {
                var dirPath = Path.GetDirectoryName(des);
                if (!string.IsNullOrEmpty(dirPath))
                    Directory.CreateDirectory(dirPath);
                File.Copy(universal, des, true);
            }
        }

        // legacy
        private void downloadUniversal(string mcVersion, string forgeVersion)
        {
            fireEvent(MFile.Library, "universal", 1, 0);

            var forgeName = $"forge-{mcVersion}-{forgeVersion}";
            var baseUrl = $"{MavenServer}{mcVersion}-{forgeVersion}";

            var universalUrl = $"{baseUrl}/{forgeName}-universal.jar";
            var universalPath = Path.Combine(
                minecraftPath.Library,
                "net",
                "minecraftforge",
                "forge",
                $"{mcVersion}-{forgeVersion}",
                $"forge-{mcVersion}-{forgeVersion}.jar"
            );

            var dirPath = Path.GetDirectoryName(universalPath);
            if (!string.IsNullOrEmpty(dirPath))
                Directory.CreateDirectory(dirPath);
            
            var dl = new WebDownload();
            dl.DownloadFile(universalUrl, universalPath);
        }

        private Dictionary<string, string?> mapping(JObject data, string kind,
            string minecraftJar, string installerPath)
        {
            // convert [path] to absolute path

            var dataMapping = new Dictionary<string, string?>();
            foreach (var item in data)
            {
                var key = item.Key;
                var value = item.Value?[kind]?.ToString();
                
                if (string.IsNullOrEmpty(value))
                    continue;

                var fullPath = Mapper.ToFullPath(value, minecraftPath.Library);
                if (fullPath == value)
                {
                    value = value.Trim('/');
                    dataMapping.Add(key, Path.Combine(installerPath, value));
                }
                else
                    dataMapping.Add(key, fullPath);
            }

            dataMapping.Add("SIDE", "CLIENT");
            dataMapping.Add("MINECRAFT_JAR", minecraftJar);

            return dataMapping;
        }

        private void checkLibraries(JArray? jarr)
        {
            if (jarr == null || jarr.Count == 0)
                return;

            var libs = new List<MLibrary>();
            var parser = new MLibraryParser();
            foreach (var item in jarr)
            {
                var parsedLib = parser.ParseJsonObject((JObject)item);
                if (parsedLib != null)
                    libs.AddRange(parsedLib);
            }

            var fileProgress = new Progress<DownloadFileChangedEventArgs>(
                e => FileChanged?.Invoke(e));
            
            var libraryChecker = new LibraryChecker();
            var lostLibrary = libraryChecker.CheckFiles(minecraftPath, libs.ToArray(), fileProgress);
            
            if (lostLibrary != null)
                downloader.DownloadFiles(lostLibrary, fileProgress, null);
        }

        private void process(JArray? processors, Dictionary<string, string?> mapData)
        {
            if (processors == null || processors.Count == 0)
                return;

            fireEvent(MFile.Library, "processors", processors.Count, 0);

            for (int i = 0; i < processors.Count; i++)
            {
                var item = processors[i];

                var outputs = item["outputs"] as JObject;
                if (outputs == null || !checkProcessorOutputs(outputs, mapData))
                    startProcessor(item, mapData);

                fireEvent(MFile.Library, "processors", processors.Count, i + 1);
            }
        }

        private bool checkProcessorOutputs(JObject outputs, Dictionary<string, string?> mapData)
        {
            foreach (var item in outputs)
            {
                if (item.Value == null)
                    continue;
                
                var key = Mapper.Interpolation(item.Key, mapData, true);
                var value = Mapper.Interpolation(item.Value.ToString(), mapData, true);

                if (!File.Exists(key) || !IOUtil.CheckSHA1(key, value))
                    return false;
            }

            return true;
        }

        private void startProcessor(JToken processor, Dictionary<string, string?> mapData)
        {
            var name = processor["jar"]?.ToString();
            if (name == null)
                return;

            // jar
            var jar = PackageName.Parse(name);
            var jarPath = Path.Combine(minecraftPath.Library, jar.GetPath());

            var jarFile = new JarFile(jarPath);
            var jarManifest = jarFile.GetManifest();

            // mainclass
            string? mainClass = null;
            var hasMainclass = jarManifest?.TryGetValue("Main-Class", out mainClass) ?? false;
            if (!hasMainclass || string.IsNullOrEmpty(mainClass))
                return;

            // classpath
            var classpathObj = processor["classpath"];
            var classpath = new List<string>();
            if (classpathObj != null)
            {
                foreach (var libName in classpathObj)
                {
                    var libNameString = libName?.ToString();
                    if (string.IsNullOrEmpty(libNameString))
                        continue;
                    
                    var lib = Path.Combine(minecraftPath.Library,
                        PackageName.Parse(libNameString).GetPath());
                    classpath.Add(lib);
                }
            }
            classpath.Add(jarPath);

            // arg
            var argsArr = processor["args"] as JArray;
            string[]? args = null;
            if (argsArr != null)
            {
                var arrStrs = argsArr.Select(x => x.ToString()).ToArray();
                args = Mapper.Map(arrStrs, mapData, minecraftPath.Library);
            }

            startJava(classpath.ToArray(), mainClass, args);
        }

        private void startJava(string[] classpath, string mainClass, string[]? args)
        {
            var arg =
                $"-cp {IOUtil.CombinePath(classpath)} " +
                $"{mainClass}";

            if (args != null && args.Length > 0)
                arg += " " + string.Join(" ", args);
            
            var process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = JavaPath,
                Arguments = arg,
            };

            var p = new ProcessUtil(process);
            p.OutputReceived += (s, e) => InstallerOutput?.Invoke(this, e);
            p.StartWithEvents();
            p.Process.WaitForExit();
        }

        private void writeProfile(JToken profileObj, string versionPath)
        {
            var dirPath = Path.GetDirectoryName(versionPath);
            if (!string.IsNullOrEmpty(dirPath))
                Directory.CreateDirectory(dirPath);
            File.WriteAllText(versionPath, profileObj.ToString());
        }

        private void fireEvent(MFile kind, string name, int total, int progressed)
        {
            FileChanged?.Invoke(new DownloadFileChangedEventArgs(kind, this, name, total, progressed));
        }
    }
}
