using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace CmlLib.Launcher
{
    /// <summary>
    /// 라이브러리
    /// </summary>
    public class MLibrary
    {
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
                        string forge = MLibraryNameParser.NameToPath(item);

                        if (forge == "") continue;

                        bool isn = false;
                        if (forge.Contains("natives"))
                            isn = true;
                        else
                            isn = false;

                        var url = item["url"]?.ToString();
                        if (url == null) url = "";
                        list.Add(new MLibrary(isn, item["name"]?.ToString(), forge, ""));

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
        public static string NameToPath(JObject job)
        {
            try
            {
                string name = job["name"]?.ToString();

                string front = name.Split(':')[0].Replace('.', '\\');
                string back = "";
                string[] tmp = name.Split(':');
                for (int i = 1; i <= tmp.Length - 1; i++)
                {
                    if (i == tmp.Length - 1)
                        back += tmp[i];
                    else
                        back += tmp[i] + ":";
                }
                string libpath = Minecraft.Library + front + "\\" + back.Replace(':', '\\') + "\\" + (back.Replace(':', '-'));
                if (job.ToString().Contains("natives"))
                {
                    string window = job["natives"]["windows"].ToString();
                    libpath += "-" + window + ".jar";
                }
                else
                {
                    libpath += ".jar";
                }
                return libpath;
            }
            catch
            {
                return "";
            }
        }
    }
}
