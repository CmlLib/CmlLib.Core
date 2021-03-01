using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.Downloader;
using CmlLib.Core.Version;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CmlLibWinFormSample
{
    public partial class MainForm : Form
    {
        public MainForm(MSession session)
        {
            this.Session = session;
            InitializeComponent();
        }

        CMLauncher launcher;
        MSession Session;
        MinecraftPath GamePath;

        bool useMJava = true;
        string javaPath = "java.exe";
        
        GameLog logForm;

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // Initialize launcher
            this.Refresh();

            var defaultPath = new MinecraftPath(MinecraftPath.GetOSDefaultPath());
            InitializeLauncher(defaultPath);
        }

        private void InitializeLauncher(MinecraftPath path)
        {
            txtPath.Text = path.BasePath;
            this.GamePath = path;

            if (useMJava)
                lbJavaPath.Text = path.Runtime;

            launcher = new CMLauncher(path);
            launcher.FileChanged += Launcher_FileChanged;
            launcher.ProgressChanged += Launcher_ProgressChanged;
            refreshVersions(null);
        }

        private void btnRefreshVersion_Click(object sender, EventArgs e)
        {
            refreshVersions(null);
        }

        private void refreshVersions(string showVersion)
        {
            cbVersion.Items.Clear();

            var th = new Thread(new ThreadStart(delegate
            {
                var versions = launcher.UpdateVersions();
                BeginInvoke(new Action(() =>
                {
                    bool showVersionExist = false;
                    foreach (var item in versions)
                    {
                        if (showVersion != null && item.Name == showVersion)
                            showVersionExist = true;
                        cbVersion.Items.Add(item.Name);
                    }

                    if (showVersion == null || !showVersionExist)
                        btnSetLastVersion_Click(null, null);
                    else
                        cbVersion.Text = showVersion;
                }));
            }));
            th.Start();
        }

        private void btnSetLastVersion_Click(object sender, EventArgs e)
        {
            cbVersion.Text = launcher.Versions.LatestReleaseVersion?.Name;
        }

        // Start Game
        private async void Btn_Launch_Click(object sender, EventArgs e)
        {
            if (Session == null)
            {
                MessageBox.Show("Login First");
                return;
            }

            if (cbVersion.Text == "")
            {
                MessageBox.Show("Select Version");
                return;
            }

            // disable ui
            setUIEnabled(false);

            try
            {
                // create LaunchOption
                var launchOption = new MLaunchOption()
                {
                    MaximumRamMb = int.Parse(TxtXmx.Text),
                    Session = this.Session,

                    VersionType = Txt_VersionType.Text,
                    GameLauncherName = Txt_GLauncherName.Text,
                    GameLauncherVersion = Txt_GLauncherVersion.Text,

                    FullScreen = cbFullscreen.Checked,

                    ServerIp = Txt_ServerIp.Text,

                    DockName = Txt_DockName.Text,
                    DockIcon = Txt_DockIcon.Text
                };

                if (!useMJava)
                    launchOption.JavaPath = javaPath;

                if (!string.IsNullOrEmpty(txtXms.Text))
                    launchOption.MinimumRamMb = int.Parse(txtXms.Text);

                if (!string.IsNullOrEmpty(Txt_ServerPort.Text))
                    launchOption.ServerPort = int.Parse(Txt_ServerPort.Text);

                if (!string.IsNullOrEmpty(Txt_ScWd.Text) && !string.IsNullOrEmpty(Txt_ScHt.Text))
                {
                    launchOption.ScreenHeight = int.Parse(Txt_ScHt.Text);
                    launchOption.ScreenWidth = int.Parse(Txt_ScWd.Text);
                }

                if (!string.IsNullOrEmpty(Txt_JavaArgs.Text))
                    launchOption.JVMArguments = Txt_JavaArgs.Text.Split(' ');

                if (useMJava) // Download Java
                {
                    var mjava = new MJava(GamePath.Runtime);
                    mjava.ProgressChanged += Launcher_ProgressChanged;

                    var javapath = mjava.CheckJava();
                    launchOption.JavaPath = javapath;
                }

                if (rbParallelDownload.Checked)
                    launcher.FileDownloader = new AsyncParallelDownloader();
                else
                    launcher.FileDownloader = new SequenceDownloader();

                if (cbSkipAssetsDownload.Checked)
                    launcher.GameFileCheckers.AssetFileChecker = null;

                var process = await launcher.CreateProcessAsync(cbVersion.Text, launchOption); // Create Arguments and Process
                StartProcess(process); // Start Process with debug options

                // or just start process
                // process.Start();
            }
            catch (FormatException fex)
            {
                MessageBox.Show("Failed to create MLaunchOption\n\n" + fex.ToString());
                return;
            }
            catch (MDownloadFileException mex) // download exception
            {
                MessageBox.Show(
                    $"FileName : {mex.ExceptionFile.Name}\n" +
                    $"FilePath : {mex.ExceptionFile.Path}\n" +
                    $"FileUrl : {mex.ExceptionFile.Url}\n" +
                    $"FileType : {mex.ExceptionFile.Type}\n\n" +
                    mex.ToString());
            }
            catch (Win32Exception wex) // java exception
            {
                MessageBox.Show(wex.ToString() + "\n\nIt seems your java setting has problem");
            }
            catch (Exception ex) // all exception
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                // re open log form
                if (logForm != null)
                    logForm.Close();

                logForm = new GameLog();
                logForm.Show();

                // enable ui
                setUIEnabled(true);
            }
        }

        // Event Handler. Show download progress
        private void Launcher_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Pb_Progress.Value = e.ProgressPercentage;
        }

        private void Launcher_FileChanged(DownloadFileChangedEventArgs e)
        {
            Pb_File.Maximum = e.TotalFileCount;
            Pb_File.Value = e.ProgressedFileCount;
            Lv_Status.Text = $"{e.FileKind.ToString()} : {e.FileName} ({e.ProgressedFileCount}/{e.TotalFileCount})";
        }

        private void btnChangePath_Click(object sender, EventArgs e)
        {
            var form = new PathForm(this.GamePath);
            form.ShowDialog();
            InitializeLauncher(form.MinecraftPath);
        }

        private void btnChangeJava_Click(object sender, EventArgs e)
        {
            var form = new JavaForm(useMJava, this.GamePath.Runtime, javaPath);
            form.ShowDialog();

            useMJava = form.UseMJava;
            this.GamePath.Runtime = form.MJavaDirectory;
            javaPath = form.JavaBinaryPath;

            if (useMJava)
                lbJavaPath.Text = form.MJavaDirectory;
            else
                lbJavaPath.Text = form.JavaBinaryPath;
        }

        private void btnAutoRamSet_Click(object sender, EventArgs e)
        {
            var computerMemory = Util.GetMemoryMb();
            if (computerMemory == null)
            {
                MessageBox.Show("Failed to get computer memory");
                return;
            }

            var max = computerMemory / 2;
            if (max < 1024)
                max = 1024;
            else if (max > 8192)
                max = 8192;

            var min = max / 10;

            TxtXmx.Text = max.ToString();
            txtXms.Text = min.ToString();
        }

        private void setUIEnabled(bool value)
        {
            groupBox1.Enabled = value;
            groupBox2.Enabled = value;
            groupBox3.Enabled = value;
            groupBox4.Enabled = value;
        }

        // not stable
        private void btnForgeInstall_Click(object sender, EventArgs e)
        {
            setUIEnabled(false);
            new Thread(() =>
            {
                var forgeJava = "";

                if (useMJava)
                {
                    var java = new MJava();
                    java.ProgressChanged += Launcher_ProgressChanged;
                    forgeJava = java.CheckJava();
                }
                else
                    forgeJava = javaPath;

                Invoke(new Action(() =>
                {
                    var forgeForm = new ForgeInstall(GamePath, forgeJava);
                    forgeForm.ShowDialog();
                    setUIEnabled(true);
                    refreshVersions(forgeForm.LastInstalledVersion);
                }));
            }).Start();
        }

        private void StartProcess(Process process)
        {
            File.WriteAllText("launcher.txt", process.StartInfo.Arguments);
            output(process.StartInfo.Arguments);

            // process options to display game log

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.EnableRaisingEvents = true;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.OutputDataReceived += Process_OutputDataReceived;

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
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
            GameLog.AddLog(msg);
        }

        private void btnChangelog_Click(object sender, EventArgs e)
        {
            // Game Changelog
            var f = new ChangeLog();
            f.Show();
        }

        private void btnMojangServer_Click(object sender, EventArgs e)
        {
            // Mojang Server
            var f = new MojangServerForm();
            f.Show();
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            // options.txt
            var path = System.IO.Path.Combine(GamePath.BasePath, "options.txt");
            var f = new GameOptions(path);
            f.Show();
        }

        private void btnGithub_Click(object sender, EventArgs e)
        {
            start("https://github.com/AlphaBs/CmlLib.Core");
        }

        private void btnWiki_Click(object sender, EventArgs e)
        {
            start("https://github.com/AlphaBs/CmlLib.Core/wiki/MLaunchOption");
        }

        private void start(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
