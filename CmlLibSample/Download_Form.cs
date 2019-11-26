using System;
using System.Windows.Forms;
using CmlLib.Launcher;

namespace CmlLibSample
{
    // Download Java Runtime

    public partial class Download_Form : Form
    {
        public void ChangeProgress(int p)
        {
            try
            {
                progressBar1.Value = p;
            }
            catch
            {

            }
        }

        public Download_Form()
        {
            InitializeComponent();
        }
    }
}
