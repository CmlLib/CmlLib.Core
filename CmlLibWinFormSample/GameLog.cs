using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;

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

        static ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();

        public static void AddLog(string msg)
        {
            logQueue.Enqueue(msg);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string msg;
            while (logQueue.TryDequeue(out msg))
            {
                richTextBox1.AppendText(msg + "\n");
                richTextBox1.ScrollToCaret();
            }
        }
    }
}
