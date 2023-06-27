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
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Directory = System.IO.Directory;
using File = System.IO.File;

namespace CmlLib.Core.Installer
{
    public class MForge
    {
        private readonly MinecraftPath minecraftPath;
        private readonly string JavaPath;
        private CMLauncher launcher;
        private static HttpClient httpClient = new HttpClient();
        private readonly IDownloader downloader;
        public event DownloadFileChangedHandler? FileChanged;
        public event EventHandler<string>? InstallerOutput;

        public MForge(MinecraftPath mc, CMLauncher launcher, string java)
        {
            this.minecraftPath = mc;
            this.JavaPath = java;
            this.launcher = launcher;
            downloader = new SequenceDownloader();

        }

        /* 1.7.10 - 1.9.4 */
        public static string GetLegacyForgeName(string mcVersion, string forgeVersion) => $"forge-{mcVersion}-{forgeVersion}-{mcVersion}";
        
        /* 1.12 - *.*.* */
        public static string GetForgeName(string mcVersion, string forgeVersion) => $"{mcVersion}-forge-{forgeVersion}";

        /*1.10 - 1.11.2 */
        public static string GetOldForgeName(string mcVersion, string forgeVersion) => $"forge-{mcVersion}-{forgeVersion}";

        /* 1.7.10 - 1.11.2 */
        private static string GetLegacyFolderName(string mcVersion, string forgeVersion) => mcVersion == "1.7.10" ? 
            $"{mcVersion}-Forge-{forgeVersion}-{mcVersion}" : 
            $"{mcVersion}-forge{mcVersion}-{forgeVersion}";

        
        public async Task<string> Install(string mcVersion, string forgeVersion, bool AlwaysUpdate = false) => IsOldType(mcVersion) ? 
            await Legacy(mcVersion, forgeVersion, AlwaysUpdate) : 
            await Newest(mcVersion, forgeVersion, AlwaysUpdate);


        private async Task<string> Newest(string mcVersion, string forgeVersion, bool AlwaysUpdate = false)
        {
            if (!AlwaysUpdate && Directory.Exists(Path.Combine(minecraftPath.Versions, GetForgeName(mcVersion, forgeVersion))))
                return GetForgeName(mcVersion, forgeVersion); //the version is already installed

            var version_jar = minecraftPath.GetVersionJarPath(mcVersion); // get vanilla jar file
            var install_folder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()); //create folder in temp
            if (!System.IO.File.Exists(version_jar))
                await launcher.CheckAndDownloadAsync(launcher.GetVersion(mcVersion)); //install vanilla version

            await DownloadFile(mcVersion, forgeVersion, install_folder); //download forge version
            File.Copy(Path.Combine(install_folder, "installer.jar"), Path.Combine(install_folder, "version.zip"));
            new FastZip().ExtractZip(Path.Combine(install_folder, "version.zip"), install_folder, null); //unzip version
            
            var version = JObject.Parse(File.ReadAllText(Path.Combine(install_folder, "version.json")));
            var installer = JObject.Parse(File.ReadAllText(Path.Combine(install_folder, "install_profile.json")));
            var installerData = installer["data"] as JObject;
            var mapData = installerData == null ? new Dictionary<string, string?>() : mapping(installerData, "client", version_jar, install_folder);

            extractMaven(install_folder); //setup maven
            await checkLibraries(installer["libraries"] as JArray); //install libs
            process(installer["processors"] as JArray, mapData, install_folder);
            setupFolder(mcVersion, forgeVersion, install_folder, version.ToString()); //copy version.json and forge.jar

            //########################AD URL##############################
            Process.Start(getAdUrl()); //We support Forge developers!
            //########################AD URL##############################

            await launcher.GetAllVersionsAsync(); //update version list
            return GetForgeName(mcVersion, forgeVersion);
        }


        private async Task<string> Legacy(string mcVersion, string forgeVersion, bool AlwaysUpdate = false)
        {
            if (!AlwaysUpdate && Directory.Exists(Path.Combine(minecraftPath.Versions, GetLegacyFolderName(mcVersion, forgeVersion))))
                return $"{GetLegacyFolderName(mcVersion, forgeVersion)}";

            var version_jar = minecraftPath.GetVersionJarPath(mcVersion); // get vanilla jar file
            var install_folder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()); //create folder in temp
            if (!System.IO.File.Exists(version_jar))
                await launcher.CheckAndDownloadAsync(launcher.GetVersion(mcVersion)); //install vanilla version

