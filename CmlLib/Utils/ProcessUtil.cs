using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CmlLib.Utils
{
    public class ProcessUtil
    {
        public event EventHandler<string> OutputReceived;
        public event EventHandler Exited;

        public Process Process { get; private set; }

        public ProcessUtil(Process process)
        {
            this.Process = process;
        }

        public void StartWithEvents()
        {
            Process.StartInfo.CreateNoWindow = true;
            Process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            Process.StartInfo.UseShellExecute = false;
            Process.StartInfo.RedirectStandardError = true;
            Process.StartInfo.RedirectStandardOutput = true;
            Process.EnableRaisingEvents = true;
            Process.ErrorDataReceived += (s, e) => OutputReceived?.Invoke(this, e.Data);
            Process.OutputDataReceived += (s, e) => OutputReceived?.Invoke(this, e.Data);
            Process.Exited += (s, e) => Exited?.Invoke(this, new EventArgs());

            Process.Start();
            Process.BeginErrorReadLine();
            Process.BeginOutputReadLine();
        }

        public Task WaitForExitTaskAsync()
        {
            return Task.Run(() =>
            {
                Process.WaitForExit();
            });
        }
    }
}
