using System.Diagnostics;

namespace CmlLib.Core.ProcessBuilder;

public class ProcessWrapper
{
    public event EventHandler<string>? OutputReceived;
    public event EventHandler? Exited;

    public Process Process { get; private set; }

    public ProcessWrapper(Process process)
    {
        this.Process = process;
    }

    public void StartWithEvents()
    {
        Process.StartInfo.CreateNoWindow = false;
        Process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
        Process.StartInfo.UseShellExecute = false;
        Process.StartInfo.RedirectStandardError = true;
        Process.StartInfo.RedirectStandardOutput = true;
        Process.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
        Process.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
        Process.EnableRaisingEvents = true;
        Process.ErrorDataReceived += (s, e) => OutputReceived?.Invoke(this, e.Data ?? "");
        Process.OutputDataReceived += (s, e) => OutputReceived?.Invoke(this, e.Data ?? "");
        Process.Exited += (s, e) => Exited?.Invoke(this, new EventArgs());

        Process.Start();
        Process.BeginErrorReadLine();
        Process.BeginOutputReadLine();
    }

    public async Task<int> WaitForExitTaskAsync()
    {
        await Task.Run(() =>
        {
            Process.WaitForExit();
        });
        return Process.ExitCode;
    }
}