            await DownloadFile(mcVersion, forgeVersion, install_folder); //download forge version
            File.Copy(Path.Combine(install_folder, "installer.jar"), Path.Combine(install_folder, "version.zip"));
            new FastZip().ExtractZip(Path.Combine(install_folder, "version.zip"), install_folder, null); //unzip version

            var installer = JObject.Parse(File.ReadAllText(Path.Combine(install_folder, "install_profile.json")));
            var version = installer["versionInfo"] as JObject;
            var installerData = installer["data"] as JObject;
            var mapData = installerData == null ? new Dictionary<string, string?>() : mapping(installerData, "client", version_jar, install_folder);
            var version_name = version["id"].ToString();
            var destPath = (installer["install"] as JObject)["path"]?.ToString();
            var universalPath = (installer["install"] as JObject)["filePath"]?.ToString();

            if (string.IsNullOrEmpty(universalPath)) throw new InvalidOperationException("filePath property in installer was null");
            if (string.IsNullOrEmpty(destPath)) throw new InvalidOperationException("path property in installer was null");

            extractUniversal(install_folder, universalPath, destPath); // old installer
            await checkLibraries(installer["libraries"] as JArray); //install libs
            process(installer["processors"] as JArray, mapData, install_folder);
            setupFolderLegacy(mcVersion, forgeVersion, install_folder, version_name, version.ToString()); //copy version.json and forge.jar

            //########################AD URL##############################
            Process.Start(getAdUrl()); //We support Forge developers!
            //########################AD URL##############################

