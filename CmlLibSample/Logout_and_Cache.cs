using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CmlLib.Launcher;

namespace CmlLibSample
{
    public partial class Logout_and_Cache : Form
    {
        public Logout_and_Cache()
        {
            InitializeComponent();
        }

        MLogin login = new MLogin();

        private void Form3_Load(object sender, EventArgs e)
        {
            var session = login.GetLocalToken();
            lvAT.Text = session.AccessToken;
            lvUsername.Text = session.Username;
            lvUUID.Text = session.UUID;
            lvCT.Text = session.ClientToken;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            // signout

            var result = login.Signout(txtEmail.Text, txtPassword.Text);
            if (result)
            {
                MessageBox.Show("Success");
                Application.Exit();
            }
            else
                MessageBox.Show("Failed");
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            // invalidate

            var result = login.Invalidate();

            if (result)
            {
                MessageBox.Show("Success");
                Application.Exit();
            }
            else
                MessageBox.Show("Failed");
        }
    }
}
