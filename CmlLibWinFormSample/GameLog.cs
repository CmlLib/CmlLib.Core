using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CmlLibWinFormSample
{
    public partial class GameLog : Form
    {
        public GameLog()
        {
            InitializeComponent();
        }

        private void GameLog_Load(object sender, EventArgs e)
        {

        }

        public void AddLog(string msg)
        {
            richTextBox1.AppendText(msg + "\n");
            richTextBox1.ScrollToCaret();
        }
    }
}
