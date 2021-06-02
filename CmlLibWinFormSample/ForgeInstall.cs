using CmlLib.Core;
using CmlLib.Core.Downloader;
using CmlLib.Core.Installer;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Windows.Forms;

namespace CmlLibWinFormSample
{
    public partial class ForgeInstall : Form
    {
        public ForgeInstall(MinecraftPath path, string java)
        {
            this.javapath = java;
            this.Path = path;
            logQueue = new ConcurrentQueue<string>();
            InitializeComponent();
        }

        public string LastInstalledVersion { get; private set; }
        MinecraftPath Path;
        string javapath;

        ConcurrentQueue<string> logQueue;

        private void btnInstall_Click(object sender, EventArgs e)
        {
            btnInstall.Enabled = false;
            txtMC.Enabled = false;
            txtForge.Enabled = false;

            new Thread(() =>
            {
                try
                {
                    var forge = new MForge(Path, javapath);
                    forge.FileChanged += Forge_FileChanged;
                    forge.InstallerOutput += Forge_InstallerOutput;
                    var versionName = forge.InstallForge(txtMC.Text, txtForge.Text);
                    LastInstalledVersion = versionName;
                    MessageBox.Show($"{versionName} was successfully installed");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    Invoke(new Action(() =>
                    {
                        btnInstall.Enabled = true;
                        txtMC.Enabled = true;
                        txtForge.Enabled = true;
                    }));
                }
            }).Start();
        }

        private void Forge_InstallerOutput(object sender, string e)
        {
            if (e != null)
                logQueue.Enqueue(e);
        }

        private void Forge_FileChanged(DownloadFileChangedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                lbStatus.Text = $"{e.FileKind} - {e.FileName}";
                pbProgress.Maximum = e.TotalFileCount;
                pbProgress.Value = e.ProgressedFileCount;
            }));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string item;
            while (logQueue.TryDequeue(out item))
            {
                txtLog.AppendText(item);
                txtLog.AppendText("\n");
            }
        }
    }
}
