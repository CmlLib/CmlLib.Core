using CmlLib.Core;
using CmlLib.Core.Auth;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using CmlLib.Core.VersionMetadata;
using CmlLib.Core.Installers;
using CmlLib.Core.ProcessBuilder;

namespace CmlLibWinFormSample
{
    public partial class MainForm : Form
    {
        private readonly MSession session;
        private readonly HttpClient _httpClient = new();

        public MainForm(MSession session)
        {
            this.session = session;
            InitializeComponent();
        }

        MinecraftLauncher? launcher;
        string? javaPath;

        private async void MainForm_Shown(object sender, EventArgs e)
        {
            lbLibraryVersion.Text = "CmlLib.Core " + getLibraryVersion();
            
            // Initialize launcher
            await initializeLauncher(new MinecraftPath());
        }

        private async Task initializeLauncher(MinecraftPath path)
        {
            lbUsername.Text = session.Username;
            txtPath.Text = path.BasePath;
            
            var parameters = MinecraftLauncherParameters.CreateDefault();
            parameters.MinecraftPath = path;
            parameters.HttpClient = _httpClient;

            launcher = new MinecraftLauncher(parameters);
            await refreshVersions();
        }

        private async void btnRefreshVersion_Click(object sender, EventArgs e)
        {
            await refreshVersions();
        }

        private async Task refreshVersions(string? showVersion=null)
        {
            if (launcher == null)
            {
                MessageBox.Show("Initialize the launcher first");
                return;
            }

            cbVersion.Items.Clear();
            var versions = await launcher.GetAllVersionsAsync();

            bool showVersionExist = false;
            foreach (var item in versions)
            {
                if (item.Name == showVersion)
                    showVersionExist = true;
                cbVersion.Items.Add(item.Name);
            }

            if (showVersion == null || !showVersionExist)
                btnSetLastVersion_Click(null, null);
            else
                cbVersion.Text = showVersion;
        }

        private void btnSetLastVersion_Click(object? sender, EventArgs? e)
        {
            cbVersion.Text = launcher?.Versions?.LatestReleaseName;
        }
        
        private void btnSortFilter_Click(object sender, EventArgs e)
        {
            if (launcher == null)
            {
                MessageBox.Show("Initialize the launcher first");
                return;
            }
            var form = new VersionSortOptionForm(launcher, new MVersionSortOption());
            form.ShowDialog();
        }

        // Start Game
        private async void Btn_Launch_Click(object sender, EventArgs e)
        {
            if (launcher == null)
            {
                MessageBox.Show("Initialize the launcher first");
                return;
            }
            if (session == null)
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
                var launchOption = new CmlLib.Core.ProcessBuilder.MLaunchOption()
                {
                    MaximumRamMb = int.Parse(TxtXmx.Text),
                    Session = this.session,

                    VersionType = Txt_VersionType.Text,
                    GameLauncherName = Txt_GLauncherName.Text,
                    GameLauncherVersion = Txt_GLauncherVersion.Text,

                    FullScreen = cbFullscreen.Checked,

                    ServerIp = Txt_ServerIp.Text,

                    DockName = Txt_DockName.Text,
                    DockIcon = Txt_DockIcon.Text
                };

                if (!string.IsNullOrEmpty(javaPath))
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
                    launchOption.JvmArgumentOverrides = new [] { MArgument.FromCommandLine(Txt_JavaArgs.Text) };

                //if (cbSkipAssetsDownload.Checked)
                //    launcher.GameFileCheckers.AssetFileChecker = null;
                //else if (launcher.GameFileCheckers.AssetFileChecker == null)
                //    launcher.GameFileCheckers.AssetFileChecker = new AssetChecker();
                
                // check file hash or don't check
                //if (launcher.GameFileCheckers.AssetFileChecker != null)
                //    launcher.GameFileCheckers.AssetFileChecker.CheckHash = !cbSkipHashCheck.Checked;
                //if (launcher.GameFileCheckers.ClientFileChecker != null)
                //    launcher.GameFileCheckers.ClientFileChecker.CheckHash = !cbSkipHashCheck.Checked;
                //if (launcher.GameFileCheckers.LibraryFileChecker != null)
                //    launcher.GameFileCheckers.LibraryFileChecker.CheckHash = !cbSkipHashCheck.Checked;

                var process = await launcher.CreateProcessAsync(cbVersion.Text, launchOption); // Create Arguments and Process

                // process.Start(); // Just start game, or
                StartProcess(process); // Start Process with debug options

                var gameLog = new GameLog(process);
                gameLog.Show();
            }
            catch (FormatException fex) // int.Parse exception
            {
                MessageBox.Show("Failed to create MLaunchOption\n\n" + fex);
            }
            catch (Win32Exception wex) // java exception
            {
                MessageBox.Show(wex + "\n\nIt seems your java setting has problem");
            }
            catch (Exception ex) // all exception
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                // enable ui
                setUIEnabled(true);
            }
        }
        
        private int uiThreadId = Thread.CurrentThread.ManagedThreadId;

        // Event Handler. Show download progress
        private void Launcher_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (Thread.CurrentThread.ManagedThreadId != uiThreadId)
            {
                Debug.WriteLine(e);
            }
            Pb_Progress.Maximum = 100;
            Pb_Progress.Value = e.ProgressPercentage;
        }

        private void Launcher_FileChanged(InstallerProgressChangedEventArgs e)
        {
            if (Thread.CurrentThread.ManagedThreadId != uiThreadId)
            {
                Debug.WriteLine(e);
            }
            Pb_File.Maximum = e.TotalTasks;
            Pb_File.Value = e.ProgressedTasks;
            Lv_Status.Text = $"[{e.EventType}][{e.ProgressedTasks}/{e.TotalTasks}] {e.Name}";
            //Debug.WriteLine(Lv_Status.Text);
        }

        private async void btnChangePath_Click(object sender, EventArgs e)
        {
            if (launcher == null)
            {
                MessageBox.Show("Initialize the launcher first");
                return;
            }
            var form = new PathForm(launcher.MinecraftPath);
            form.ShowDialog();
            await initializeLauncher(form.MinecraftPath);
        }

        private void btnChangeJava_Click(object sender, EventArgs e)
        {
            var form = new JavaForm(javaPath);
            form.ShowDialog();
            javaPath = form.JavaBinaryPath;
            
            if (string.IsNullOrEmpty(javaPath))
                lbJavaPath.Text = "Use default java";
            else
                lbJavaPath.Text = javaPath;
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

        private void StartProcess(Process process)
        {
            File.WriteAllText("launcher.txt", process.StartInfo.Arguments);
            
            // process options to display game log

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.StandardErrorEncoding = System.Text.Encoding.UTF8;
            process.StartInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
            process.EnableRaisingEvents = true;

            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
        }

        private void btnChangelog_Click(object sender, EventArgs e)
        {
            // Game Changelog
            var f = new ChangeLog();
            f.Show();
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            if (launcher == null)
            {
                MessageBox.Show("Initialize the launcher first");
                return;
            }
            var path = Path.Combine(launcher.MinecraftPath.BasePath, "options.txt");
            var f = new GameOptions(path);
            f.Show();
        }

        private void btnGithub_Click(object sender, EventArgs e)
        {
            start("https://github.com/AlphaBs/CmlLib.Core");
        }

        private void btnWiki_Click(object sender, EventArgs e)
        {
            start("https://github.com/AlphaBs/CmlLib.Core/wiki/");
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

        private string? getLibraryVersion()
        {
            try
            {
                return Assembly.GetAssembly(typeof(MinecraftLauncher))?.GetName().Version?.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
