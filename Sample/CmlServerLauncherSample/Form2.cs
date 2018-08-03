using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using CmlLib.Launcher;
using CmlLib.Utils;

namespace CmlServerLauncherSample
{
    public partial class Form2 : Form
    {
        MSession session;

        public Form2(MSession s)
        {
            session = s;
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            label1.Text = session.Username + " 님 환영합니다.";
            comboBox1.Items.AddRange(Info.LaunchVersions);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            var th = new Thread(new ThreadStart(Start));
            th.Start();
        }

        void Start()
        {
            c("실행 준비 중...",0,0);

            string startVersion = "";
            this.Invoke((MethodInvoker)delegate { startVersion = comboBox1.Text; });

            Minecraft.init(Info.MinecraftPath); // 게임 폴더를 설정

            var patch = new ZipPatch(Info.PatchVer, Info.PatchURL, Minecraft.path + "\\patchver.dat");
            if (patch.CheckHasUpdate())
            {
                patch.DownloadProgressChanged += (sender, e) => {
                    c("패치 중...", 100, e.ProgressPercentage);
                };
                patch.Patch();
            }

            var profileList = MProfileInfo.GetAllProfileList(); // 프로파일 리스트를 불러옴

            var download = new WrapLauncher(); // 게임에 필요한 파일들 다운로드
            download.ChangeDownloadFileEvent += Launcher_ChangeDownloadFileEvent;
            download.ChangeProgressEvent += Launcher_ChangeProgressEvent;
            download.ExceptionEvent += Launcher_ExceptionEvent;

            var launch = download.Patch(profileList, startVersion); // 실행 정보 설정
            launch.Session = session;
            launch.XmxRam = Setting.Json.XmxRam;
            launch.JavaPath = MJava.DefaultRuntimeDirectory + "\\bin\\javaw.exe";
            launch.LauncherName = Info.LauncherName;

            if (Info.ScreenWidth != 0)
                launch.ScreenWidth = Info.ScreenWidth;
            if (Info.ScreenHeight != 0)
                launch.ScreenHeight = Info.ScreenHeight;
            if (Info.ServerIp != "")
                launch.ServerIp = Info.ServerIp;

            // 실행
            launch.Start();

            Thread.Sleep(3000);
            Program.Stop();
        }

        private void Launcher_ExceptionEvent(Exception excep, string msg)
        {
            MessageBox.Show(msg);
            throw excep;
        }

        private void Launcher_ChangeProgressEvent(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            Invoke((MethodInvoker)delegate {
                progressBar2.Value = e.ProgressPercentage;
            });
        }

        private void Launcher_ChangeDownloadFileEvent(ChangeProgressEventArgs e)
        {
            var msg = $"{e.FileKind} {e.FileName} : {e.CurrentValue} / {e.MaxValue}";
            c(msg, e.MaxValue, e.CurrentValue);
        }

        void c(string msg , int max, int val)
        {
            Invoke((MethodInvoker)delegate {
                label1.Text = msg;
                progressBar1.Maximum = max;
                progressBar1.Value = val;
            });
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            Program.Stop();
        }
    }
}
