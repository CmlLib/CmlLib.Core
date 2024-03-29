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
            components = new System.ComponentModel.Container();
            groupBox2 = new GroupBox();
            txtFeatures = new TextBox();
            label26 = new Label();
            txtExtraGameArguments = new TextBox();
            label25 = new Label();
            txtExtraJVMArguments = new TextBox();
            label24 = new Label();
            txtJVMArgumentOverrides = new TextBox();
            label11 = new Label();
            cbDemo = new CheckBox();
            txtClientId = new TextBox();
            label23 = new Label();
            txtQuickPlayReamls = new TextBox();
            label22 = new Label();
            txtQuickPlaySingleplay = new TextBox();
            label20 = new Label();
            txtQuickPlayPath = new TextBox();
            label19 = new Label();
            cbFullscreen = new CheckBox();
            btnAutoRamSet = new Button();
            txtDockIcon = new TextBox();
            txtXms = new TextBox();
            label17 = new Label();
            txtDockName = new TextBox();
            label21 = new Label();
            label18 = new Label();
            txtGLauncherVersion = new TextBox();
            label16 = new Label();
            txtGLauncherName = new TextBox();
            label15 = new Label();
            txtServerPort = new TextBox();
            txtXmx = new TextBox();
            label14 = new Label();
            Xmx_RAM = new Label();
            txtScreenHeight = new TextBox();
            txtScreenWidth = new TextBox();
            label10 = new Label();
            label9 = new Label();
            txtServerIP = new TextBox();
            txtVersionType = new TextBox();
            label8 = new Label();
            label7 = new Label();
            Pb_Progress = new ProgressBar();
            Lv_Status = new Label();
            groupBox1 = new GroupBox();
            cbJavaUseDefault = new CheckBox();
            txtJava = new TextBox();
            label6 = new Label();
            btnChangePath = new Button();
            txtPath = new TextBox();
            label4 = new Label();
            btnLaunch = new Button();
            cbVersion = new ComboBox();
            label1 = new Label();
            label12 = new Label();
            btnGithub = new Button();
            btnWiki = new Button();
            btnChangelog = new Button();
            groupBox4 = new GroupBox();
            btnCancel = new Button();
            btnSortFilter = new Button();
            btnRefreshVersion = new Button();
            btnSetLastVersion = new Button();
            btnOptions = new Button();
            lbLibraryVersion = new Label();
            groupBox3 = new GroupBox();
            btnLogout = new Button();
            btnLogin = new Button();
            label13 = new Label();
            label5 = new Label();
            label3 = new Label();
            label2 = new Label();
            txtXUID = new TextBox();
            txtUUID = new TextBox();
            txtAccessToken = new TextBox();
            txtUsername = new TextBox();
            lbTime = new Label();
            eventTimer = new System.Windows.Forms.Timer(components);
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox4.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(txtFeatures);
            groupBox2.Controls.Add(label26);
            groupBox2.Controls.Add(txtExtraGameArguments);
            groupBox2.Controls.Add(label25);
            groupBox2.Controls.Add(txtExtraJVMArguments);
            groupBox2.Controls.Add(label24);
            groupBox2.Controls.Add(txtJVMArgumentOverrides);
            groupBox2.Controls.Add(label11);
            groupBox2.Controls.Add(cbDemo);
            groupBox2.Controls.Add(txtClientId);
            groupBox2.Controls.Add(label23);
            groupBox2.Controls.Add(txtQuickPlayReamls);
            groupBox2.Controls.Add(label22);
            groupBox2.Controls.Add(txtQuickPlaySingleplay);
            groupBox2.Controls.Add(label20);
            groupBox2.Controls.Add(txtQuickPlayPath);
            groupBox2.Controls.Add(label19);
            groupBox2.Controls.Add(cbFullscreen);
            groupBox2.Controls.Add(btnAutoRamSet);
            groupBox2.Controls.Add(txtDockIcon);
            groupBox2.Controls.Add(txtXms);
            groupBox2.Controls.Add(label17);
            groupBox2.Controls.Add(txtDockName);
            groupBox2.Controls.Add(label21);
            groupBox2.Controls.Add(label18);
            groupBox2.Controls.Add(txtGLauncherVersion);
            groupBox2.Controls.Add(label16);
            groupBox2.Controls.Add(txtGLauncherName);
            groupBox2.Controls.Add(label15);
            groupBox2.Controls.Add(txtServerPort);
            groupBox2.Controls.Add(txtXmx);
            groupBox2.Controls.Add(label14);
            groupBox2.Controls.Add(Xmx_RAM);
            groupBox2.Controls.Add(txtScreenHeight);
            groupBox2.Controls.Add(txtScreenWidth);
            groupBox2.Controls.Add(label10);
            groupBox2.Controls.Add(label9);
            groupBox2.Controls.Add(txtServerIP);
            groupBox2.Controls.Add(txtVersionType);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(label7);
            groupBox2.Location = new Point(405, 15);
            groupBox2.Margin = new Padding(3, 4, 3, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(3, 4, 3, 4);
            groupBox2.Size = new Size(720, 447);
            groupBox2.TabIndex = 20;
            groupBox2.TabStop = false;
            groupBox2.Text = "Options (Empty textbox means using default option)";
            // 
            // txtFeatures
            // 
            txtFeatures.Location = new Point(383, 319);
            txtFeatures.Name = "txtFeatures";
            txtFeatures.Size = new Size(331, 23);
            txtFeatures.TabIndex = 42;
            // 
            // label26
            // 
            label26.AutoSize = true;
            label26.Location = new Point(383, 301);
            label26.Name = "label26";
            label26.Size = new Size(174, 15);
            label26.TabIndex = 41;
            label26.Text = "Features: (separate by commas)";
            // 
            // txtExtraGameArguments
            // 
            txtExtraGameArguments.Location = new Point(383, 275);
            txtExtraGameArguments.Name = "txtExtraGameArguments";
            txtExtraGameArguments.Size = new Size(331, 23);
            txtExtraGameArguments.TabIndex = 40;
            // 
            // label25
            // 
            label25.AutoSize = true;
            label25.Location = new Point(383, 257);
            label25.Name = "label25";
            label25.Size = new Size(126, 15);
            label25.TabIndex = 39;
            label25.Text = "ExtraGameArguments:";
            // 
            // txtExtraJVMArguments
            // 
            txtExtraJVMArguments.Location = new Point(383, 231);
            txtExtraJVMArguments.Name = "txtExtraJVMArguments";
            txtExtraJVMArguments.Size = new Size(331, 23);
            txtExtraJVMArguments.TabIndex = 38;
            // 
            // label24
            // 
            label24.AutoSize = true;
            label24.Location = new Point(383, 214);
            label24.Name = "label24";
            label24.Size = new Size(117, 15);
            label24.TabIndex = 37;
            label24.Text = "ExtraJVMArguments:";
            // 
            // txtJVMArgumentOverrides
            // 
            txtJVMArgumentOverrides.Location = new Point(383, 188);
            txtJVMArgumentOverrides.Name = "txtJVMArgumentOverrides";
            txtJVMArgumentOverrides.Size = new Size(331, 23);
            txtJVMArgumentOverrides.TabIndex = 36;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(383, 170);
            label11.Name = "label11";
            label11.Size = new Size(139, 15);
            label11.TabIndex = 35;
            label11.Text = "JVMArgumentOverrides: ";
            // 
            // cbDemo
            // 
            cbDemo.AutoSize = true;
            cbDemo.Location = new Point(383, 130);
            cbDemo.Name = "cbDemo";
            cbDemo.Size = new Size(58, 19);
            cbDemo.TabIndex = 34;
            cbDemo.Text = "Demo";
            cbDemo.UseVisualStyleBackColor = true;
            // 
            // txtClientId
            // 
            txtClientId.Location = new Point(133, 167);
            txtClientId.Margin = new Padding(3, 4, 3, 4);
            txtClientId.Name = "txtClientId";
            txtClientId.Size = new Size(224, 23);
            txtClientId.TabIndex = 33;
            // 
            // label23
            // 
            label23.AutoSize = true;
            label23.Location = new Point(62, 170);
            label23.Name = "label23";
            label23.Size = new Size(54, 15);
            label23.TabIndex = 32;
            label23.Text = "ClientId: ";
            // 
            // txtQuickPlayReamls
            // 
            txtQuickPlayReamls.Location = new Point(490, 96);
            txtQuickPlayReamls.Margin = new Padding(3, 4, 3, 4);
            txtQuickPlayReamls.Name = "txtQuickPlayReamls";
            txtQuickPlayReamls.Size = new Size(224, 23);
            txtQuickPlayReamls.TabIndex = 31;
            // 
            // label22
            // 
            label22.AutoSize = true;
            label22.Location = new Point(383, 99);
            label22.Name = "label22";
            label22.Size = new Size(101, 15);
            label22.TabIndex = 30;
            label22.Text = "QuickPlayRealms:";
            // 
            // txtQuickPlaySingleplay
            // 
            txtQuickPlaySingleplay.Location = new Point(490, 62);
            txtQuickPlaySingleplay.Margin = new Padding(3, 4, 3, 4);
            txtQuickPlaySingleplay.Name = "txtQuickPlaySingleplay";
            txtQuickPlaySingleplay.Size = new Size(224, 23);
            txtQuickPlaySingleplay.TabIndex = 29;
            // 
            // label20
            // 
            label20.AutoSize = true;
            label20.Location = new Point(364, 66);
            label20.Name = "label20";
            label20.Size = new Size(120, 15);
            label20.TabIndex = 28;
            label20.Text = "QuickPlaySingleplay: ";
            // 
            // txtQuickPlayPath
            // 
            txtQuickPlayPath.Location = new Point(490, 28);
            txtQuickPlayPath.Margin = new Padding(3, 4, 3, 4);
            txtQuickPlayPath.Name = "txtQuickPlayPath";
            txtQuickPlayPath.Size = new Size(224, 23);
            txtQuickPlayPath.TabIndex = 27;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.Location = new Point(394, 33);
            label19.Name = "label19";
            label19.Size = new Size(90, 15);
            label19.TabIndex = 26;
            label19.Text = "QuickPlayPath: ";
            // 
            // cbFullscreen
            // 
            cbFullscreen.AutoSize = true;
            cbFullscreen.Location = new Point(447, 130);
            cbFullscreen.Margin = new Padding(3, 4, 3, 4);
            cbFullscreen.Name = "cbFullscreen";
            cbFullscreen.Size = new Size(79, 19);
            cbFullscreen.TabIndex = 25;
            cbFullscreen.Text = "Fullscreen";
            cbFullscreen.UseVisualStyleBackColor = true;
            // 
            // btnAutoRamSet
            // 
            btnAutoRamSet.Location = new Point(325, 381);
            btnAutoRamSet.Margin = new Padding(3, 4, 3, 4);
            btnAutoRamSet.Name = "btnAutoRamSet";
            btnAutoRamSet.Size = new Size(75, 29);
            btnAutoRamSet.TabIndex = 24;
            btnAutoRamSet.Text = "Auto Set";
            btnAutoRamSet.UseVisualStyleBackColor = true;
            btnAutoRamSet.Click += btnAutoRamSet_Click;
            // 
            // txtDockIcon
            // 
            txtDockIcon.Location = new Point(133, 332);
            txtDockIcon.Margin = new Padding(3, 4, 3, 4);
            txtDockIcon.Name = "txtDockIcon";
            txtDockIcon.Size = new Size(224, 23);
            txtDockIcon.TabIndex = 17;
            // 
            // txtXms
            // 
            txtXms.Location = new Point(134, 363);
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
            // txtDockName
            // 
            txtDockName.Location = new Point(133, 299);
            txtDockName.Margin = new Padding(3, 4, 3, 4);
            txtDockName.Name = "txtDockName";
            txtDockName.Size = new Size(224, 23);
            txtDockName.TabIndex = 15;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.Location = new Point(40, 369);
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
            // txtGLauncherVersion
            // 
            txtGLauncherVersion.Location = new Point(133, 265);
            txtGLauncherVersion.Margin = new Padding(3, 4, 3, 4);
            txtGLauncherVersion.Name = "txtGLauncherVersion";
            txtGLauncherVersion.Size = new Size(224, 23);
            txtGLauncherVersion.TabIndex = 13;
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
            // txtGLauncherName
            // 
            txtGLauncherName.Location = new Point(133, 231);
            txtGLauncherName.Margin = new Padding(3, 4, 3, 4);
            txtGLauncherName.Name = "txtGLauncherName";
            txtGLauncherName.Size = new Size(224, 23);
            txtGLauncherName.TabIndex = 11;
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
            // txtServerPort
            // 
            txtServerPort.Location = new Point(133, 61);
            txtServerPort.Margin = new Padding(3, 4, 3, 4);
            txtServerPort.Name = "txtServerPort";
            txtServerPort.Size = new Size(224, 23);
            txtServerPort.TabIndex = 9;
            // 
            // txtXmx
            // 
            txtXmx.Location = new Point(134, 397);
            txtXmx.Margin = new Padding(3, 4, 3, 4);
            txtXmx.Name = "txtXmx";
            txtXmx.Size = new Size(182, 23);
            txtXmx.TabIndex = 11;
            txtXmx.Text = "1024";
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
            // Xmx_RAM
            // 
            Xmx_RAM.AutoSize = true;
            Xmx_RAM.Location = new Point(36, 400);
            Xmx_RAM.Name = "Xmx_RAM";
            Xmx_RAM.Size = new Size(89, 15);
            Xmx_RAM.TabIndex = 10;
            Xmx_RAM.Text = "Xmx(MaxMb) : ";
            // 
            // txtScreenHeight
            // 
            txtScreenHeight.Location = new Point(133, 130);
            txtScreenHeight.Margin = new Padding(3, 4, 3, 4);
            txtScreenHeight.Name = "txtScreenHeight";
            txtScreenHeight.Size = new Size(224, 23);
            txtScreenHeight.TabIndex = 6;
            // 
            // txtScreenWidth
            // 
            txtScreenWidth.Location = new Point(133, 96);
            txtScreenWidth.Margin = new Padding(3, 4, 3, 4);
            txtScreenWidth.Name = "txtScreenWidth";
            txtScreenWidth.Size = new Size(224, 23);
            txtScreenWidth.TabIndex = 5;
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
            // txtServerIP
            // 
            txtServerIP.Location = new Point(133, 28);
            txtServerIP.Margin = new Padding(3, 4, 3, 4);
            txtServerIP.Name = "txtServerIP";
            txtServerIP.Size = new Size(224, 23);
            txtServerIP.TabIndex = 1;
            // 
            // txtVersionType
            // 
            txtVersionType.Location = new Point(133, 198);
            txtVersionType.Margin = new Padding(3, 4, 3, 4);
            txtVersionType.Name = "txtVersionType";
            txtVersionType.Size = new Size(224, 23);
            txtVersionType.TabIndex = 1;
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
            Pb_Progress.Size = new Size(1105, 29);
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
            groupBox1.Controls.Add(cbJavaUseDefault);
            groupBox1.Controls.Add(txtJava);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(btnChangePath);
            groupBox1.Controls.Add(txtPath);
            groupBox1.Controls.Add(label4);
            groupBox1.Location = new Point(14, 15);
            groupBox1.Margin = new Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 4, 3, 4);
            groupBox1.Size = new Size(385, 99);
            groupBox1.TabIndex = 16;
            groupBox1.TabStop = false;
            groupBox1.Text = "CmlLib Sample Launcher";
            // 
            // cbJavaUseDefault
            // 
            cbJavaUseDefault.AutoSize = true;
            cbJavaUseDefault.Checked = true;
            cbJavaUseDefault.CheckState = CheckState.Checked;
            cbJavaUseDefault.Location = new Point(290, 60);
            cbJavaUseDefault.Name = "cbJavaUseDefault";
            cbJavaUseDefault.Size = new Size(85, 19);
            cbJavaUseDefault.TabIndex = 14;
            cbJavaUseDefault.Text = "Use default";
            cbJavaUseDefault.UseVisualStyleBackColor = true;
            cbJavaUseDefault.CheckedChanged += cbJavaUseDefault_CheckedChanged;
            // 
            // txtJava
            // 
            txtJava.Location = new Point(52, 58);
            txtJava.Margin = new Padding(3, 4, 3, 4);
            txtJava.Name = "txtJava";
            txtJava.ReadOnly = true;
            txtJava.Size = new Size(226, 23);
            txtJava.TabIndex = 13;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(15, 61);
            label6.Name = "label6";
            label6.Size = new Size(38, 15);
            label6.TabIndex = 12;
            label6.Text = "Java : ";
            // 
            // btnChangePath
            // 
            btnChangePath.Location = new Point(317, 20);
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
            txtPath.Location = new Point(85, 25);
            txtPath.Margin = new Padding(3, 4, 3, 4);
            txtPath.Name = "txtPath";
            txtPath.ReadOnly = true;
            txtPath.Size = new Size(226, 23);
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
            // btnLaunch
            // 
            btnLaunch.Location = new Point(28, 102);
            btnLaunch.Margin = new Padding(3, 4, 3, 4);
            btnLaunch.Name = "btnLaunch";
            btnLaunch.Size = new Size(250, 69);
            btnLaunch.TabIndex = 2;
            btnLaunch.Text = "Install and Launch";
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
            label12.Location = new Point(756, 525);
            label12.Name = "label12";
            label12.Size = new Size(191, 15);
            label12.TabIndex = 23;
            label12.Text = "AlphaBs (ksi123456ab@naver.com)";
            // 
            // btnGithub
            // 
            btnGithub.Location = new Point(966, 524);
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
            btnWiki.Location = new Point(1047, 524);
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
            // groupBox4
            // 
            groupBox4.Controls.Add(btnCancel);
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
            // btnCancel
            // 
            btnCancel.Location = new Point(284, 104);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 67);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnSortFilter
            // 
            btnSortFilter.Location = new Point(92, 65);
            btnSortFilter.Margin = new Padding(3, 4, 3, 4);
            btnSortFilter.Name = "btnSortFilter";
            btnSortFilter.Size = new Size(182, 29);
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
            btnOptions.Location = new Point(145, 526);
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
            lbLibraryVersion.Location = new Point(756, 542);
            lbLibraryVersion.Name = "lbLibraryVersion";
            lbLibraryVersion.Size = new Size(205, 23);
            lbLibraryVersion.TabIndex = 31;
            lbLibraryVersion.Text = "CmlLib.Core";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btnLogout);
            groupBox3.Controls.Add(btnLogin);
            groupBox3.Controls.Add(label13);
            groupBox3.Controls.Add(label5);
            groupBox3.Controls.Add(label3);
            groupBox3.Controls.Add(label2);
            groupBox3.Controls.Add(txtXUID);
            groupBox3.Controls.Add(txtUUID);
            groupBox3.Controls.Add(txtAccessToken);
            groupBox3.Controls.Add(txtUsername);
            groupBox3.Location = new Point(14, 121);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(385, 144);
            groupBox3.TabIndex = 32;
            groupBox3.TabStop = false;
            groupBox3.Text = "groupBox3";
            // 
            // btnLogout
            // 
            btnLogout.Location = new Point(317, 82);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(58, 48);
            btnLogout.TabIndex = 9;
            btnLogout.Text = "Logout";
            btnLogout.UseVisualStyleBackColor = true;
            btnLogout.Click += btnLogout_Click;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(317, 28);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(58, 48);
            btnLogin.TabIndex = 8;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(52, 114);
            label13.Name = "label13";
            label13.Size = new Size(33, 15);
            label13.TabIndex = 7;
            label13.Text = "XUID";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(52, 85);
            label5.Name = "label5";
            label5.Size = new Size(34, 15);
            label5.TabIndex = 6;
            label5.Text = "UUID";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(15, 56);
            label3.Name = "label3";
            label3.Size = new Size(74, 15);
            label3.TabIndex = 5;
            label3.Text = "AccessToken";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(31, 28);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 4;
            label2.Text = "Username";
            // 
            // txtXUID
            // 
            txtXUID.Location = new Point(92, 111);
            txtXUID.Name = "txtXUID";
            txtXUID.Size = new Size(219, 23);
            txtXUID.TabIndex = 3;
            // 
            // txtUUID
            // 
            txtUUID.Location = new Point(92, 82);
            txtUUID.Name = "txtUUID";
            txtUUID.Size = new Size(219, 23);
            txtUUID.TabIndex = 2;
            // 
            // txtAccessToken
            // 
            txtAccessToken.Location = new Point(92, 53);
            txtAccessToken.Name = "txtAccessToken";
            txtAccessToken.Size = new Size(219, 23);
            txtAccessToken.TabIndex = 1;
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(92, 24);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(219, 23);
            txtUsername.TabIndex = 0;
            // 
            // lbTime
            // 
            lbTime.AutoSize = true;
            lbTime.Location = new Point(1002, 470);
            lbTime.Name = "lbTime";
            lbTime.Size = new Size(12, 15);
            lbTime.TabIndex = 33;
            lbTime.Text = "?";
            // 
            // eventTimer
            // 
            eventTimer.Enabled = true;
            eventTimer.Tick += eventTimer_Tick;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1137, 564);
            Controls.Add(lbTime);
            Controls.Add(groupBox3);
            Controls.Add(lbLibraryVersion);
            Controls.Add(btnOptions);
            Controls.Add(groupBox4);
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
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Button btnSortFilter;

        private System.Windows.Forms.Label lbLibraryVersion;

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtScreenHeight;
        private System.Windows.Forms.TextBox txtScreenWidth;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtServerIP;
        private System.Windows.Forms.TextBox txtVersionType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ProgressBar Pb_Progress;
        private System.Windows.Forms.ProgressBar Pb_File;
        private System.Windows.Forms.Label Lv_Status;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtXmx;
        private System.Windows.Forms.Label Xmx_RAM;
        private System.Windows.Forms.Button btnChangePath;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnLaunch;
        private System.Windows.Forms.ComboBox cbVersion;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnGithub;
        private System.Windows.Forms.TextBox txtServerPort;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtGLauncherVersion;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtGLauncherName;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtDockIcon;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtDockName;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Button btnWiki;
        private System.Windows.Forms.Button btnChangelog;
        private System.Windows.Forms.Button btnAutoRamSet;
        private System.Windows.Forms.TextBox txtXms;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnSetLastVersion;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Button btnRefreshVersion;
        private System.Windows.Forms.CheckBox cbFullscreen;
        private GroupBox groupBox3;
        private Button btnLogout;
        private Button btnLogin;
        private Label label13;
        private Label label5;
        private Label label3;
        private Label label2;
        private TextBox txtXUID;
        private TextBox txtUUID;
        private TextBox txtAccessToken;
        private TextBox txtUsername;
        private CheckBox cbJavaUseDefault;
        private TextBox txtJava;
        private TextBox txtQuickPlayPath;
        private Label label19;
        private TextBox txtQuickPlaySingleplay;
        private Label label20;
        private TextBox txtQuickPlayReamls;
        private Label label22;
        private TextBox txtClientId;
        private Label label23;
        private CheckBox cbDemo;
        private Label label11;
        private TextBox txtJVMArgumentOverrides;
        private TextBox txtExtraJVMArguments;
        private Label label24;
        private TextBox txtExtraGameArguments;
        private Label label25;
        private TextBox txtFeatures;
        private Label label26;
        private Button btnCancel;
        private Label lbTime;
        private System.Windows.Forms.Timer eventTimer;
    }
}