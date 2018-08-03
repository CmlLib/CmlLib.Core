using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using CmlLib.Launcher;
using CmlLib.Utils;

namespace CmlServerLauncherSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var java = new MJava();
            if (!java.CheckJava())
            {
                var form = new Form3(java);
                form.ShowDialog();
            }

            lvMessage.Text = "자동 로그인 중...";
            panel1.Enabled = false;

            var th = new Thread(new ThreadStart(delegate {
                var login = new MLogin();
                var session = login.TryAutoLogin();
                this.Invoke((MethodInvoker)delegate {
                    if (session.Status == MLoginResult.Success)
                    {
                        showMain(session);
                    }
                    else
                    {
                        lvMessage.Text = "로그인을 해주세요";
                        panel1.Enabled = true;
                    }
                });
            }));
            th.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            lvMessage.Text = "로그인 중...";
            panel1.Enabled = false;

            var th = new Thread(new ThreadStart(delegate {
                var login = new MLogin();
                var session = login.Authenticate(textBox1.Text, textBox2.Text);
                this.Invoke((MethodInvoker)delegate {
                    if (session.Status == MLoginResult.Success)
                    {
                        showMain(session);
                    }
                    else
                    {
                        lvMessage.Text = "로그인 실패 : " + session.Status.ToString();
                        panel1.Enabled = true;
                    }
                });
            }));
        }

        void showMain(MSession session)
        {
            var main = new Form2(session);
            this.Hide();
            main.Show();
        }
    }
}
