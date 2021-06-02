using System;
using System.Windows.Forms;

namespace CmlLibWinFormSample
{
    public partial class JavaForm : Form
    {
        public bool UseMJava { get; set; }
        public string MJavaDirectory { get; set; }
        public string JavaBinaryPath { get; set; }

        public JavaForm(bool useMJava, string mjavaDir, string javaPath)
        {
            this.UseMJava = useMJava;
            this.MJavaDirectory = mjavaDir;
            this.JavaBinaryPath = javaPath;

            InitializeComponent();
        }

        private void JavaForm_Load(object sender, EventArgs e)
        {
            rbAutoJava.Checked = UseMJava;
            txtJavaDirectory.Text = MJavaDirectory;
            txtUserJava.Text = JavaBinaryPath;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rbAutoJava_CheckedChanged(object sender, EventArgs e)
        {
            gAutoJava.Enabled = rbAutoJava.Checked;
        }

        private void rbUserJava_CheckedChanged(object sender, EventArgs e)
        {
            gUserJava.Enabled = rbUserJava.Checked;
        }
    }
}
