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
        private const string MavenServer = "https://files.minecraftforge.net/maven/net/minecraftforge/forge/";

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
            this.Minecraft = mc;
            JavaPath = java;
            Downloader = new SequenceDownloader();
        }

        public string JavaPath { get; private set; }
        private readonly MinecraftPath Minecraft;
        private readonly IDownloader Downloader;
        public event DownloadFileChangedHandler FileChanged;
        public event EventHandler<string> InstallerOutput;

        public string InstallForge(string mcversion, string forgeversion)
        {
            var minecraftJar = Minecraft.GetVersionJarPath(mcversion);
            if (!File.Exists(minecraftJar))
                throw new IOException($"Install {mcversion} first");

            var installerPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            var installerStream = getInstallerStream(mcversion, forgeversion); // download installer
            var extractedFile = extractInstaller(installerStream, installerPath); // extract installer

            var profileObj = extractedFile.Item1; // version profile json
            var installerObj = extractedFile.Item2; // installer info

            // copy forge libraries to minecraft
            extractMaven(installerPath); // new installer
            extractUniversal(installerPath,
                installerObj["filePath"]?.ToString(), installerObj["path"]?.ToString()); // old installer

            // download libraries and processors
            checkLibraries(installerObj["libraries"] as JArray);

            // mapping client data
            var mapData = mapping(installerObj["data"] as JObject, "client", minecraftJar, installerPath);

            // process
            process(installerObj["processors"] as JArray, mapData);

            // version name like 1.16.2-forge-33.0.20
            var versionName = installerObj["target"]?.ToString()
                           ?? installerObj["version"]?.ToString()
                           ?? GetForgeName(mcversion, forgeversion);

            var versionPath = Minecraft.GetVersionJsonPath(versionName);

            // write version profile json
            writeProfile(profileObj, versionPath);

            return versionName;
        }

        private Stream getInstallerStream(string mcversion, string forgeversion)
        {
            fireEvent(MFile.Library, "installer", 1, 0);

            var url = $"{MavenServer}{mcversion}-{forgeversion}/" +
                $"forge-{mcversion}-{forgeversion}-installer.jar";

            return WebRequest.Create(url).GetResponse().GetResponseStream();
        }

        private Tuple<JToken, JToken> extractInstaller(Stream stream, string extractPath)
        {
            // extract installer
            string install_profile = null;
            string versions_json = null;

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
                            install_profile = readStreamString(s);
                        else if (e.Name == "version.json")
                            versions_json = readStreamString(s);
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(realpath));

                            using (var fs = File.OpenWrite(realpath))
                                s.CopyTo(fs);
                        }
                    }
                }
            }

            JToken profileObj;
            var installObj = JObject.Parse(install_profile); // installer info
            var versionInfo = installObj["versionInfo"]; // version profile

            if (versionInfo == null)
                profileObj = JObject.Parse(versions_json);
            else
            {
                installObj = installObj["install"] as JObject;
                profileObj = versionInfo;
            }

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
                IOUtil.CopyDirectory(org, Minecraft.Library, true);
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
            var des = Path.Combine(Minecraft.Library, desPath);

            if (File.Exists(universal))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(des));
                File.Copy(universal, des, true);
            }
        }

        // legacy
        private void downloadUniversal(string mcversion, string forgeversion)
        {
            fireEvent(MFile.Library, "universal", 1, 0);

            var forgeName = $"forge-{mcversion}-{forgeversion}";
            var baseUrl = $"{MavenServer}{mcversion}-{forgeversion}";

            var universalUrl = $"{baseUrl}/{forgeName}-universal.jar";
            var universalPath = Path.Combine(
                Minecraft.Library,
                "net",
                "minecraftforge",
                "forge",
                $"{mcversion}-{forgeversion}",
                $"forge-{mcversion}-{forgeversion}.jar"
            );

            Directory.CreateDirectory(Path.GetDirectoryName(universalPath));
            var downloader = new WebDownload();
            downloader.DownloadFile(universalUrl, universalPath);
        }

        private Dictionary<string, string> mapping(JObject data, string kind,
            string minecraftJar, string installerPath)
        {
            if (data == null)
                return null;

            // convert [path] to absolute path

            var dataMapping = new Dictionary<string, string>();
            foreach (var item in data)
            {
                var key = item.Key;
                var value = item.Value[kind]?.ToString();

                var fullpath = Mapper.ToFullPath(value, Minecraft.Library);
                if (fullpath == value)
                {
                    value = value.Trim('/');
                    dataMapping.Add(key, Path.Combine(installerPath, value));
                }
                else
                    dataMapping.Add(key, fullpath);
            }

            dataMapping.Add("SIDE", "CLIENT");
            dataMapping.Add("MINECRAFT_JAR", minecraftJar);

            return dataMapping;
        }

        private void checkLibraries(JArray jarr)
        {
            if (jarr == null || jarr.Count == 0)
                return;

            var libs = new List<MLibrary>();
            var parser = new MLibraryParser();
            foreach (var item in jarr)
            {
                var parsedLib = parser.ParseJsonObject((JObject)item);
                libs.AddRange(parsedLib);
            }

            var fileProgress = new Progress<DownloadFileChangedEventArgs>(
                e => FileChanged?.Invoke(e));
            
            var libraryChecker = new LibraryChecker();
            var lostLibrary = libraryChecker.CheckFiles(Minecraft, libs.ToArray(), fileProgress);
            
            Downloader.DownloadFiles(lostLibrary, fileProgress, null);
        }

        private void process(JArray processors, Dictionary<string, string> mapData)
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

        private bool checkProcessorOutputs(JObject outputs, Dictionary<string, string> mapData)
        {
            foreach (var outitem in outputs)
            {
                var key = Mapper.Interpolation(outitem.Key, mapData);
                var value = Mapper.Interpolation(outitem.Value.ToString(), mapData);

                if (!File.Exists(key) || !IOUtil.CheckSHA1(key, value))
                    return false;
            }

            return true;
        }

        private void startProcessor(JToken processor, Dictionary<string, string> mapData)
        {
            var name = processor["jar"]?.ToString();
            if (name == null)
                return;

            // jar
            var jar = PackageName.Parse(name);
            var jarpath = Path.Combine(Minecraft.Library, jar.GetPath());

            var jarfile = new JarFile(jarpath);
            var jarManifest = jarfile.GetManifest();

            // mainclass
            string mainclass = null;
            var hasMainclass = jarManifest?.TryGetValue("Main-Class", out mainclass) ?? false;
            if (!hasMainclass || string.IsNullOrEmpty(mainclass))
                return;

            // classpath
            var classpathObj = processor["classpath"];
            var classpath = new List<string>();
            if (classpathObj != null)
            {
                foreach (var libname in classpathObj)
                {
                    var lib = Path.Combine(Minecraft.Library,
                        PackageName.Parse(libname?.ToString()).GetPath());
                    classpath.Add(lib);
                }
            }
            classpath.Add(jarpath);

            // arg
            var argsArr = processor["args"] as JArray;
            string[] args = null;
            if (argsArr != null)
            {
                var arrStrs = argsArr.Select(x => x.ToString()).ToArray();
                args = Mapper.Map(arrStrs, mapData, Minecraft.Library);
            }

            startJava(classpath.ToArray(), mainclass, args);
        }

        private void startJava(string[] classpath, string mainclass, string[] args)
        {
            var arg =
                $"-cp {IOUtil.CombinePath(classpath)} " +
                $"{mainclass} " +
                $"{string.Join(" ", args)}";

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
            Directory.CreateDirectory(Path.GetDirectoryName(versionPath));
            File.WriteAllText(versionPath, profileObj.ToString());
        }

        private void fireEvent(MFile kind, string name, int total, int progressed)
        {
            FileChanged?.Invoke(new DownloadFileChangedEventArgs(kind, name, total, progressed));
        }
    }
}
