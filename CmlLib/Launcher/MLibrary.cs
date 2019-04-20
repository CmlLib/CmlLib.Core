using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmlLib.Launcher
{
    /// <summary>
    /// 라이브러리
    /// </summary>
    public class MLibrary
    {
        static string DefaultLibraryServer = "https://libraries.minecraft.net/";

        internal static MLibrary[] ParseJson(JArray json)
        {
            var list = new List<MLibrary>(json.Count);
            foreach (JObject item in json)
            {
                try
                {
                    var name = item["name"]?.ToString();

                    // FORGE 라이브러리
                    if (item["downloads"] == null)
                    {
                        bool isn = item["natives"]?["windows"] != null;
                        var nativeStr = "";
                        if (isn)
                            nativeStr = item["natives"]["windows"].ToString();

                        if (name == null) continue;

                        list.Add(new MLibrary(name, nativeStr, item));

                        continue;
                    }

                    // NATIVE 라이브러리
                    var classif = item["downloads"]["classifiers"];
                    if (classif != null)
                    {
                        JObject job = null;
                        bool isgo = true;

                        var nativeId = "";

                        if (classif["natives-windows-64"] != null && Environment.Is64BitOperatingSystem)
                            nativeId = "natives-windows-64";
                        else if (classif["natives-windows-32"] != null)
                            nativeId = "natives-windows-32";
                        else if (classif["natives-windows"] != null)
                            nativeId = "natives-windows";
                        else
                            isgo = false;

                        job = (JObject)classif[nativeId];

                        if (isgo)
                        {
                            var obj = new MLibrary(name, nativeId, job);
                            list.Add(obj);
                        }
                    }

                    // 일반 LIBRARY
                    var arti = item["downloads"]["artifact"];
                    if (arti != null)
                    {
                        var job = (JObject)arti;

                        var obj = new MLibrary(name, "", job);
                        list.Add(obj);
                    }
                }
                catch { }
            }

            return list.ToArray();
        }

        internal MLibrary(string name, string nativeId, JObject job)
        {
            var path = job["path"]?.ToString();
            if (path == null || path == "")
                path = MLibraryNameParser.NameToPath(name, nativeId);

            var url = job["url"]?.ToString();
            if (url == null)
                url = DefaultLibraryServer + path;
            else if (url.Split('/').Last() == "")
                url += path;

            Hash = job["sha1"]?.ToString() ?? "";
            IsNative = (nativeId != "");
            Name = name;
            Path = Minecraft.Library + path;
            Url = url;
        }

        /// <summary>
        /// true 이면 네이티브 라이브러리, 아니면 일반 라이브러리입니다.
        /// </summary>
        public bool IsNative { get; private set; }
        /// <summary>
        /// 라이브러리의 이름
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 라이브러리가 설치되야 할 경로
        /// </summary>
        public string Path { get; private set; }
        /// <summary>
        /// 라이브러리를 다운로드 할 수 있는 URL
        /// </summary>
        public string Url { get; private set; }
        /// <summary>
        /// 현재 OS 에서 필요한 라이브러리인지 확인
        /// </summary>
        public bool IsRequire { get; private set; } = true;

        public string Hash { get; private set; } = "";
    }

    class MLibraryNameParser
    {
        public static string NameToPath(string name, string native)
        {
            try
            {
                string[] tmp = name.Split(':');
                string front = tmp[0].Replace('.', '/');
                string back = "";

                for (int i = 1; i <= tmp.Length - 1; i++)
                {
                    if (i == tmp.Length - 1)
                        back += tmp[i];
                    else
                        back += tmp[i] + ":";
                }
                string libpath = front + "/" + back.Replace(':', '/') + "/" + (back.Replace(':', '-'));
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
    }
}
