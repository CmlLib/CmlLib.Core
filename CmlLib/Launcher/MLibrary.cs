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

        internal static List<MLibrary> ParseJson(JArray json)
        {
            var list = new List<MLibrary>();
            foreach (JObject item in json)
            {
                try
                {
                    // FORGE 라이브러리
                    if (item["downloads"] == null)
                    {
                        bool isn = item["natives"]?["windows"] != null;
                        var nativeStr = "";
                        if (isn)
                            nativeStr = item["natives"]["windows"].ToString();

                        var libName = item["name"]?.ToString();
                        if (libName == null) continue;

                        var libPath = MLibraryNameParser.NameToPath(libName, nativeStr);

                        var url = item["url"]?.ToString();
                        if (url == null)
                            url = DefaultLibraryServer + libPath;
                        else if (url.Split('/').Last() == "")
                            url += libPath;

                        var absoluteLibPath = Minecraft.Library + libPath;

                        list.Add(new MLibrary(isn, libName, absoluteLibPath, url));

                        continue;
                    }

                    var name = item["name"]?.ToString();

                    // NATIVE 라이브러리
                    var classif = item["downloads"]["classifiers"];
                    if (classif != null)
                    {
                        JObject job = null;
                        bool isgo = true;

                        if (classif["natives-windows-64"] != null && Environment.Is64BitOperatingSystem)
                            job = (JObject)classif["natives-windows-64"];
                        else if (classif["natives-windows-32"] != null)
                            job = (JObject)classif["natives-windows-32"];
                        else if (classif["natives-windows"] != null)
                            job = (JObject)classif["natives-windows"];
                        else
                            isgo = false;

                        if (isgo)
                        {
                            var obj = new MLibrary(true, name, job);
                            list.Add(obj);
                        }
                    }

                    // 일반 LIBRARY
                    var arti = item["downloads"]["artifact"];
                    if (arti != null)
                    {
                        var job = (JObject)arti;
                        var obj = new MLibrary(false, name, job);
                        list.Add(obj);
                    }
                }
                catch { }
            }

            return list;
        }

        internal MLibrary(bool isn, string name, JObject job)
        {
            IsNative = isn;
            Name = name;

            if (job != null)
            {
                Path = Minecraft.Library + job["path"]?.ToString();
                Url = job["url"]?.ToString();
            }
        }

        internal MLibrary(bool isn, string name, string path, string url)
        {
            IsNative = isn; Name = name; Path = path; Url = url;
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
