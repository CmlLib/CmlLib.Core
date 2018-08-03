using System;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace CmlLib.Utils
{
    public class MJava
    {
        public static string DefaultRuntimeDirectory = CmlLib.Launcher.Minecraft.mPath + "\\runtime";

        public event DownloadProgressChangedEventHandler DownloadProgressChangedEvent;
        public event EventHandler DownloadCompletedEvent;
        public string RuntimeDirectory { get; private set; }

        public MJava() : this(DefaultRuntimeDirectory) { }

        public MJava(string runtimePath)
        {
            DownloadProgressChangedEvent += delegate { };
            DownloadCompletedEvent += delegate { };
            RuntimeDirectory = runtimePath;
        }

        public bool CheckJava()
        {
            return File.Exists(RuntimeDirectory + "\\bin\\java.exe");
        }

        public bool CheckJavaw()
        {
            return File.Exists(RuntimeDirectory + "\\bin\\javaw.exe");
        }

        string WorkingPath;
        public void DownloadJavaAsync()
        {
            string json = "";

            WorkingPath = Path.GetTempPath() + "\\temp_download_runtime";

            if (Directory.Exists(WorkingPath))
                DeleteDirectory(WorkingPath);
            Directory.CreateDirectory(WorkingPath);

            using (var wc = new WebClient())
            {
                json = wc.DownloadString("http://launchermeta.mojang.com/mc/launcher.json");

                var job = JObject.Parse(json)["windows"];
                var url = job[Environment.Is64BitOperatingSystem ? "64" : "32"]["jre"]["url"].ToString();

                Directory.CreateDirectory(RuntimeDirectory);
                wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                wc.DownloadFileAsync(new Uri(url), WorkingPath + "\\javatemp.lzma");
            }
        }

        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //var data = Convert.FromBase64String(Properties.Resources.zipLib);
            //File.WriteAllBytes(WorkingPath + "\\7za.exe", data);

            //zip("e -y \"javatemp.lzma\"");
            //zip("x -y -o\"" + RuntimeDirectory + "\" \"javatemp\"");

            var zip = new SevenZip(WorkingPath);
            zip.Decompress("javatemp.lzma", WorkingPath);
            zip.Decompress("javatemp", RuntimeDirectory);

            if (!File.Exists(RuntimeDirectory + "\\bin\\javaw.exe"))
            {
                try
                {
                    DeleteDirectory(RuntimeDirectory);
                }
                catch { }
                throw new Exception("Failed Download");
            }

            DownloadCompletedEvent(sender, new EventArgs());
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadProgressChangedEvent(sender, e);
        }

        //private void zip(string arg)
        //{
        //    Process pr = new Process();
        //    pr.StartInfo.WorkingDirectory = WorkingPath;
        //    pr.StartInfo.FileName = WorkingPath + "\\7za.exe";
        //    pr.StartInfo.Arguments = arg;
        //    //pr.StartInfo.RedirectStandardOutput = true;
        //    //pr.StartInfo.UseShellExecute = false;
        //    pr.StartInfo.CreateNoWindow = true;
        //    pr.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        //    pr.Start();
        //    pr.WaitForExit();
        //}

        private static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, true);
        }
    }
}
