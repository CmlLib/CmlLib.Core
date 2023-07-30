using System;
using System.Windows.Forms;

namespace CmlLibWinFormSample
{
    public partial class JavaForm : Form
    {
        public string JavaBinaryPath { get; set; }

        public JavaForm(string javaPath)
        {
            this.JavaBinaryPath = javaPath;
            InitializeComponent();
        }

        private void JavaForm_Load(object sender, EventArgs e)
        {
            txtUserJava.Text = JavaBinaryPath;
            if (string.IsNullOrEmpty(JavaBinaryPath))
                cbAutoJava.Checked = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void JavaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.JavaBinaryPath = txtUserJava.Text;
        }

        private void cbAutoJava_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAutoJava.Checked)
                txtUserJava.Clear();
            txtUserJava.Enabled = !cbAutoJava.Checked;
        }
    }
}
