using System;
using System.Threading;
using System.Windows.Forms;
using CmlLib.Launcher;
using System.Collections.Generic;

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
            // 프로그램이 켜졌을때 자바 설치되있는지 확인

            var java = new CmlLib.Utils.MJava(Minecraft.mPath + "\\runtime");
            if (!java.CheckJavaw())
            {
                var form = new Form2();
                form.Show();
                bool iscom = false;

                java.DownloadProgressChangedEvent += (s, v) => {
                    form.ChangeProgress(v.ProgressPercentage);
                };
                java.DownloadCompletedEvent += (t, w) =>
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

            Txt_Java.Text = Minecraft.mPath + "\\runtime\\bin\\javaw.exe";
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
            var th = new Thread(new ThreadStart(delegate {
                Minecraft.init(textBox1.Text);

                versions = MProfileInfo.GetProfiles();
                Invoke((MethodInvoker)delegate {
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

            Minecraft.init(textBox1.Text);
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
            var th = new Thread(new ThreadStart(delegate {
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

        private void Btn_Launch_Click(object sender, EventArgs e)
        {
            // 실행 버튼 눌렀을때
            // 실행할 버전으로 프로파일 검색하고 패치
            // 실행에 필요한 인수 설정
            // 실행

            if (Cb_Version.Text == "") return;
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;

            string nn = Cb_Version.Text;
            string jj = Txt_Java.Text;
            string xmx = Txt_Ram.Text;
            string ln = Txt_LauncherName.Text;
            string server = Txt_ServerIp.Text;

            var th = new Thread(new ThreadStart(delegate {

                MProfile profile = null;
                MProfile forgeProfile = null;

                foreach (var item in versions)
                {
                    if (item.Name == nn)
                    {
                        profile = MProfile.Parse(item);
                        break;
                    }
                }

                if (profile.IsForge)
                {
                    foreach (var item in versions)
                    {
                        if (item.Name == profile.InnerJarId)
                        {
                            forgeProfile = MProfile.Parse(item);
                            break;
                        }
                    }

                    DownloadGame(forgeProfile, true);
                }

                DownloadGame(profile, !profile.IsForge);

                MLaunch launch = new MLaunch();
                launch.BaseProfile = forgeProfile;
                launch.StartProfile = profile;
                launch.JavaPath = jj; //자바 경로같은거 설정자
                launch.LauncherName = ln;
                launch.XmxRam = xmx;
                launch.ServerIp = server;
                launch.Session = session;
                launch.CustomJavaParameter = Txt_JavaArgs.Text;

                if (Txt_ScWd.Text != "" && Txt_ScHt.Text != "")
                {
                    launch.ScreenHeight = int.Parse(Txt_ScHt.Text);
                    launch.ScreenWidth = int.Parse(Txt_ScWd.Text);
                }

                var pro = launch.GetProcess(); // 인수 생성 후 설정된 Process 객체 가져옴
                System.IO.File.WriteAllText("mcarg.txt", pro.StartInfo.Arguments); // 만들어진 인수 파일로 저장 (디버그용)
                pro.Start(); // Process 객체로 실행

                this.Invoke((MethodInvoker)delegate {
                    groupBox1.Enabled = true;
                    groupBox2.Enabled = true;
                });
            }));
            th.Start();
        }

        private void DownloadGame(MProfile profile, bool downloadResource = true)
        {
            MDownloader downloader = new MDownloader(profile);
            downloader.ChangeFileProgressEvent += Launcher_ChangeDownloadFileEvent;
            downloader.ChangeProgressEvent += Launcher_ChangeProgressEvent;
            downloader.DownloadAll(downloadResource);
        }

        private void Launcher_ChangeProgressEvent(ChangeProgressEventArgs e)
        {
            Invoke((MethodInvoker)delegate {
                Lv_Status.Text = e.FileName;
                progressBar1.Maximum = e.MaxValue;
                progressBar1.Value = e.CurrentValue;
            });
        }

        private void Launcher_ChangeDownloadFileEvent(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            Invoke((MethodInvoker)delegate {
                progressBar2.Value = e.ProgressPercentage;
            });
        }

        private void invoker(string title, int max, int min) //스레드에서 상태표시
        {
            invoker($"{title} - {min}/{max}");
        }

        private void invoker(string msg) //스레드에서 상태표시
        {
            Invoke((MethodInvoker)delegate {
                Lv_Status.Text = msg;
            });
        }
    }
}
