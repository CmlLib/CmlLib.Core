using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using CmlLib;
using CmlLib.Core;
using System.Diagnostics;
using CmlLib.Core.Downloader;
using CmlLib.Core.Auth;

namespace CmlLibWinFormSample
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        CMLauncher Launcher;
        MSession Session;

        GameLog logForm;
        bool allowOffline = true;

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // Initialize launcher
            Txt_Path.Text = MinecraftPath.GetOSDefaultPath();

            var th = new Thread(new ThreadStart(delegate
            {
                InitializeLauncher();

                // Try auto login

                var login = new MLogin();
                var result = login.TryAutoLogin();

                if (result.Result != MLoginResult.Success)
                {
                    Console.WriteLine(result.Result);
                    Console.WriteLine(result.ErrorMessage);
                    return;
                }

                MessageBox.Show("Auto Login Success!");

                Session = result.Session;

                Invoke(new Action(() => {
                    Btn_Login.Enabled = false;
                    Btn_Login.Text = "Auto Login\nSuccess";
                }));
            }));
            th.Start();
        }

        private void Btn_apply_Click(object sender, EventArgs e)
        {
            // Apply

            InitializeLauncher();
        }

        private void Cb_Forge_CheckedChanged(object sender, EventArgs e)
        {
            if (!Cb_Forge.Checked)
            {
                Txt_ForgeVersion.Clear();
                Txt_ForgeVersion.Enabled = false;
            }
            else
                Txt_ForgeVersion.Enabled = true;
        }

        private void InitializeLauncher()
        {
            Launcher = new CMLauncher(Txt_Path.Text);

            Launcher.FileChanged += Launcher_FileChanged;
            Launcher.ProgressChanged += Launcher_ProgressChanged;
            var versions = Launcher.GetAllVersions();

            Invoke(new Action(() => 
            {
                Cb_Version.Items.Clear();
                foreach (var item in versions)
                {
                    Cb_Version.Items.Add(item.Name);
                }
            }));
        }

        private void Btn_Login_Click(object sender, EventArgs e)
        {
            // Login

            Btn_Login.Enabled = false;
            if (Txt_Pw.Text == "") // Offline Login
            {
                if (allowOffline)
                {
                    Session = MSession.GetOfflineSession(Txt_Email.Text);
                    MessageBox.Show("Offline login Success : " + Txt_Email.Text);
                }
                else
                {
                    MessageBox.Show("Password was empty");
                    Btn_Login.Enabled = true;
                    return;
                }
            }
            else // Online Login
            {
                var th = new Thread(new ThreadStart(delegate
                {
                    var login = new MLogin();
                    var result = login.Authenticate(Txt_Email.Text, Txt_Pw.Text);
                    if (result.Result == MLoginResult.Success)
                    {
                        MessageBox.Show("Login Success : " + result.Session.Username); // Success Login
                        Session = result.Session;
                    }
                    else
                    {
                        MessageBox.Show(result.Result.ToString() + "\n" + result.ErrorMessage); // Failed to login. Show error message
                        Invoke((MethodInvoker)delegate { Btn_Login.Enabled = true; });
                    }
                }));
                th.Start();
            }
        }

        private void Btn_Launch_Click(object sender, EventArgs e)
        {
            // Launch

            if (Session == null)
            {
                MessageBox.Show("Login First");
                return;
            }

            if (Cb_Version.Text == "") return;

            groupBox1.Enabled = false;
            groupBox2.Enabled = false;

            if (cbFast.Checked)
            {
                Launcher.Minecraft.SetAssetsPath(Path.Combine(MinecraftPath.GetOSDefaultPath(), "assets"));
                Launcher.Minecraft.Runtime = Path.Combine(MinecraftPath.GetOSDefaultPath(), "runtime");
            }

            try
            {
                var version = Cb_Version.Text;
                var forge = "";

                if (Cb_Forge.Checked)
                    forge = Txt_ForgeVersion.Text;

                var launchOption = new MLaunchOption()
                {
                    JavaPath = Txt_Java.Text,
                    MaximumRamMb = int.Parse(Txt_Ram.Text),
                    Session = this.Session,

                    VersionType = Txt_VersionType.Text,
                    GameLauncherName = Txt_GLauncherName.Text,
                    GameLauncherVersion = Txt_GLauncherVersion.Text,

                    ServerIp = Txt_ServerIp.Text,

                    DockName = Txt_DockName.Text,
                    DockIcon = Txt_DockIcon.Text
                };

                if (!string.IsNullOrEmpty(Txt_ServerPort.Text))
                    launchOption.ServerPort = int.Parse(Txt_ServerPort.Text);

                if (!string.IsNullOrEmpty(Txt_ScWd.Text) && !string.IsNullOrEmpty(Txt_ScHt.Text))
                {
                    launchOption.ScreenHeight = int.Parse(Txt_ScHt.Text);
                    launchOption.ScreenWidth = int.Parse(Txt_ScWd.Text);
                }

                if (!string.IsNullOrEmpty(Txt_JavaArgs.Text))
                    launchOption.JVMArguments = Txt_JavaArgs.Text.Split(' ');

                var th = new Thread(() =>
                {
                    try
                    {
                        Process mc;

                        if (string.IsNullOrEmpty(forge))
                            mc = Launcher.CreateProcess(version, launchOption); // vanilla
                        else
                            mc = Launcher.CreateProcess(version, forge, launchOption); // forge

                        StartProcess(mc);
                    }
                    catch (MDownloadFileException mex)
                    {
                        MessageBox.Show(
                            $"FileName : {mex.ExceptionFile.Name}\n" +
                            $"FilePath : {mex.ExceptionFile.Path}\n" +
                            $"FileUrl : {mex.ExceptionFile.Url}\n" +
                            $"FileType : {mex.ExceptionFile.Type}\n\n" +
                            mex.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                });
                th.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                var fWork = new Action(() =>
                {
                    if (logForm != null)
                        logForm.Close();

                    logForm = new GameLog();
                    logForm.Show();

                    groupBox1.Enabled = true;
                    groupBox2.Enabled = true;
                });

                if (this.InvokeRequired)
                    this.Invoke(fWork);
                else
                    fWork.Invoke();
            }
        }

        private void Launcher_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                Pb_Progress.Value = e.ProgressPercentage;
            }));
        }

        private void Launcher_FileChanged(DownloadFileChangedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                Pb_File.Maximum = e.TotalFileCount;
                Pb_File.Value = e.ProgressedFileCount;
                Lv_Status.Text = $"{e.FileKind.ToString()} : {e.FileName} ({e.ProgressedFileCount}/{e.TotalFileCount})";
            }));
        }

        private void StartProcess(Process process)
        {
            File.WriteAllText("launcher.txt", process.StartInfo.Arguments);
            output(process.StartInfo.Arguments);

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents = true;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.OutputDataReceived += Process_OutputDataReceived;

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            output(e.Data);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            output(e.Data);
        }

        void output(string msg)
        {
            Invoke(new Action(() =>
            {
                if (logForm != null && !logForm.IsDisposed)
                    logForm.AddLog(msg);
            }));
        }

        private void Btn_loginOption_Click(object sender, EventArgs e)
        {
            var form3 = new Logout_and_Cache();
            form3.Show();
        }

        private void btnGithub_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("https://github.com/AlphaBs/CmlLib.Core");
            }
            catch
            {

            }
        }

        private void btnWiki_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("https://github.com/AlphaBs/CmlLib.Core/wiki/MLaunchOption");
            }
            catch
            {

            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void btnChangelog_Click(object sender, EventArgs e)
        {
            var f = new ChangeLog();
            f.Show();
        }
    }
}
