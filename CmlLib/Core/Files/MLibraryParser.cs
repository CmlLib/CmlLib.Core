using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CmlLib.Utils;

namespace CmlLib.Core.Files
{
    public class MLibraryParser
    {
        public bool CheckOSRules { get; set; } = true;

        public MLibrary[]? ParseJsonObject(JsonElement item)
        {
            try
            {
                var list = new List<MLibrary>();

                var name = item.GetPropertyValue("name");
                var isRequire = true;

                // check rules array
                if (CheckOSRules && item.TryGetProperty("rules", out var rules))
                    isRequire = MRule.CheckOSRequire(rules);

                // forge clientreq
                var req = item.GetPropertyValue("clientreq");
                if (req != null && req.ToLower() != "true")
                    isRequire = false;

                // support TLauncher
                var artifact = item.SafeGetProperty("artifact")
                               ?? item.SafeGetProperty("downloads")?.SafeGetProperty("artifact");
                var classifiers = item.SafeGetProperty("classifies")
                                  ?? item.SafeGetProperty("downloads")?.SafeGetProperty("classifiers");

                // NATIVE library
                var natives = item.SafeGetProperty("natives");
                if (natives != null)
                {
                    var nativeId = natives?.GetPropertyValue(MRule.OSName)?
                        .Replace("${arch}", MRule.Arch);

                    if (classifiers != null && nativeId != null)
                    {
                        var lObj = classifiers?.SafeGetProperty(nativeId) ?? classifiers?.SafeGetProperty(MRule.OSName);
                        if (lObj != null)
                            list.Add(createMLibrary(name, nativeId, isRequire, lObj));
                    }
                    else
                        list.Add(createMLibrary(name, nativeId, isRequire, null));
                }

                // COMMON library
                if (artifact != null)
                {
                    var obj = createMLibrary(name, "", isRequire, artifact);
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

        private MLibrary createMLibrary(string? name, string? nativeId, bool require, JsonElement? element)
        {
            string? path = element?.GetPropertyValue("path");
            if (string.IsNullOrEmpty(path) && !string.IsNullOrEmpty(name))
                path = PackageName.Parse(name).GetPath(nativeId);

            var hash = element?.GetPropertyValue("sha1");
            if (string.IsNullOrEmpty(hash))
            {
                var checksums = element?.SafeGetProperty("checksums")?.EnumerateArray();
                hash = checksums?.FirstOrDefault().GetString();
            }

            long size = 0;
            element?.SafeGetProperty("size")?.TryGetInt64(out size);

            return new MLibrary
            {
                Hash = hash,
                IsNative = !string.IsNullOrEmpty(nativeId),
                Name = name,
                Path = path,
                Size = size,
                Url = element?.GetPropertyValue("url"),
                IsRequire = require
            };
        }
    }
}
