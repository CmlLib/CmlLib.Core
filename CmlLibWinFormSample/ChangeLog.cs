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

        private Changelogs changelogs;
        
        private async void ChangeLog_Load(object sender, EventArgs e)
        {
            btnLoad.Enabled = false;
            changelogs = await Changelogs.GetChangelogs();
            listBox1.Items.AddRange(changelogs.GetAvailableVersions());
            btnLoad.Enabled = true;
        }
        
        private async void btnLoad_Click(object sender, EventArgs e)
        {
            btnLoad.Enabled = false;
            
            var version = listBox1.SelectedItem.ToString();
            var body = await changelogs.GetChangelogHtml(version);
            webBrowser1.DocumentText = body;

            btnLoad.Enabled = true;
        }
    }
}