            await launcher.GetAllVersionsAsync(); //update version list
            return version_name;
        }

        private void extractMaven(string installerPath)
        {
            var org = Path.Combine(installerPath, "maven");
            if (Directory.Exists(org))
                IOUtil.CopyDirectory(org, minecraftPath.Library, true);
        }

        private void extractUniversal(string installerPath, string universalPath, string destinyName)
        {

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

        private Task checkLibraries(JArray? jarr)
        {
            if (jarr == null || jarr.Count == 0)
                return Task.CompletedTask;

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

            if (lostLibrary == null)
                return Task.CompletedTask;
            downloader.DownloadFiles(lostLibrary, fileProgress, null).GetAwaiter().GetResult();
            return Task.CompletedTask;
        }

        private Dictionary<string, string?> mapping(JObject data, string kind,
            string minecraftJar, string installerPath)
        {
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

        private void process(JArray? processors, Dictionary<string, string?> mapData, string install_folder)
        {
            if (processors == null || processors.Count == 0)
                return;


            for (int i = 0; i < processors.Count; i++)
            {
                var item = processors[i];

                var outputs = item["outputs"] as JObject;
                if (outputs == null || !checkProcessorOutputs(outputs, mapData))
                    if (item["sides"] == null || (item["sides"] as JArray)[0].ToString() == "client") //skip server side
                        startProcessor(item, mapData, install_folder);

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

        private void startProcessor(JToken processor, Dictionary<string, string?> mapData, string install_folder)
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

            startJava(classpath.ToArray(), mainClass, args, install_folder);
        }

        private void startJava(string[] classpath, string mainClass, string[]? args, string install_folder)
        {
            for (int i = 0; i < args.Length; i++)
                if (args[i] == "{INSTALLER}")
                    args[i] = args[i].Replace("{INSTALLER}", Path.Combine(install_folder, "installer.jar"));
            var arg =
                $"-cp {IOUtil.CombinePath(classpath)} " +
                $"{mainClass}";

            if (args != null && args.Length > 0)
                arg += " " + string.Join(" ", args);

            var process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                FileName = JavaPath,
                Arguments = arg.Replace("--side CLIENT", "--side client"), //fix installertools bug
            };

            var p = new ProcessUtil(process);
            p.OutputReceived += (s, e) => 
            InstallerOutput?.Invoke(this, e);
            p.StartWithEvents();
            p.Process.WaitForExit();
        }

        private bool IsOldType(string mcVersion) => Convert.ToInt32(mcVersion.Split('.')[1]) < 12 ? true : false;

        private void setupFolder(string mcVersion, string forgeVersion, string install_folder, string JVersion)
        {
            string version_folder = Path.Combine(minecraftPath.Versions, GetForgeName(mcVersion, forgeVersion));
            if(Directory.Exists(version_folder))
                Directory.Delete(version_folder, true); //remove version folder
            Directory.CreateDirectory(version_folder); //create version folder
            File.WriteAllText(Path.Combine(version_folder, $"{GetForgeName(mcVersion, forgeVersion)}.json"), JVersion); //write version.json
            var jar = Path.Combine(install_folder, $"maven\\net\\minecraftforge\\forge\\{mcVersion}-{forgeVersion}\\forge-{mcVersion}-{forgeVersion}.jar");
            if (File.Exists(jar)) //fix 1.17+ errors
                File.Copy(jar, Path.Combine(version_folder, $"{GetForgeName(mcVersion, forgeVersion)}.jar")); //copy jar file
            Directory.Delete(install_folder, true); //remove temp folder
        }

        private void setupFolderLegacy(string mcVersion, string forgeVersion, string install_folder, string version_name, string JVersion)
        {
            string version_folder = Path.Combine(minecraftPath.Versions, version_name);
            var universal_jar = Convert.ToInt32(mcVersion.Split('.')[1]) < 10 ?
                $"{GetLegacyForgeName(mcVersion, forgeVersion)}-universal.jar" :
                $"{GetOldForgeName(mcVersion, forgeVersion)}-universal.jar";

            if (Directory.Exists(version_folder))
                Directory.Delete(version_folder, true); //remove version folder
            Directory.CreateDirectory(version_folder); //create version folder

            File.Copy(Path.Combine(install_folder, universal_jar),
                Path.Combine(version_folder, $"{version_name}.jar")); //copy jar file
            File.WriteAllText(Path.Combine(version_folder, $"{version_name}.json"), JVersion); //write version.json
            Directory.Delete(install_folder, true); //remove temp folder
        }

        public async Task<string> getForgeUrl(string mcVersion, string forgeVersion)
        {
            var document = new HtmlDocument();
            var html = await httpClient.GetStringAsync($"https://files.minecraftforge.net/net/minecraftforge/forge/index_{mcVersion}.html");
            document.LoadHtml(html);
            var rows = document.DocumentNode.SelectNodes("//html[1]//body[1]//main[1]//div[2]//div[2]//div[2]//table[1]//tbody[1]//tr").ToList();
            foreach (var row in rows)
            {
                var current_version = ClearName(row.Descendants(0).Where(n => n.HasClass("download-version")).FirstOrDefault().FirstChild.OuterHtml.Replace(" ", ""));
                if (current_version == forgeVersion)
                    return GetQueryString(row.ChildNodes[5].ChildNodes[1].ChildNodes[3].ChildNodes[3].Attributes["href"].Value, "url=");
            }
            throw new Exception("The version was not found on the official website");
        }

        public async Task<string> getForgeMD5(string mcVersion, string forgeVersion)
        {
            var document = new HtmlDocument();
            var html = await httpClient.GetStringAsync($"https://files.minecraftforge.net/net/minecraftforge/forge/index_{mcVersion}.html");
            document.LoadHtml(html);
            var rows = document.DocumentNode.SelectNodes("//html[1]//body[1]//main[1]//div[2]//div[2]//div[2]//table[1]//tbody[1]//tr").ToList();
            foreach (var row in rows)
            {
                var current_version = ClearName(row.Descendants(0).Where(n => n.HasClass("download-version")).FirstOrDefault().FirstChild.OuterHtml.Replace(" ", ""));
                if (current_version == forgeVersion)
                    return ClearName(row.ChildNodes[5].ChildNodes[1].ChildNodes[3].ChildNodes[5].ChildNodes[2].InnerText);
            }
            return "404";
        }

        private string getAdUrl() =>
            $"https://adfoc.us/serve/sitelinks/?id=271228&url=https://maven.minecraftforge.net/";

        private string ClearName(string name) => name.Replace(" ", "").Replace("\n", "");

        private string GetQueryString(string url, string key)
        {
            int index = url.IndexOf('?');
            var query = url.Substring(index + 1).Split('&').SingleOrDefault(s => s.StartsWith(key));
            return query == null ? url : query.Replace(key, null);
        }

        private async Task DownloadFile(string mcVersion, string forgeVersion, string install_folder)
        {
            System.IO.Directory.CreateDirectory(install_folder);
            var fileUrl = await getForgeUrl(mcVersion, forgeVersion);
            var httpResult = await httpClient.GetAsync(fileUrl);
            using var resultStream = await httpResult.Content.ReadAsStreamAsync();
            using var fileStream = System.IO.File.Create(Path.Combine(install_folder, "installer.jar"));
            resultStream.CopyTo(fileStream);
        }

        private static string CalculateMD5(string filename)
        {
            using var md5 = MD5.Create();
            using var stream = File.OpenRead(filename);
            var hash = md5.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private void fireEvent(MFile kind, string name, int total, int progressed)
        {
            FileChanged?.Invoke(new DownloadFileChangedEventArgs(kind, this, name, total, progressed));
        }
    }
}
