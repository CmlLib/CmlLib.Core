﻿using CmlLib.Core.Utils;

namespace CmlLibWinFormSample
{
    public partial class ChangeLog : Form
    {
        public ChangeLog()
        {
            InitializeComponent();
        }

        private Changelogs? changelogs;
        
        private async void ChangeLog_Load(object sender, EventArgs e)
        {
            btnLoad.Enabled = false;
            changelogs = await Changelogs.GetChangelogs(new HttpClient());
            listBox1.Items.AddRange(changelogs.GetAvailableVersions());
            btnLoad.Enabled = true;
        }
        
        private async void btnLoad_Click(object sender, EventArgs e)
        {
            if (changelogs == null)
            {
                MessageBox.Show("Changelogs was not loaded yet");
                return;
            }

            var version = listBox1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(version))
                return;
            
            btnLoad.Enabled = false;

            var body = await changelogs.GetChangelogHtml(version);
            webBrowser1.DocumentText = body;

            btnLoad.Enabled = true;
        }
    }
}
