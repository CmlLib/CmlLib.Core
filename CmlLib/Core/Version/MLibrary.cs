using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace CmlLib.Core.Version
{
    public class MLibrary
    {
        // class structure

        private MLibrary() { }

        public bool IsNative { get; private set; }

        public string Name { get; private set; }
        public string Path { get; private set; }
        public string Url { get; private set; }
        public bool IsRequire { get; private set; } = true;

        public string Hash { get; private set; } = "";

        // class builder
        internal class Parser
        {
            public static bool CheckOSRules = true;

            public static MLibrary[] ParseJson(string libraryPath, JArray json)
            {
                var ruleChecker = new MRule();
                var list = new List<MLibrary>(json.Count);
                foreach (JObject item in json)
                {
                    try
                    {
                        var name = item["name"]?.ToString();

                        // check rules array
                        var rules = item["rules"];
                        if (CheckOSRules && rules != null)
                        {
                            var isRequire = ruleChecker.CheckOSRequire((JArray)rules);

                            if (!isRequire)
                                continue;
                        }

                        // forge clientreq
                        var req = item["clientreq"]?.ToString();
                        if (req != null && req.ToLower() != "true")
                            continue;

                        // support TLauncher
                        var artifact = item["artifact"] ?? item["downloads"]?["artifact"];
                        var classifiers = item["classifies"] ?? item["downloads"]?["classifiers"];
                        var natives = item["natives"];
                        var nativeId = "";

                        // NATIVE library
                        if (natives != null)
                        {
                            nativeId = natives[MRule.OSName]?.ToString()?.Replace("${arch}", MRule.Arch);

                            if (classifiers != null && nativeId != null)
                            {
                                JObject lObj = (JObject)classifiers[nativeId];
                                list.Add(createMLibrary(libraryPath, name, nativeId, lObj));
                            }
                            else
                                list.Add(createMLibrary(libraryPath, name, nativeId, new JObject()));
                        }

                        // COMMON library
                        if (artifact != null)
                        {
                            var obj = createMLibrary(libraryPath, name, "", (JObject)artifact);
                            list.Add(obj);
                        }

                        // library
                        if (artifact == null && natives == null)
                        {
                            var obj = createMLibrary(libraryPath, name, "", item);
                            list.Add(obj);
                        }
                    }
                    catch { }
                }

                return list.ToArray();
            }

            private static string NameToPath(string name, string native)
            {
                try
                {
                    string[] tmp = name.Split(':');
                    string front = tmp[0].Replace('.', '/');
                    string back = name.Substring(name.IndexOf(':') + 1);

                    string libpath = front + "/" + back.Replace(':', '/') + "/" + back.Replace(':', '-');

                    if (native != "")
                        libpath += "-" + native + ".jar";
                    else
                        libpath += ".jar";
                    return libpath;
                }
                catch
                {
                    return "";
                }
            }

            private static MLibrary createMLibrary(string libraryPath, string name, string nativeId, JObject job)
            {
                var path = job["path"]?.ToString();
                if (string.IsNullOrEmpty(path))
                    path = NameToPath(name, nativeId);

                var url = job["url"]?.ToString();
                if (url == null)
                    url = MojangServer.Library + path;
                else if (url.Split('/').Last() == "")
                    url += path;

                var hash = job["sha1"] ?? job["checksums"]?[0];

                var library = new MLibrary();
                library.Hash = hash?.ToString() ?? "";
                library.IsNative = nativeId != "";
                library.Name = name;
                library.Path = System.IO.Path.Combine(libraryPath, path);
                library.Url = url;

                return library;
            }
        }
    }
}
