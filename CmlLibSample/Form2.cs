using System;
using System.Windows.Forms;
using CmlLib.Launcher;

namespace CmlLibSample
{
    public partial class Form2 : Form
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

        public Form2()
        {
            InitializeComponent();
        }
    }
}
