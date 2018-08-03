using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CmlLib.Utils;

namespace CmlServerLauncherSample
{
    public partial class Form3 : Form
    {
        public Form3(MJava j)
        {
            java = j;
            InitializeComponent();
        }

        MJava java;
        bool isExit = true;

        private void Form3_Shown(object sender, EventArgs e)
        {
            java.DownloadProgressChangedEvent += Java_DownloadProgressChangedEvent;
            java.DownloadCompletedEvent += Java_DownloadCompletedEvent;
            java.DownloadJavaAsync();
        }

        private void Java_DownloadCompletedEvent(object sender, EventArgs e)
        {
            isExit = false;
            this.Close();
        }

        private void Java_DownloadProgressChangedEvent(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isExit)
                Program.Stop();
        }
    }
}
