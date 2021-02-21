using CmlLib.Utils;
using System;
using System.Windows.Forms;

namespace CmlLibWinFormSample
{
    public partial class ChangeLog : Form
    {
        public ChangeLog()
        {
            InitializeComponent();
        }

        private void ChangeLog_Load(object sender, EventArgs e)
        {
            listBox1.Items.AddRange(Changelogs.GetAvailableVersions());
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = Changelogs.GetChangelogUrl(listBox1.SelectedItem.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var html = Changelogs.GetChangelogHtml(listBox1.SelectedItem.ToString());

            webBrowser1.DocumentText = html;
            richTextBox1.Text = html;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            webBrowser1.Visible = true;
            richTextBox1.Visible = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            webBrowser1.Visible = false;
            richTextBox1.Visible = true;
        }
    }
}
