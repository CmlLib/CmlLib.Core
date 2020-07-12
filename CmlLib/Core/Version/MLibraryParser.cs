using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmlLib.Core.Version
{
    public class MLibraryParser
    {
        public static bool CheckOSRules = true;

        public static MLibrary[] ParseJsonObject(JObject item)
        {
            try
            {
                var list = new List<MLibrary>(2);

                var name = item["name"]?.ToString();
                var isRequire = true;

                // check rules array
                var rules = item["rules"];
                if (CheckOSRules && rules != null)
                    isRequire = MRule.CheckOSRequire((JArray)rules);

                // forge clientreq
                var req = item["clientreq"]?.ToString();
                if (req != null && req.ToLower() != "true")
                    isRequire = false;

                // support TLauncher
                var artifact = item["artifact"] ?? item["downloads"]?["artifact"];
                var classifiers = item["classifies"] ?? item["downloads"]?["classifiers"];
                var natives = item["natives"];

                // NATIVE library
                if (natives != null)
                {
                    var nativeId = natives[MRule.OSName]?.ToString()?.Replace("${arch}", MRule.Arch);

                    if (classifiers != null && nativeId != null)
                    {
                        JObject lObj = (JObject)classifiers[nativeId];
                        list.Add(createMLibrary(name, nativeId, isRequire, lObj));
                    }
                    else
                        list.Add(createMLibrary(name, nativeId, isRequire, new JObject()));
                }

                // COMMON library
                if (artifact != null)
                {
                    var obj = createMLibrary(name, "", isRequire, (JObject)artifact);
                    list.Add(obj);
                }

                // library
                if (artifact == null && natives == null)
                {
                    var obj = createMLibrary(name, "", isRequire, item);
                    list.Add(obj);
                }

                return list.ToArray();
            }
            catch (Exception ex)
            {

                return null;
            }
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

        private static MLibrary createMLibrary(string name, string nativeId, bool require, JObject job)
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
            library.IsNative = !string.IsNullOrEmpty(nativeId);
            library.Name = name;
            library.Path = path;
            library.Url = url;
            library.IsRequire = require;

            return library;
        }
    }
}
