using CmlLib.Core;
using CmlLib.Core.Auth;
using System.ComponentModel;
using System.Diagnostics;
using CmlLib.Core.VersionMetadata;
using CmlLib.Core.Installers;
using CmlLib.Core.ProcessBuilder;
using CmlLib.Core.Rules;
using CmlLib.Core.Auth.Microsoft;

namespace CmlLibWinFormSample
{
    public partial class MainForm : Form
    {
        private readonly HttpClient _httpClient = new();

        public MainForm()
        {
            InitializeComponent();
        }

        CancellationTokenSource? cancellationToken;
        MinecraftLauncher? launcher;

        private async void MainForm_Shown(object sender, EventArgs e)
        {
            lbLibraryVersion.Text = "CmlLib.Core " + Util.GetLibraryVersion();
            txtExtraJVMArguments.Text = string.Join(' ', MLaunchOption.DefaultExtraJvmArguments.SelectMany(arg => arg.Values));

            var defaultSession = MSession.CreateOfflineSession("cmltester123");
            txtUsername.Text = defaultSession.Username;
            txtUUID.Text = defaultSession.UUID;
            txtAccessToken.Text = defaultSession.AccessToken;
            txtXUID.Text = defaultSession.Xuid;

            // Initialize launcher
            await initializeLauncher(new MinecraftPath());
        }

        private async Task initializeLauncher(MinecraftPath path)
        {
            txtPath.Text = path.BasePath;

            var parameters = MinecraftLauncherParameters.CreateDefault(path, _httpClient);
            launcher = new MinecraftLauncher(parameters);
            await refreshVersions();
        }

        private async void btnRefreshVersion_Click(object sender, EventArgs e)
        {
            await refreshVersions();
        }

        private async Task refreshVersions(string? showVersion = null)
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
                    Session = new MSession
                    {
                        Username = txtUsername.Text,
                        AccessToken = txtAccessToken.Text,
                        UUID = txtUUID.Text,
                        Xuid = txtXUID.Text
                    },
                    IsDemo = cbDemo.Checked,
                    FullScreen = cbFullscreen.Checked,
                    JvmArgumentOverrides = new[] { MArgument.FromCommandLine(txtJVMArgumentOverrides.Text) },
                    ExtraJvmArguments = new[] { MArgument.FromCommandLine(txtExtraJVMArguments.Text) },
                    ExtraGameArguments = new[] { MArgument.FromCommandLine(txtExtraGameArguments.Text) },
                };

                if (!cbJavaUseDefault.Checked)
                    launchOption.JavaPath = txtJava.Text;

                if (!string.IsNullOrEmpty(txtClientId.Text))
                    launchOption.ClientId = txtClientId.Text;

                if (!string.IsNullOrEmpty(txtVersionType.Text))
                    launchOption.VersionType = txtVersionType.Text;

                if (!string.IsNullOrEmpty(txtGLauncherName.Text))
                    launchOption.GameLauncherName = txtGLauncherName.Text;

                if (!string.IsNullOrEmpty(txtGLauncherVersion.Text))
                    launchOption.GameLauncherVersion = txtGLauncherVersion.Text;

                if (!string.IsNullOrEmpty(txtDockName.Text))
                    launchOption.DockName = txtDockName.Text;

                if (!string.IsNullOrEmpty(txtDockIcon.Text))
                    launchOption.DockIcon = txtDockIcon.Text;

                if (!string.IsNullOrEmpty(txtQuickPlayPath.Text))
                    launchOption.QuickPlayPath = txtQuickPlayPath.Text;

                if (!string.IsNullOrEmpty(txtQuickPlaySingleplay.Text))
                    launchOption.QuickPlaySingleplayer = txtQuickPlaySingleplay.Text;

                if (!string.IsNullOrEmpty(txtQuickPlayReamls.Text))
                    launchOption.QuickPlayRealms = txtQuickPlayReamls.Text;

                if (!string.IsNullOrEmpty(txtXmx.Text))
                    launchOption.MaximumRamMb = int.Parse(txtXmx.Text);

                if (!string.IsNullOrEmpty(txtXms.Text))
                    launchOption.MinimumRamMb = int.Parse(txtXms.Text);

