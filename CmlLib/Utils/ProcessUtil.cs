using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace CmlLib.Utils;

public class ProcessUtil
{
    public ProcessUtil(Process process)
    {
        Process = process;
    }

    public Process Process { get; }
    public event EventHandler<string>? OutputReceived;
    public event EventHandler? Exited;

    public void StartWithEvents()
    {
        Process.StartInfo.CreateNoWindow = true;
        Process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        Process.StartInfo.UseShellExecute = false;
        Process.StartInfo.RedirectStandardError = true;
        Process.StartInfo.RedirectStandardOutput = true;
        Process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
        Process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
        Process.EnableRaisingEvents = true;
        Process.ErrorDataReceived += (s, e) => OutputReceived?.Invoke(this, e.Data ?? "");
        Process.OutputDataReceived += (s, e) => OutputReceived?.Invoke(this, e.Data ?? "");
        Process.Exited += (s, e) => Exited?.Invoke(this, new EventArgs());

        Process.Start();
        Process.BeginErrorReadLine();
        Process.BeginOutputReadLine();
    }

    public Task WaitForExitTaskAsync()
    {
        return Task.Run(() => { Process.WaitForExit(); });
    }
}
