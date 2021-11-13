using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows.Forms;

namespace CmlLibWinFormSample
{
    public partial class GameLog : Form
    {
        public GameLog(Process process)
        {
            InitializeComponent();
            
            process.ErrorDataReceived += Process_DataReceived;
            process.OutputDataReceived += Process_DataReceived;
            output(process.StartInfo.Arguments);
        }
        
        private readonly ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();

        private void Process_DataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
                logQueue.Enqueue(e.Data);
        }
        
        private void output(string msg) => logQueue.Enqueue(msg);
        
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
