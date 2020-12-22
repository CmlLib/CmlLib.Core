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
using CmlLib.Core;
using CmlLib.Core.Auth;

namespace XboxLoginTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
        }

        MSession session;

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var loginForm = new LoginForm();
            loginForm.action = "login";
            loginForm.ShowDialog();
            var session = loginForm.session;

            if (session != null)
                setSession(session);
        }

        private void setSession(MSession session)
        {
            this.session = session;
            btnStart.Enabled = true;
            txtAccessToken.Text = session.AccessToken;
            txtUUID.Text = session.UUID;
            txtUsername.Text = session.Username;
        }

        private void btnSignout_Click(object sender, EventArgs e)
        {
            var loginForm = new LoginForm();
            loginForm.action = "signout";
            loginForm.ShowDialog();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            btnSignout.Enabled = false;
            btnStart.Enabled = false;

            new Thread(() =>
            {
                try
                {
                    var path = new MinecraftPath();
                    var launcher = new CMLauncher(path);
                    launcher.FileChanged += Launcher_FileChanged;
                    launcher.ProgressChanged += Launcher_ProgressChanged;

                    var versions = launcher.GetAllVersions();
                    var lastVersion = versions.LatestReleaseVersion;

                    var process = launcher.CreateProcess(lastVersion.Name, new MLaunchOption()
                    {
                        Session = this.session
                    });

                    process.Start();
                    MessageBox.Show("Success");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }).Start();
        }

        private void Launcher_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                pbProgress.Value = e.ProgressPercentage;
            }));
        }

        private void Launcher_FileChanged(CmlLib.Core.Downloader.DownloadFileChangedEventArgs e)
        {
            Invoke(new Action(() =>
            {
                pbPatch.Maximum = e.TotalFileCount;
                pbPatch.Value = e.ProgressedFileCount;

                lbMessage.Text = $"{e.FileKind} - {e.FileName}";
            }));
        }
    }
}
