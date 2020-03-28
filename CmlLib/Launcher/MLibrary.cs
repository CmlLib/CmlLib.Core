using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmlLib.Launcher
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

                        // FORGE library
                        var downloads = item["downloads"];
                        if (downloads == null)
                        {
                            var natives = item["natives"];
                            var nativeId = "";

                            if (natives != null)
                                nativeId = natives[MRule.OSName]?.ToString();

                            list.Add(createMLibrary(libraryPath, name, nativeId, item));

                            continue;
                        }

                        // NATIVE library
                        var classif = item["downloads"]?["classifiers"];
                        if (classif != null)
                        {
                            var nativeId = "";

                            var nativeObj = item["natives"];
                            if (nativeObj != null)
                                nativeId = nativeObj[MRule.OSName]?.ToString();

                            if (nativeId != null && classif[nativeId] != null)
                            {
                                nativeId = nativeId.Replace("${arch}", MRule.Arch);
                                var lObj = (JObject)classif[nativeId];
                                list.Add(createMLibrary(libraryPath, name, nativeId, lObj));
                            }
                        }

                        // COMMON library
                        var arti = item["downloads"]?["artifact"];
                        if (arti != null)
                        {
                            var obj = createMLibrary(libraryPath, name, "", (JObject)arti);
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
                if (path == null || path == "")
                    path = NameToPath(name, nativeId);

                var url = job["url"]?.ToString();
                if (url == null)
                    url = MojangServer.Library + path;
                else if (url.Split('/').Last() == "")
                    url += path;

                var library = new MLibrary();
                library.Hash = job["sha1"]?.ToString() ?? "";
                library.IsNative = (nativeId != "");
                library.Name = name;
                library.Path = System.IO.Path.Combine(libraryPath, path);
                library.Url = url;

                return library;
            }
        }
    }
}