                if (!string.IsNullOrEmpty(txtServerIP.Text))
                    launchOption.ServerIp = txtServerIP.Text;

                if (!string.IsNullOrEmpty(txtServerPort.Text))
                    launchOption.ServerPort = int.Parse(txtServerPort.Text);

                if (!string.IsNullOrEmpty(txtScreenWidth.Text) && !string.IsNullOrEmpty(txtScreenHeight.Text))
                {
                    launchOption.ScreenHeight = int.Parse(txtScreenHeight.Text);
                    launchOption.ScreenWidth = int.Parse(txtScreenWidth.Text);
                }

                if (!string.IsNullOrEmpty(txtFeatures.Text))
                {
                    launchOption.Features = txtFeatures.Text
                        .Split(',')
                        .Select(f => f.Trim())
                        .Where(f => !string.IsNullOrWhiteSpace(f))
                        .ToList();
                }

                cancellationToken = new CancellationTokenSource();

                var version = cbVersion.Text;
                var fileProgress = new SyncProgress<InstallerProgressChangedEventArgs>(Launcher_FileChanged);
                var byteProgress = new SyncProgress<ByteProgress>(Launcher_ProgressChanged);
                var stopwatch = new Stopwatch();

                var process = await Task.Run(async () =>
                {
                    stopwatch.Start();
                    var result = await launcher.InstallAndBuildProcessAsync(
                        version,
                        launchOption,
                        fileProgress,
                        byteProgress,
                        cancellationToken.Token);
                    stopwatch.Stop();
                    return result;
                }); // Create Arguments and Process

                lbTime.Text = stopwatch.Elapsed.ToString();
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

        ByteProgress byteProgress;
        private void Launcher_ProgressChanged(ByteProgress e)
        {
            byteProgress = e;
        }

        InstallerProgressChangedEventArgs? fileProgress;
        private void Launcher_FileChanged(InstallerProgressChangedEventArgs e)
        {
            if (e.EventType == InstallerEventType.Done)
                fileProgress = e;
        }

        private void eventTimer_Tick(object sender, EventArgs e)
        {
            var bytePercentage = (int)(byteProgress.ProgressedBytes / (double)byteProgress.TotalBytes * 100);
            if (bytePercentage >= 0 && bytePercentage <= 100)
            {
                Pb_Progress.Value = bytePercentage;
                Pb_Progress.Maximum = 100;
            }

            if (fileProgress != null)
                Lv_Status.Text = $"[{fileProgress.ProgressedTasks}/{fileProgress.TotalTasks}] {fileProgress.Name}";
        }

        private void cbJavaUseDefault_CheckedChanged(object sender, EventArgs e)
        {
            if (cbJavaUseDefault.Checked)
            {
                txtJava.ReadOnly = true;
            }
            else
            {
                txtJava.ReadOnly = false;
            }
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

            txtXmx.Text = max.ToString();
            txtXms.Text = min.ToString();
        }

        private void setUIEnabled(bool value)
        {
            groupBox1.Enabled = value;
            groupBox2.Enabled = value;
            groupBox3.Enabled = value;
            btnLaunch.Enabled = value;
            btnCancel.Enabled = !value;
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
            Util.OpenUrl("https://github.com/AlphaBs/CmlLib.Core");
        }

        private void btnWiki_Click(object sender, EventArgs e)
        {
            Util.OpenUrl("https://github.com/AlphaBs/CmlLib.Core/wiki/");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            cancellationToken?.Cancel();
        }

        JELoginHandler loginHandler = JELoginHandlerBuilder.BuildDefault();

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            setUIEnabled(false);
            try
            {
                var session = await loginHandler.Authenticate();
                txtAccessToken.Text = session.AccessToken;
                txtUsername.Text = session.Username;
                txtUUID.Text = session.UUID;
                txtXUID.Text = session.Xuid;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            setUIEnabled(true);
        }

        private async void btnLogout_Click(object sender, EventArgs e)
        {
            setUIEnabled(false);
            try
            {
                await loginHandler.SignoutWithBrowser();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            setUIEnabled(true);
        }
    }
}
