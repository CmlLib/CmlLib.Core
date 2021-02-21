using CmlLib.Core.Auth;
using System;
using System.Threading;
using System.Windows.Forms;

namespace CmlLibWinFormSample
{
    public partial class LoginForm : Form
    {
        public LoginForm(MSession session)
        {
            this.Session = session;
            InitializeComponent();
        }

        public MSession Session;
        MLogin login = new MLogin();

        private void LoginForm_Load(object sender, EventArgs e)
        {
            UpdateSession(Session);
            btnAutoLogin_Click(null, null);
        }

        private void btnAutoLogin_Click(object sender, EventArgs e)
        {
            gMojangLogin.Enabled = false;

            var th = new Thread(() =>
            {
                var result = login.TryAutoLogin();

                if (result.Result != MLoginResult.Success)
                {
                    MessageBox.Show($"Failed to AutoLogin : {result.Result}\n{result.ErrorMessage}");
                    Invoke(new Action(() =>
                    {
                        gMojangLogin.Enabled = true;
                    }));
                    return;
                }

                MessageBox.Show("Auto Login Success!");
                Invoke(new Action(() =>
                {
                    gMojangLogin.Enabled = true;

                    btnAutoLogin.Enabled = false;
                    btnLogin.Enabled = false;
                    btnAutoLoginMojangLauncher.Enabled = false;
                    btnLogin.Text = "Auto Login\nSuccess";

                    UpdateSession(result.Session);
                }));
            });
            th.Start();
        }

        private void btnAutoLoginMojangLauncher_Click(object sender, EventArgs e)
        {
            gMojangLogin.Enabled = false;

            var th = new Thread(() =>
            {
                var result = login.TryAutoLoginFromMojangLauncher();

                if (result.Result != MLoginResult.Success)
                {
                    MessageBox.Show($"Failed to AutoLogin : {result.Result}\n{result.ErrorMessage}");
                    Invoke(new Action(() =>
                    {
                        gMojangLogin.Enabled = true;
                    }));
                    return;
                }

                MessageBox.Show("Auto Login Success!");
                Invoke(new Action(() =>
                {
                    gMojangLogin.Enabled = true;

                    btnAutoLogin.Enabled = false;
                    btnAutoLoginMojangLauncher.Enabled = false;
                    btnLogin.Enabled = false;
                    btnLogin.Text = "Auto Login\nSuccess";

                    UpdateSession(result.Session);
                }));
            });
            th.Start();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Empty Textbox");
                return;
            }

            gMojangLogin.Enabled = false;

            var th = new Thread(new ThreadStart(delegate
            {
                var result = login.Authenticate(txtEmail.Text, txtPassword.Text);
                if (result.Result == MLoginResult.Success)
                {
                    MessageBox.Show("Login Success"); // Success Login
                    Invoke(new Action(() =>
                    {
                        UpdateSession(result.Session);
                    }));
                }
                else
                {
                    MessageBox.Show(result.Result.ToString() + "\n" + result.ErrorMessage); // Failed to login. Show error message
                    Invoke(new Action(() =>
                    {
                        gMojangLogin.Enabled = true;
                    }));
                }
            }));
            th.Start();
        }

        private void btnSignout_Click(object sender, EventArgs e)
        {
            var result = login.Signout(txtEmail.Text, txtPassword.Text);
            if (result)
            {
                MessageBox.Show("Success");
                gMojangLogin.Enabled = true;
            }
            else
                MessageBox.Show("Fail");
        }

        private void btnInvalidate_Click(object sender, EventArgs e)
        {
            var result = login.Invalidate();
            if (result)
            {
                MessageBox.Show("Success");
                gMojangLogin.Enabled = true;
            }
            else
                MessageBox.Show("Fail");
        }

        private void btnDeleteToken_Click(object sender, EventArgs e)
        {
            login.DeleteTokenFile();
            MessageBox.Show("Success");
            gMojangLogin.Enabled = true;
        }

        private void btnOfflineLogin_Click(object sender, EventArgs e)
        {
            UpdateSession(MSession.GetOfflineSession(txtUsername.Text));
            MessageBox.Show("Success");
        }

        private void UpdateSession(MSession session)
        {
            this.Session = session;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                $"AccessToken : {Session.AccessToken}\n" +
                $"UUID : {Session.UUID}\n" +
                $"Username : {Session.Username}");

            this.Close();
        }
    }
}
