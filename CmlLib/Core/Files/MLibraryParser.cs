using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace CmlLib.Core.Files
{
    public class MLibraryParser
    {
        public bool CheckOSRules { get; set; } = true;

        public MLibrary[] ParseJsonObject(JObject item)
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
                        JToken lObj = classifiers[nativeId] ?? classifiers[MRule.OSName];
                        if (lObj != null)
                            list.Add(createMLibrary(name, nativeId, isRequire, (JObject)lObj));
                    }
                    else
                        list.Add(createMLibrary(name, nativeId, isRequire, new JObject()));
                }

                // COMMON library
                if (artifact != null)
                {
                    MLibrary obj = createMLibrary(name, "", isRequire, (JObject)artifact);
                    list.Add(obj);
                }

                // library
                if (artifact == null && natives == null)
                {
                    MLibrary obj = createMLibrary(name, "", isRequire, item);
                    list.Add(obj);
                }

                return list.ToArray();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return null;
            }
        }

        private MLibrary createMLibrary(string name, string nativeId, bool require, JObject job)
        {
            string path = job["path"]?.ToString();
            if (string.IsNullOrEmpty(path))
                path = PackageName.Parse(name).GetPath(nativeId);

            var hash = job["sha1"] ?? job["checksums"]?[0];

            string sizestr = job["size"]?.ToString();
            long.TryParse(sizestr, out long size);

            return new MLibrary
            {
                Hash = hash?.ToString(),
                IsNative = !string.IsNullOrEmpty(nativeId),
                Name = name,
                Path = path,
                Size = size,
                Url = job["url"]?.ToString(),
                IsRequire = require
            };
        }
    }
}
