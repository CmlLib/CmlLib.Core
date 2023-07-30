﻿using CmlLib.Core.Auth;
using System;
using System.Threading;
using System.Windows.Forms;

namespace CmlLibWinFormSample
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        MLogin login = new MLogin();

        private void LoginForm_Load(object sender, EventArgs e)
        {
            btnAutoLogin_Click(null, null);
        }

        private async void btnAutoLogin_Click(object sender, EventArgs e)
        {
            gMojangLogin.Enabled = false;
            gOfflineLogin.Enabled = false;

            var result = await login.TryAutoLogin();

            if (result.Result != MLoginResult.Success)
            {
                MessageBox.Show($"Failed to AutoLogin : {result.Result}\n{result.ErrorMessage}");
                Invoke(new Action(() =>
                {
                    gMojangLogin.Enabled = true;
                    gOfflineLogin.Enabled = true;
                }));
                return;
            }

            MessageBox.Show("Auto Login Success!");
            Invoke(new Action(() =>
            {
                gMojangLogin.Enabled = true;
                gOfflineLogin.Enabled = true;

                btnAutoLogin.Enabled = false;
                btnLogin.Enabled = false;
                btnLogin.Text = "Auto Login\nSuccess";

                UpdateSession(result.Session);
            }));
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Empty Textbox");
                return;
            }

            gMojangLogin.Enabled = false;
            gOfflineLogin.Enabled = false;

            var result = await login.Authenticate(txtEmail.Text, txtPassword.Text);
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
                    gOfflineLogin.Enabled = true;
                }));
            }
        }

        private async void btnSignout_Click(object sender, EventArgs e)
        {
            var result = await login.Signout(txtEmail.Text, txtPassword.Text);
            if (result)
            {
                MessageBox.Show("Success");
                gMojangLogin.Enabled = true;
            }
            else
                MessageBox.Show("Fail");
        }

        private async void btnInvalidate_Click(object sender, EventArgs e)
        {
            var result = await login.Invalidate();
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
