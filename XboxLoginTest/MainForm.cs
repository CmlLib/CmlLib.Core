using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Downloader;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

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

        private async void btnStart_Click(object sender, EventArgs e)
        {
            btnLogin.Enabled = false;
            btnSignout.Enabled = false;
            btnStart.Enabled = false;

            try
            {
                var path = new MinecraftPath();
                var launcher = new CMLauncher(path);
                launcher.FileChanged += Launcher_FileChanged;
                launcher.ProgressChanged += Launcher_ProgressChanged;

                var versions = await launcher.GetAllVersionsAsync();
                var lastVersion = versions.LatestReleaseVersion;

                var process = await launcher.CreateProcessAsync(lastVersion.Name, new MLaunchOption()
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
        }

        private void Launcher_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbProgress.Value = e.ProgressPercentage;
        }

        private void Launcher_FileChanged(DownloadFileChangedEventArgs e)
        {
            pbPatch.Maximum = e.TotalFileCount;
            pbPatch.Value = e.ProgressedFileCount;

            lbMessage.Text = $"{e.FileKind} - {e.FileName}";
        }
    }
}
