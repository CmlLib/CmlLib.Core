using System;
using System.Diagnostics;
using System.IO;

namespace CmlLib.Utils
{
    public class SevenZip
    {
        public SevenZip(string workDir)
        {
            if (!Directory.Exists(workDir))
                throw new DirectoryNotFoundException("not exist " + workDir);

            this.WorkingDirectory = workDir;
            var data = Convert.FromBase64String(Properties.Resources.zipLib);
            File.WriteAllBytes(WorkingDirectory + "\\7za.exe", data);
        }

        public string WorkingDirectory { get; private set; }

        void cmd(string arg)
        {
            Process pr = new Process();
            pr.StartInfo.WorkingDirectory = WorkingDirectory;
            pr.StartInfo.FileName = WorkingDirectory + "\\7za.exe";
            pr.StartInfo.Arguments = arg;
            pr.StartInfo.CreateNoWindow = true;
            pr.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            pr.Start();
            pr.WaitForExit();
        }

        public void Decompress(string zip, string path)
        {
            // x file.zip -oPath
            string arg = $"x \"{zip}\" -o\"{path}\" -y";
            cmd(arg);
        }
    }
}
