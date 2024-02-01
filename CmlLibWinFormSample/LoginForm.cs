using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft;
using System;
using System.Windows.Forms;

namespace CmlLibWinFormSample
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        JELoginHandler loginHandler;
        MSession session;

        private async void LoginForm_Load(object sender, EventArgs e)
        {
            this.Enabled = false;

            loginHandler = JELoginHandlerBuilder.BuildDefault();

            try
            {
                session = await loginHandler.AuthenticateSilently();
                txtXboxUsername.Text = session.Username;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            this.Enabled = true;
        }

        private async void btnAddNewAccount_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            try
            {
                session = await loginHandler.AuthenticateInteractively();
                txtXboxUsername.Text = session.Username;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this.Enabled = true;
        }

        private async void btnSignout_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            try
            {
                await loginHandler.SignoutWithBrowser();
                session = null;
                txtXboxUsername.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            this.Enabled = true;
        }

        private void btnXboxLogin_Click(object sender, EventArgs e)
        {
            if (session == null)
            {
                MessageBox.Show("Click 'Add a new account' first");
            }
            else
            {
                UpdateSession(session);
            }
        }

        private void btnOfflineLogin_Click(object sender, EventArgs e)
        {
            UpdateSession(MSession.CreateOfflineSession(txtUsername.Text));
        }

        private void UpdateSession(MSession session)
        {
            // Success to login!

            var mainForm = new MainForm(session);
            mainForm.FormClosed += (s, e) => this.Close();
            mainForm.Show();
            this.Hide();
        }
    }
}
