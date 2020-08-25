using System;
using System.Collections.Concurrent;
using System.Windows.Forms;

namespace CmlLibWinFormSample
{
    public partial class GameLog : Form
    {
        public GameLog()
        {
            InitializeComponent();
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
