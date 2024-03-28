namespace CmlLibWinFormSample
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox2 = new GroupBox();
            cbFullscreen = new CheckBox();
            btnAutoRamSet = new Button();
            Txt_DockIcon = new TextBox();
            txtXms = new TextBox();
            label17 = new Label();
            Txt_DockName = new TextBox();
            label21 = new Label();
            label18 = new Label();
            Txt_GLauncherVersion = new TextBox();
            label16 = new Label();
            Txt_GLauncherName = new TextBox();
            label15 = new Label();
            Txt_ServerPort = new TextBox();
            TxtXmx = new TextBox();
            label14 = new Label();
            Txt_JavaArgs = new TextBox();
            Xmx_RAM = new Label();
            Txt_ScHt = new TextBox();
            Txt_ScWd = new TextBox();
            label11 = new Label();
            label10 = new Label();
            label9 = new Label();
            Txt_ServerIp = new TextBox();
            Txt_VersionType = new TextBox();
            label8 = new Label();
            label7 = new Label();
            Pb_Progress = new ProgressBar();
            Lv_Status = new Label();
            groupBox1 = new GroupBox();
            btnChangeJava = new Button();
            lbJavaPath = new Label();
            lbUsername = new Label();
            label6 = new Label();
            btnChangePath = new Button();
            txtPath = new TextBox();
            label4 = new Label();
            label2 = new Label();
            btnLaunch = new Button();
            cbVersion = new ComboBox();
            label1 = new Label();
            label12 = new Label();
            btnGithub = new Button();
            btnWiki = new Button();
            btnChangelog = new Button();
            rbSequenceDownload = new RadioButton();
            rbParallelDownload = new RadioButton();
            groupBox3 = new GroupBox();
            cbSkipHashCheck = new CheckBox();
            cbSkipAssetsDownload = new CheckBox();
            groupBox4 = new GroupBox();
            btnSortFilter = new Button();
            btnRefreshVersion = new Button();
            btnSetLastVersion = new Button();
            btnOptions = new Button();
            lbLibraryVersion = new Label();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(cbFullscreen);
            groupBox2.Controls.Add(btnAutoRamSet);
            groupBox2.Controls.Add(Txt_DockIcon);
            groupBox2.Controls.Add(txtXms);
            groupBox2.Controls.Add(label17);
            groupBox2.Controls.Add(Txt_DockName);
            groupBox2.Controls.Add(label21);
            groupBox2.Controls.Add(label18);
            groupBox2.Controls.Add(Txt_GLauncherVersion);
            groupBox2.Controls.Add(label16);
            groupBox2.Controls.Add(Txt_GLauncherName);
            groupBox2.Controls.Add(label15);
            groupBox2.Controls.Add(Txt_ServerPort);
            groupBox2.Controls.Add(TxtXmx);
            groupBox2.Controls.Add(label14);
            groupBox2.Controls.Add(Txt_JavaArgs);
            groupBox2.Controls.Add(Xmx_RAM);
            groupBox2.Controls.Add(Txt_ScHt);
            groupBox2.Controls.Add(Txt_ScWd);
            groupBox2.Controls.Add(label11);
            groupBox2.Controls.Add(label10);
            groupBox2.Controls.Add(label9);
            groupBox2.Controls.Add(Txt_ServerIp);
            groupBox2.Controls.Add(Txt_VersionType);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(label7);
            groupBox2.Location = new Point(405, 15);
            groupBox2.Margin = new Padding(3, 4, 3, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(3, 4, 3, 4);
            groupBox2.Size = new Size(385, 447);
            groupBox2.TabIndex = 20;
            groupBox2.TabStop = false;
            groupBox2.Text = "Options (Empty textbox means using default option)";
            // 
            // cbFullscreen
            // 
            cbFullscreen.AutoSize = true;
            cbFullscreen.Location = new Point(133, 359);
            cbFullscreen.Margin = new Padding(3, 4, 3, 4);
            cbFullscreen.Name = "cbFullscreen";
            cbFullscreen.Size = new Size(79, 19);
            cbFullscreen.TabIndex = 25;
            cbFullscreen.Text = "Fullscreen";
            cbFullscreen.UseVisualStyleBackColor = true;
            // 
            // btnAutoRamSet
            // 
            btnAutoRamSet.Location = new Point(295, 400);
            btnAutoRamSet.Margin = new Padding(3, 4, 3, 4);
            btnAutoRamSet.Name = "btnAutoRamSet";
            btnAutoRamSet.Size = new Size(75, 29);
            btnAutoRamSet.TabIndex = 24;
            btnAutoRamSet.Text = "Auto Set";
            btnAutoRamSet.UseVisualStyleBackColor = true;
            btnAutoRamSet.Click += btnAutoRamSet_Click;
            // 
            // Txt_DockIcon
            // 
            Txt_DockIcon.Location = new Point(133, 332);
            Txt_DockIcon.Margin = new Padding(3, 4, 3, 4);
            Txt_DockIcon.Name = "Txt_DockIcon";
            Txt_DockIcon.Size = new Size(224, 23);
            Txt_DockIcon.TabIndex = 17;
            // 
            // txtXms
            // 
            txtXms.Location = new Point(104, 382);
            txtXms.Margin = new Padding(3, 4, 3, 4);
            txtXms.Name = "txtXms";
            txtXms.Size = new Size(182, 23);
            txtXms.TabIndex = 23;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.Location = new Point(57, 336);
            label17.Name = "label17";
            label17.Size = new Size(66, 15);
            label17.TabIndex = 16;
            label17.Text = "DockIcon : ";
            // 
            // Txt_DockName
            // 
            Txt_DockName.Location = new Point(133, 299);
            Txt_DockName.Margin = new Padding(3, 4, 3, 4);
            Txt_DockName.Name = "Txt_DockName";
            Txt_DockName.Size = new Size(224, 23);
            Txt_DockName.TabIndex = 15;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new Point(10, 388);
            label21.Name = "label21";
            label21.Size = new Size(86, 15);
            label21.TabIndex = 22;
            label21.Text = "Xms(MinMb) : ";
            // 
            // label18
            // 
            label18.AutoSize = true;
            label18.Location = new Point(47, 302);
            label18.Name = "label18";
            label18.Size = new Size(75, 15);
            label18.TabIndex = 14;
            label18.Text = "DockName : ";
            // 
            // Txt_GLauncherVersion
            // 
            Txt_GLauncherVersion.Location = new Point(133, 265);
            Txt_GLauncherVersion.Margin = new Padding(3, 4, 3, 4);
            Txt_GLauncherVersion.Name = "Txt_GLauncherVersion";
            Txt_GLauncherVersion.Size = new Size(224, 23);
            Txt_GLauncherVersion.TabIndex = 13;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(6, 269);
            label16.Name = "label16";
            label16.Size = new Size(111, 15);
            label16.TabIndex = 12;
            label16.Text = "GLauncherVersion : ";
            // 
            // Txt_GLauncherName
            // 
            Txt_GLauncherName.Location = new Point(133, 231);
            Txt_GLauncherName.Margin = new Padding(3, 4, 3, 4);
            Txt_GLauncherName.Name = "Txt_GLauncherName";
            Txt_GLauncherName.Size = new Size(224, 23);
            Txt_GLauncherName.TabIndex = 11;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.Location = new Point(14, 235);
            label15.Name = "label15";
            label15.Size = new Size(105, 15);
            label15.TabIndex = 10;
            label15.Text = "GLauncherName : ";
            // 
            // Txt_ServerPort
            // 
            Txt_ServerPort.Location = new Point(133, 61);
            Txt_ServerPort.Margin = new Padding(3, 4, 3, 4);
            Txt_ServerPort.Name = "Txt_ServerPort";
            Txt_ServerPort.Size = new Size(224, 23);
            Txt_ServerPort.TabIndex = 9;
            // 
            // TxtXmx
            // 
            TxtXmx.Location = new Point(104, 416);
            TxtXmx.Margin = new Padding(3, 4, 3, 4);
            TxtXmx.Name = "TxtXmx";
            TxtXmx.Size = new Size(182, 23);
            TxtXmx.TabIndex = 11;
            TxtXmx.Text = "1024";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(48, 65);
            label14.Name = "label14";
            label14.Size = new Size(73, 15);
            label14.TabIndex = 8;
            label14.Text = "Server Port : ";
            // 
            // Txt_JavaArgs
            // 
            Txt_JavaArgs.Location = new Point(133, 164);
            Txt_JavaArgs.Margin = new Padding(3, 4, 3, 4);
            Txt_JavaArgs.Name = "Txt_JavaArgs";
            Txt_JavaArgs.Size = new Size(224, 23);
            Txt_JavaArgs.TabIndex = 7;
            // 
            // Xmx_RAM
            // 
            Xmx_RAM.AutoSize = true;
            Xmx_RAM.Location = new Point(6, 419);
            Xmx_RAM.Name = "Xmx_RAM";
            Xmx_RAM.Size = new Size(89, 15);
            Xmx_RAM.TabIndex = 10;
            Xmx_RAM.Text = "Xmx(MaxMb) : ";
            // 
            // Txt_ScHt
            // 
            Txt_ScHt.Location = new Point(133, 130);
            Txt_ScHt.Margin = new Padding(3, 4, 3, 4);
            Txt_ScHt.Name = "Txt_ScHt";
            Txt_ScHt.Size = new Size(224, 23);
            Txt_ScHt.TabIndex = 6;
            // 
            // Txt_ScWd
            // 
            Txt_ScWd.Location = new Point(133, 96);
            Txt_ScWd.Margin = new Padding(3, 4, 3, 4);
            Txt_ScWd.Name = "Txt_ScWd";
            Txt_ScWd.Size = new Size(224, 23);
            Txt_ScWd.TabIndex = 5;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(19, 168);
            label11.Name = "label11";
            label11.Size = new Size(100, 15);
            label11.TabIndex = 4;
            label11.Text = "JVM Arguments : ";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(31, 134);
            label10.Name = "label10";
            label10.Size = new Size(90, 15);
            label10.TabIndex = 3;
            label10.Text = "Screen Height : ";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(36, 100);
            label9.Name = "label9";
            label9.Size = new Size(86, 15);
            label9.TabIndex = 2;
            label9.Text = "Screen Width : ";
            // 
            // Txt_ServerIp
            // 
            Txt_ServerIp.Location = new Point(133, 28);
            Txt_ServerIp.Margin = new Padding(3, 4, 3, 4);
            Txt_ServerIp.Name = "Txt_ServerIp";
            Txt_ServerIp.Size = new Size(224, 23);
            Txt_ServerIp.TabIndex = 1;
            // 
            // Txt_VersionType
            // 
            Txt_VersionType.Location = new Point(133, 198);
            Txt_VersionType.Margin = new Padding(3, 4, 3, 4);
            Txt_VersionType.Name = "Txt_VersionType";
            Txt_VersionType.Size = new Size(224, 23);
            Txt_VersionType.TabIndex = 1;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(59, 31);
            label8.Name = "label8";
            label8.Size = new Size(61, 15);
            label8.TabIndex = 0;
            label8.Text = "Server IP : ";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(38, 201);
            label7.Name = "label7";
            label7.Size = new Size(78, 15);
            label7.TabIndex = 0;
            label7.Text = "VersionType : ";
            // 
            // Pb_Progress
            // 
            Pb_Progress.Location = new Point(14, 489);
            Pb_Progress.Margin = new Padding(3, 4, 3, 4);
            Pb_Progress.Name = "Pb_Progress";
            Pb_Progress.Size = new Size(776, 29);
            Pb_Progress.TabIndex = 19;
            // 
            // Lv_Status
            // 
            Lv_Status.AutoSize = true;
            Lv_Status.Location = new Point(12, 466);
            Lv_Status.Name = "Lv_Status";
            Lv_Status.Size = new Size(39, 15);
            Lv_Status.TabIndex = 17;
            Lv_Status.Text = "Ready";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btnChangeJava);
            groupBox1.Controls.Add(lbJavaPath);
            groupBox1.Controls.Add(lbUsername);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(btnChangePath);
            groupBox1.Controls.Add(txtPath);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(14, 15);
            groupBox1.Margin = new Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 4, 3, 4);
            groupBox1.Size = new Size(385, 145);
            groupBox1.TabIndex = 16;
            groupBox1.TabStop = false;
            groupBox1.Text = "CmlLib Sample Launcher";
            // 
            // btnChangeJava
            // 
            btnChangeJava.Location = new Point(317, 112);
            btnChangeJava.Margin = new Padding(3, 4, 3, 4);
            btnChangeJava.Name = "btnChangeJava";
            btnChangeJava.Size = new Size(58, 29);
            btnChangeJava.TabIndex = 21;
            btnChangeJava.Text = "Change";
            btnChangeJava.UseVisualStyleBackColor = true;
            btnChangeJava.Click += btnChangeJava_Click;
            // 
            // lbJavaPath
            // 
            lbJavaPath.AutoSize = true;
            lbJavaPath.Location = new Point(88, 115);
            lbJavaPath.Name = "lbJavaPath";
            lbJavaPath.Size = new Size(90, 15);
            lbJavaPath.TabIndex = 20;
            lbJavaPath.Text = "Use default java";
            // 
            // lbUsername
            // 
            lbUsername.AutoSize = true;
            lbUsername.Location = new Point(88, 84);
            lbUsername.Name = "lbUsername";
            lbUsername.Size = new Size(53, 15);
            lbUsername.TabIndex = 18;
            lbUsername.Text = "test_user";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(41, 115);
            label6.Name = "label6";
            label6.Size = new Size(38, 15);
            label6.TabIndex = 12;
            label6.Text = "Java : ";
            // 
            // btnChangePath
            // 
            btnChangePath.Location = new Point(317, 45);
            btnChangePath.Margin = new Padding(3, 4, 3, 4);
            btnChangePath.Name = "btnChangePath";
            btnChangePath.Size = new Size(58, 29);
            btnChangePath.TabIndex = 9;
            btnChangePath.Text = "Change";
            btnChangePath.UseVisualStyleBackColor = true;
            btnChangePath.Click += btnChangePath_Click;
            // 
            // txtPath
            // 
            txtPath.Location = new Point(17, 46);
            txtPath.Margin = new Padding(3, 4, 3, 4);
            txtPath.Name = "txtPath";
            txtPath.ReadOnly = true;
            txtPath.Size = new Size(294, 23);
            txtPath.TabIndex = 8;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(15, 28);
            label4.Name = "label4";
            label4.Size = new Size(74, 15);
            label4.TabIndex = 7;
            label4.Text = "Game Path : ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(21, 84);
            label2.Name = "label2";
            label2.Size = new Size(61, 15);
            label2.TabIndex = 3;
            label2.Text = "Account : ";
            // 
            // btnLaunch
            // 
            btnLaunch.Location = new Point(28, 102);
            btnLaunch.Margin = new Padding(3, 4, 3, 4);
            btnLaunch.Name = "btnLaunch";
            btnLaunch.Size = new Size(330, 69);
            btnLaunch.TabIndex = 2;
            btnLaunch.Text = "Download and Launch";
            btnLaunch.UseVisualStyleBackColor = true;
            btnLaunch.Click += Btn_Launch_Click;
            // 
            // cbVersion
            // 
            cbVersion.FormattingEnabled = true;
            cbVersion.Location = new Point(92, 34);
            cbVersion.Margin = new Padding(3, 4, 3, 4);
            cbVersion.Name = "cbVersion";
            cbVersion.Size = new Size(182, 23);
            cbVersion.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(26, 38);
            label1.Name = "label1";
            label1.Size = new Size(54, 15);
            label1.TabIndex = 0;
            label1.Text = "Version : ";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(424, 525);
            label12.Name = "label12";
            label12.Size = new Size(191, 15);
            label12.TabIndex = 23;
            label12.Text = "AlphaBs (ksi123456ab@naver.com)";
            // 
            // btnGithub
            // 
            btnGithub.Location = new Point(634, 524);
            btnGithub.Margin = new Padding(3, 4, 3, 4);
            btnGithub.Name = "btnGithub";
            btnGithub.Size = new Size(75, 29);
            btnGithub.TabIndex = 24;
            btnGithub.Text = "GitHub";
            btnGithub.UseVisualStyleBackColor = true;
            btnGithub.Click += btnGithub_Click;
            // 
            // btnWiki
            // 
            btnWiki.Location = new Point(715, 524);
            btnWiki.Margin = new Padding(3, 4, 3, 4);
            btnWiki.Name = "btnWiki";
            btnWiki.Size = new Size(75, 29);
            btnWiki.TabIndex = 25;
            btnWiki.Text = "Wiki";
            btnWiki.UseVisualStyleBackColor = true;
            btnWiki.Click += btnWiki_Click;
            // 
            // btnChangelog
            // 
            btnChangelog.Location = new Point(12, 525);
            btnChangelog.Margin = new Padding(3, 4, 3, 4);
            btnChangelog.Name = "btnChangelog";
            btnChangelog.Size = new Size(127, 29);
            btnChangelog.TabIndex = 26;
            btnChangelog.Text = "GameChangelog";
            btnChangelog.UseVisualStyleBackColor = true;
            btnChangelog.Click += btnChangelog_Click;
            // 
            // rbSequenceDownload
            // 
            rbSequenceDownload.AutoSize = true;
            rbSequenceDownload.Location = new Point(38, 30);
            rbSequenceDownload.Margin = new Padding(3, 4, 3, 4);
            rbSequenceDownload.Name = "rbSequenceDownload";
            rbSequenceDownload.Size = new Size(140, 19);
            rbSequenceDownload.TabIndex = 22;
            rbSequenceDownload.Text = "SequenceDownloader";
            rbSequenceDownload.UseVisualStyleBackColor = true;
            // 
            // rbParallelDownload
            // 
            rbParallelDownload.AutoSize = true;
            rbParallelDownload.Checked = true;
            rbParallelDownload.Location = new Point(193, 30);
            rbParallelDownload.Margin = new Padding(3, 4, 3, 4);
            rbParallelDownload.Name = "rbParallelDownload";
            rbParallelDownload.Size = new Size(159, 19);
            rbParallelDownload.TabIndex = 23;
            rbParallelDownload.TabStop = true;
            rbParallelDownload.Text = "AsyncParallelDownloader";
            rbParallelDownload.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(cbSkipHashCheck);
            groupBox3.Controls.Add(cbSkipAssetsDownload);
            groupBox3.Controls.Add(rbSequenceDownload);
            groupBox3.Controls.Add(rbParallelDownload);
            groupBox3.Location = new Point(14, 168);
            groupBox3.Margin = new Padding(3, 4, 3, 4);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(3, 4, 3, 4);
            groupBox3.Size = new Size(385, 98);
            groupBox3.TabIndex = 27;
            groupBox3.TabStop = false;
            groupBox3.Text = "Download Options";
            // 
            // cbSkipHashCheck
            // 
            cbSkipHashCheck.AutoSize = true;
            cbSkipHashCheck.Location = new Point(200, 57);
            cbSkipHashCheck.Margin = new Padding(3, 4, 3, 4);
            cbSkipHashCheck.Name = "cbSkipHashCheck";
            cbSkipHashCheck.Size = new Size(127, 19);
            cbSkipHashCheck.TabIndex = 26;
            cbSkipHashCheck.Text = "Skip hash checking";
            cbSkipHashCheck.UseVisualStyleBackColor = true;
            // 
            // cbSkipAssetsDownload
            // 
            cbSkipAssetsDownload.AutoSize = true;
            cbSkipAssetsDownload.Location = new Point(44, 57);
            cbSkipAssetsDownload.Margin = new Padding(3, 4, 3, 4);
            cbSkipAssetsDownload.Name = "cbSkipAssetsDownload";
            cbSkipAssetsDownload.Size = new Size(133, 19);
            cbSkipAssetsDownload.TabIndex = 25;
            cbSkipAssetsDownload.Text = "Skip asset download";
            cbSkipAssetsDownload.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(btnSortFilter);
            groupBox4.Controls.Add(btnRefreshVersion);
            groupBox4.Controls.Add(btnSetLastVersion);
            groupBox4.Controls.Add(cbVersion);
            groupBox4.Controls.Add(label1);
            groupBox4.Controls.Add(btnLaunch);
            groupBox4.Location = new Point(14, 272);
            groupBox4.Margin = new Padding(3, 4, 3, 4);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(3, 4, 3, 4);
            groupBox4.Size = new Size(385, 190);
            groupBox4.TabIndex = 28;
            groupBox4.TabStop = false;
            groupBox4.Text = "Launch";
            // 
            // btnSortFilter
            // 
            btnSortFilter.Location = new Point(130, 65);
            btnSortFilter.Margin = new Padding(3, 4, 3, 4);
            btnSortFilter.Name = "btnSortFilter";
            btnSortFilter.Size = new Size(143, 29);
            btnSortFilter.TabIndex = 5;
            btnSortFilter.Text = "Sort option";
            btnSortFilter.UseVisualStyleBackColor = true;
            btnSortFilter.Click += btnSortFilter_Click;
            // 
            // btnRefreshVersion
            // 
            btnRefreshVersion.Location = new Point(283, 65);
            btnRefreshVersion.Margin = new Padding(3, 4, 3, 4);
            btnRefreshVersion.Name = "btnRefreshVersion";
            btnRefreshVersion.Size = new Size(75, 29);
            btnRefreshVersion.TabIndex = 4;
            btnRefreshVersion.Text = "Refresh";
            btnRefreshVersion.UseVisualStyleBackColor = true;
            btnRefreshVersion.Click += btnRefreshVersion_Click;
            // 
            // btnSetLastVersion
            // 
            btnSetLastVersion.Location = new Point(283, 31);
            btnSetLastVersion.Margin = new Padding(3, 4, 3, 4);
            btnSetLastVersion.Name = "btnSetLastVersion";
            btnSetLastVersion.Size = new Size(75, 29);
            btnSetLastVersion.TabIndex = 2;
            btnSetLastVersion.Text = "Lastest\r\n";
            btnSetLastVersion.UseVisualStyleBackColor = true;
            btnSetLastVersion.Click += btnSetLastVersion_Click;
            // 
            // btnOptions
            // 
            btnOptions.Location = new Point(278, 525);
            btnOptions.Margin = new Padding(3, 4, 3, 4);
            btnOptions.Name = "btnOptions";
            btnOptions.Size = new Size(121, 29);
            btnOptions.TabIndex = 30;
            btnOptions.Text = "options.txt";
            btnOptions.UseVisualStyleBackColor = true;
            btnOptions.Click += btnOptions_Click;
            // 
            // lbLibraryVersion
            // 
            lbLibraryVersion.Location = new Point(424, 542);
            lbLibraryVersion.Name = "lbLibraryVersion";
            lbLibraryVersion.Size = new Size(205, 23);
            lbLibraryVersion.TabIndex = 31;
            lbLibraryVersion.Text = "CmlLib.Core";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(803, 564);
            Controls.Add(lbLibraryVersion);
            Controls.Add(btnOptions);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(btnChangelog);
            Controls.Add(btnWiki);
            Controls.Add(btnGithub);
            Controls.Add(label12);
            Controls.Add(groupBox2);
            Controls.Add(Pb_Progress);
            Controls.Add(Lv_Status);
            Controls.Add(groupBox1);
            Margin = new Padding(3, 4, 3, 4);
            Name = "MainForm";
            Text = "MainForm";
            Shown += MainForm_Shown;
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Button btnSortFilter;

        private System.Windows.Forms.Label lbLibraryVersion;

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox Txt_JavaArgs;
        private System.Windows.Forms.TextBox Txt_ScHt;
        private System.Windows.Forms.TextBox Txt_ScWd;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox Txt_ServerIp;
        private System.Windows.Forms.TextBox Txt_VersionType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ProgressBar Pb_Progress;
        private System.Windows.Forms.ProgressBar Pb_File;
        private System.Windows.Forms.Label Lv_Status;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TxtXmx;
        private System.Windows.Forms.Label Xmx_RAM;
        private System.Windows.Forms.Button btnChangePath;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLaunch;
        private System.Windows.Forms.ComboBox cbVersion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnGithub;
        private System.Windows.Forms.TextBox Txt_ServerPort;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox Txt_GLauncherVersion;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox Txt_GLauncherName;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox Txt_DockIcon;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox Txt_DockName;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button btnWiki;
        private System.Windows.Forms.Button btnChangelog;
        private System.Windows.Forms.Button btnAutoRamSet;
        private System.Windows.Forms.TextBox txtXms;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Button btnChangeJava;
        private System.Windows.Forms.Label lbJavaPath;
        private System.Windows.Forms.Label lbUsername;
        private System.Windows.Forms.RadioButton rbSequenceDownload;
        private System.Windows.Forms.RadioButton rbParallelDownload;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cbSkipAssetsDownload;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnSetLastVersion;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Button btnRefreshVersion;
        private System.Windows.Forms.CheckBox cbFullscreen;
        private System.Windows.Forms.CheckBox cbSkipHashCheck;
    }
}