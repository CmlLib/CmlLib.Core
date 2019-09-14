using System;
using System.Threading;
using System.Windows.Forms;
using CmlLib.Launcher;
using System.Collections.Generic;
using System.Diagnostics;

namespace CmlLibSample
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show(CmlLib._Test.tstr);

            // 프로그램이 켜졌을때 자바 설치되있는지 확인

            var java = new CmlLib.Utils.MJava(Minecraft.DefaultPath + "\\runtime");
            if (!java.CheckJavaw())
            {
                var form = new Form2();
                form.Show();
                bool iscom = false;

                java.DownloadProgressChanged += (s, v) =>
                {
                    form.ChangeProgress(v.ProgressPercentage);
                };
                java.UnzipCompleted += (t, w) =>
                {
                    form.Close();
                    this.Show();
                    iscom = true;
                };

                java.DownloadJavaAsync();

                while (!iscom)
                {
                    Application.DoEvents();
                }
            }

            Txt_Java.Text = Minecraft.DefaultPath + "\\runtime\\bin\\javaw.exe";
        }

        MProfileInfo[] versions;
        MSession session;

        private void Form1_Shown(object sender, EventArgs e)
        {
            // 런처가 실행되었을때
            // Minecraft.init
            // MProfileInfo 리스트 불러옴
            // 자동 로그인 시도

            textBox1.Text = Environment.GetEnvironmentVariable("appdata") + "\\.minecraft";
            var th = new Thread(new ThreadStart(delegate
            {
                Minecraft.Initialize(textBox1.Text);

                versions = MProfileInfo.GetProfiles();
                Invoke((MethodInvoker)delegate
                {
                    foreach (var item in versions)
                    {
                        Cb_Version.Items.Add(item.Name);
                    }
                });

                var login = new MLogin();
                MSession result = login.TryAutoLogin();

                if (result.Result != MLoginResult.Success)
                    return;

                MessageBox.Show("Auto Login Success!");
                session = result;
                Invoke((MethodInvoker)delegate { Btn_Login.Enabled = false; });
            }));
            th.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Apply 버튼 눌럿을때
            // 다시 Minecraft.init 를 해줌
            // 다시 프로파일 리스트를 불러옴

            Minecraft.Initialize(textBox1.Text);
            versions = MProfileInfo.GetProfiles();
            Cb_Version.Items.Clear();
            foreach (var item in versions)
            {
                Cb_Version.Items.Add(item.Name);
            }
        }

        private void Btn_Login_Click(object sender, EventArgs e)
        {
            // 로그인 버튼 눌렀을때
            // 로그인함

            Btn_Login.Enabled = false;
            if (Txt_Pw.Text == "")
            {
                //MessageBox.Show("배포용. 복돌기능 막혀잇습니다.");
                session = MSession.GetOfflineSession(Txt_Email.Text);
                MessageBox.Show("Offline login Success : " + Txt_Email.Text);
            }
            else
            {
                var th = new Thread(new ThreadStart(delegate
                {
                    var login = new MLogin();
                    var result = login.Authenticate(Txt_Email.Text, Txt_Pw.Text);
                    if (result.Result == MLoginResult.Success)
                    {
                        MessageBox.Show("Login Success : " + result.Username);
                        session = result;
                    }
                    else
                    {
                        MessageBox.Show(result.Result.ToString() + "\n" + result.Message);
                        Invoke((MethodInvoker)delegate { Btn_Login.Enabled = true; });
                    }
                }));
                th.Start();
            }
        }

        private void Btn_Launch_Click(object sender, EventArgs e)
        {
            // 실행 버튼 눌렀을때
            // 실행할 버전으로 프로파일 검색하고 패치
            // 실행에 필요한 인수 설정
            // 실행

            if (session == null)
            {
                MessageBox.Show("Login First");
                return;
            }

            if (Cb_Version.Text == "") return;
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;

            string nn = Cb_Version.Text;
            string jj = Txt_Java.Text;
            string xmx = Txt_Ram.Text;
            string ln = Txt_LauncherName.Text;
            string server = Txt_ServerIp.Text;

            var th = new Thread(new ThreadStart(delegate
            {
                var profile = MProfile.GetProfile(versions, nn);

                DownloadGame(profile);

                MLaunchOption option = new MLaunchOption()
                {
                    StartProfile = profile,
                    JavaPath = jj,
                    LauncherName = ln,
                    MaximumRamMb = int.Parse(xmx),
                    ServerIp = server,
                    Session = session,
                    CustomJavaParameter = Txt_JavaArgs.Text
                };

                if (Txt_ScWd.Text != "" && Txt_ScHt.Text != "")
                {
                    option.ScreenHeight = int.Parse(Txt_ScHt.Text);
                    option.ScreenWidth = int.Parse(Txt_ScWd.Text);
                }

                MLaunch launch = new MLaunch(option);

                if (true)
                {
                    StartDebug(launch);
                }
                else
                {
                    var pro = launch.GetProcess(); // 인수 생성 후 설정된 Process 객체 가져옴
                    System.IO.File.WriteAllText("mcarg.txt", pro.StartInfo.Arguments); // 만들어진 인수 파일로 저장 (디버그용)
                    pro.Start(); // Process 객체로 실행
                }

                this.Invoke((MethodInvoker)delegate
                {
                    groupBox1.Enabled = true;
                    groupBox2.Enabled = true;
                });
            }));
            th.Start();
        }

        private void DownloadGame(MProfile profile, bool downloadResource = true)
        {
            MDownloader downloader = new MDownloader(profile);
            downloader.ChangeFile += Downloader_ChangeFile;
            downloader.ChangeProgress += Downloader_ChangeProgress;
            downloader.DownloadAll(downloadResource);
        }

        private void Downloader_ChangeProgress(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                progressBar2.Value = e.ProgressPercentage;
            });

        }

        private void Downloader_ChangeFile(DownloadFileChangedEventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                Lv_Status.Text = e.FileKind.ToString() + " : " + e.FileName;
                progressBar1.Maximum = e.MaxValue;
                progressBar1.Value = e.CurrentValue;
            });
        }

        private void invoker(string title, int max, int min) //스레드에서 상태표시
        {
            invoker($"{title} - {min}/{max}");
        }

        private void invoker(string msg) //스레드에서 상태표시
        {
            Invoke((MethodInvoker)delegate
            {
                Lv_Status.Text = msg;
            });
        }

        #region DEBUG PROCESS

        private void StartDebug(MLaunch launch)
        {
            Console.WriteLine("GameStartMode : Debug");

            var process = launch.GetProcess();

            Console.WriteLine("Trying Write Game Args");
            try
            {
                System.IO.File.WriteAllText("launcher.txt", process.StartInfo.Arguments);
            }
            catch
            {
                Console.WriteLine("Write Game Args Failed");
            }

            Console.WriteLine("Setting Debug Process");
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents = true;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.OutputDataReceived += Process_OutputDataReceived;

            Console.WriteLine("Start Debug Process");
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            //Dispatcher.Invoke(new Action(delegate
            //{
            //    Console.WriteLine("Start Log Window");
            //    LogWindow.Log.Show();
            //}));
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            output(e.Data);
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            output(e.Data);
        }

        void output(string msg)
        {
            msg += "\n";
            Console.WriteLine(msg);
        }

        #endregion

        private void Button2_Click(object sender, EventArgs e)
        {
            var form3 = new Form3();
            form3.Show();
        }
    }
}
