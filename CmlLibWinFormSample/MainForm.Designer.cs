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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cbFullscreen = new System.Windows.Forms.CheckBox();
            this.btnAutoRamSet = new System.Windows.Forms.Button();
            this.Txt_DockIcon = new System.Windows.Forms.TextBox();
            this.txtXms = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.Txt_DockName = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.Txt_GLauncherVersion = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.Txt_GLauncherName = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.Txt_ServerPort = new System.Windows.Forms.TextBox();
            this.TxtXmx = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.Txt_JavaArgs = new System.Windows.Forms.TextBox();
            this.Xmx_RAM = new System.Windows.Forms.Label();
            this.Txt_ScHt = new System.Windows.Forms.TextBox();
            this.Txt_ScWd = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.Txt_ServerIp = new System.Windows.Forms.TextBox();
            this.Txt_VersionType = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.Pb_Progress = new System.Windows.Forms.ProgressBar();
            this.Pb_File = new System.Windows.Forms.ProgressBar();
            this.Lv_Status = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnChangeJava = new System.Windows.Forms.Button();
            this.lbJavaPath = new System.Windows.Forms.Label();
            this.lbUsername = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnChangePath = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnLaunch = new System.Windows.Forms.Button();
            this.cbVersion = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btnGithub = new System.Windows.Forms.Button();
            this.btnWiki = new System.Windows.Forms.Button();
            this.btnChangelog = new System.Windows.Forms.Button();
            this.rbSequenceDownload = new System.Windows.Forms.RadioButton();
            this.rbParallelDownload = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cbSkipHashCheck = new System.Windows.Forms.CheckBox();
            this.cbSkipAssetsDownload = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnRefreshVersion = new System.Windows.Forms.Button();
            this.btnForgeInstall = new System.Windows.Forms.Button();
            this.btnSetLastVersion = new System.Windows.Forms.Button();
            this.btnMojangServer = new System.Windows.Forms.Button();
            this.btnOptions = new System.Windows.Forms.Button();
            this.lbLibraryVersion = new System.Windows.Forms.Label();
            this.btnSortFilter = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cbFullscreen);
            this.groupBox2.Controls.Add(this.btnAutoRamSet);
            this.groupBox2.Controls.Add(this.Txt_DockIcon);
            this.groupBox2.Controls.Add(this.txtXms);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.Txt_DockName);
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.Txt_GLauncherVersion);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.Txt_GLauncherName);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.Txt_ServerPort);
            this.groupBox2.Controls.Add(this.TxtXmx);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.Txt_JavaArgs);
            this.groupBox2.Controls.Add(this.Xmx_RAM);
            this.groupBox2.Controls.Add(this.Txt_ScHt);
            this.groupBox2.Controls.Add(this.Txt_ScWd);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.Txt_ServerIp);
            this.groupBox2.Controls.Add(this.Txt_VersionType);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(463, 15);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(440, 447);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options (Empty textbox means using default option)";
            // 
            // cbFullscreen
            // 
            this.cbFullscreen.AutoSize = true;
            this.cbFullscreen.Location = new System.Drawing.Point(152, 359);
            this.cbFullscreen.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbFullscreen.Name = "cbFullscreen";
            this.cbFullscreen.Size = new System.Drawing.Size(95, 19);
            this.cbFullscreen.TabIndex = 25;
            this.cbFullscreen.Text = "Fullscreen";
            this.cbFullscreen.UseVisualStyleBackColor = true;
            // 
            // btnAutoRamSet
            // 
            this.btnAutoRamSet.Location = new System.Drawing.Point(337, 400);
            this.btnAutoRamSet.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAutoRamSet.Name = "btnAutoRamSet";
            this.btnAutoRamSet.Size = new System.Drawing.Size(86, 29);
            this.btnAutoRamSet.TabIndex = 24;
            this.btnAutoRamSet.Text = "Auto Set";
            this.btnAutoRamSet.UseVisualStyleBackColor = true;
            this.btnAutoRamSet.Click += new System.EventHandler(this.btnAutoRamSet_Click);
            // 
            // Txt_DockIcon
            // 
            this.Txt_DockIcon.Location = new System.Drawing.Point(152, 332);
            this.Txt_DockIcon.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Txt_DockIcon.Name = "Txt_DockIcon";
            this.Txt_DockIcon.Size = new System.Drawing.Size(255, 25);
            this.Txt_DockIcon.TabIndex = 17;
            // 
            // txtXms
            // 
            this.txtXms.Location = new System.Drawing.Point(119, 382);
            this.txtXms.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtXms.Name = "txtXms";
            this.txtXms.Size = new System.Drawing.Size(207, 25);
            this.txtXms.TabIndex = 23;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(65, 336);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(84, 15);
            this.label17.TabIndex = 16;
            this.label17.Text = "DockIcon : ";
            // 
            // Txt_DockName
            // 
            this.Txt_DockName.Location = new System.Drawing.Point(152, 299);
            this.Txt_DockName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Txt_DockName.Name = "Txt_DockName";
            this.Txt_DockName.Size = new System.Drawing.Size(255, 25);
            this.Txt_DockName.TabIndex = 15;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(11, 388);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(105, 15);
            this.label21.TabIndex = 22;
            this.label21.Text = "Xms(MinMb) : ";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(54, 302);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(92, 15);
            this.label18.TabIndex = 14;
            this.label18.Text = "DockName : ";
            // 
            // Txt_GLauncherVersion
            // 
            this.Txt_GLauncherVersion.Location = new System.Drawing.Point(152, 265);
            this.Txt_GLauncherVersion.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Txt_GLauncherVersion.Name = "Txt_GLauncherVersion";
            this.Txt_GLauncherVersion.Size = new System.Drawing.Size(255, 25);
            this.Txt_GLauncherVersion.TabIndex = 13;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(7, 269);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(141, 15);
            this.label16.TabIndex = 12;
            this.label16.Text = "GLauncherVersion : ";
            // 
            // Txt_GLauncherName
            // 
            this.Txt_GLauncherName.Location = new System.Drawing.Point(152, 231);
            this.Txt_GLauncherName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Txt_GLauncherName.Name = "Txt_GLauncherName";
            this.Txt_GLauncherName.Size = new System.Drawing.Size(255, 25);
            this.Txt_GLauncherName.TabIndex = 11;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(16, 235);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(129, 15);
            this.label15.TabIndex = 10;
            this.label15.Text = "GLauncherName : ";
            // 
            // Txt_ServerPort
            // 
            this.Txt_ServerPort.Location = new System.Drawing.Point(152, 61);
            this.Txt_ServerPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Txt_ServerPort.Name = "Txt_ServerPort";
            this.Txt_ServerPort.Size = new System.Drawing.Size(255, 25);
            this.Txt_ServerPort.TabIndex = 9;
            // 
            // TxtXmx
            // 
            this.TxtXmx.Location = new System.Drawing.Point(119, 416);
            this.TxtXmx.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TxtXmx.Name = "TxtXmx";
            this.TxtXmx.Size = new System.Drawing.Size(207, 25);
            this.TxtXmx.TabIndex = 11;
            this.TxtXmx.Text = "1024";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(55, 65);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(96, 15);
            this.label14.TabIndex = 8;
            this.label14.Text = "Server Port : ";
            // 
            // Txt_JavaArgs
            // 
            this.Txt_JavaArgs.Location = new System.Drawing.Point(152, 164);
            this.Txt_JavaArgs.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Txt_JavaArgs.Name = "Txt_JavaArgs";
            this.Txt_JavaArgs.Size = new System.Drawing.Size(255, 25);
            this.Txt_JavaArgs.TabIndex = 7;
            // 
            // Xmx_RAM
            // 
            this.Xmx_RAM.AutoSize = true;
            this.Xmx_RAM.Location = new System.Drawing.Point(7, 419);
            this.Xmx_RAM.Name = "Xmx_RAM";
            this.Xmx_RAM.Size = new System.Drawing.Size(112, 15);
            this.Xmx_RAM.TabIndex = 10;
            this.Xmx_RAM.Text = "Xmx(MaxMb) : ";
            // 
            // Txt_ScHt
            // 
            this.Txt_ScHt.Location = new System.Drawing.Point(152, 130);
            this.Txt_ScHt.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Txt_ScHt.Name = "Txt_ScHt";
            this.Txt_ScHt.Size = new System.Drawing.Size(255, 25);
            this.Txt_ScHt.TabIndex = 6;
            // 
            // Txt_ScWd
            // 
            this.Txt_ScWd.Location = new System.Drawing.Point(152, 96);
            this.Txt_ScWd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Txt_ScWd.Name = "Txt_ScWd";
            this.Txt_ScWd.Size = new System.Drawing.Size(255, 25);
            this.Txt_ScWd.TabIndex = 5;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(22, 168);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(121, 15);
            this.label11.TabIndex = 4;
            this.label11.Text = "JVM Arguments : ";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(35, 134);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(114, 15);
            this.label10.TabIndex = 3;
            this.label10.Text = "Screen Height : ";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(41, 100);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(110, 15);
            this.label9.TabIndex = 2;
            this.label9.Text = "Screen Width : ";
            // 
            // Txt_ServerIp
            // 
            this.Txt_ServerIp.Location = new System.Drawing.Point(152, 28);
            this.Txt_ServerIp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Txt_ServerIp.Name = "Txt_ServerIp";
            this.Txt_ServerIp.Size = new System.Drawing.Size(255, 25);
            this.Txt_ServerIp.TabIndex = 1;
            // 
            // Txt_VersionType
            // 
            this.Txt_VersionType.Location = new System.Drawing.Point(152, 198);
            this.Txt_VersionType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Txt_VersionType.Name = "Txt_VersionType";
            this.Txt_VersionType.Size = new System.Drawing.Size(255, 25);
            this.Txt_VersionType.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(67, 31);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 15);
            this.label8.TabIndex = 0;
            this.label8.Text = "Server IP : ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(43, 201);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(102, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "VersionType : ";
            // 
            // Pb_Progress
            // 
            this.Pb_Progress.Location = new System.Drawing.Point(16, 521);
            this.Pb_Progress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Pb_Progress.Name = "Pb_Progress";
            this.Pb_Progress.Size = new System.Drawing.Size(887, 29);
            this.Pb_Progress.TabIndex = 19;
            // 
            // Pb_File
            // 
            this.Pb_File.Location = new System.Drawing.Point(16, 485);
            this.Pb_File.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Pb_File.Name = "Pb_File";
            this.Pb_File.Size = new System.Drawing.Size(887, 29);
            this.Pb_File.TabIndex = 18;
            // 
            // Lv_Status
            // 
            this.Lv_Status.AutoSize = true;
            this.Lv_Status.Location = new System.Drawing.Point(14, 466);
            this.Lv_Status.Name = "Lv_Status";
            this.Lv_Status.Size = new System.Drawing.Size(49, 15);
            this.Lv_Status.TabIndex = 17;
            this.Lv_Status.Text = "Ready";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnChangeJava);
            this.groupBox1.Controls.Add(this.lbJavaPath);
            this.groupBox1.Controls.Add(this.lbUsername);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btnChangePath);
            this.groupBox1.Controls.Add(this.txtPath);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(16, 15);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(440, 145);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CmlLib Sample Launcher";
            // 
            // btnChangeJava
            // 
            this.btnChangeJava.Location = new System.Drawing.Point(362, 112);
            this.btnChangeJava.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnChangeJava.Name = "btnChangeJava";
            this.btnChangeJava.Size = new System.Drawing.Size(66, 29);
            this.btnChangeJava.TabIndex = 21;
            this.btnChangeJava.Text = "Change";
            this.btnChangeJava.UseVisualStyleBackColor = true;
            this.btnChangeJava.Click += new System.EventHandler(this.btnChangeJava_Click);
            // 
            // lbJavaPath
            // 
            this.lbJavaPath.AutoSize = true;
            this.lbJavaPath.Location = new System.Drawing.Point(101, 115);
            this.lbJavaPath.Name = "lbJavaPath";
            this.lbJavaPath.Size = new System.Drawing.Size(113, 15);
            this.lbJavaPath.TabIndex = 20;
            this.lbJavaPath.Text = "Use default java";
            // 
            // lbUsername
            // 
            this.lbUsername.AutoSize = true;
            this.lbUsername.Location = new System.Drawing.Point(101, 84);
            this.lbUsername.Name = "lbUsername";
            this.lbUsername.Size = new System.Drawing.Size(67, 15);
            this.lbUsername.TabIndex = 18;
            this.lbUsername.Text = "test_user";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(47, 115);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 15);
            this.label6.TabIndex = 12;
            this.label6.Text = "Java : ";
            // 
            // btnChangePath
            // 
            this.btnChangePath.Location = new System.Drawing.Point(362, 45);
            this.btnChangePath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnChangePath.Name = "btnChangePath";
            this.btnChangePath.Size = new System.Drawing.Size(66, 29);
            this.btnChangePath.TabIndex = 9;
            this.btnChangePath.Text = "Change";
            this.btnChangePath.UseVisualStyleBackColor = true;
            this.btnChangePath.Click += new System.EventHandler(this.btnChangePath_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(19, 46);
            this.txtPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(335, 25);
            this.txtPath.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 15);
            this.label4.TabIndex = 7;
            this.label4.Text = "Game Path : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Account : ";
            // 
            // btnLaunch
            // 
            this.btnLaunch.Location = new System.Drawing.Point(32, 102);
            this.btnLaunch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnLaunch.Name = "btnLaunch";
            this.btnLaunch.Size = new System.Drawing.Size(377, 69);
            this.btnLaunch.TabIndex = 2;
            this.btnLaunch.Text = "Download and Launch";
            this.btnLaunch.UseVisualStyleBackColor = true;
            this.btnLaunch.Click += new System.EventHandler(this.Btn_Launch_Click);
            // 
            // cbVersion
            // 
            this.cbVersion.FormattingEnabled = true;
            this.cbVersion.Location = new System.Drawing.Point(105, 34);
            this.cbVersion.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbVersion.Name = "cbVersion";
            this.cbVersion.Size = new System.Drawing.Size(207, 23);
            this.cbVersion.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Version : ";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(485, 559);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(242, 15);
            this.label12.TabIndex = 23;
            this.label12.Text = "AlphaBs (ksi123456ab@naver.com)";
            // 
            // btnGithub
            // 
            this.btnGithub.Location = new System.Drawing.Point(725, 558);
            this.btnGithub.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnGithub.Name = "btnGithub";
            this.btnGithub.Size = new System.Drawing.Size(86, 29);
            this.btnGithub.TabIndex = 24;
            this.btnGithub.Text = "GitHub";
            this.btnGithub.UseVisualStyleBackColor = true;
            this.btnGithub.Click += new System.EventHandler(this.btnGithub_Click);
            // 
            // btnWiki
            // 
            this.btnWiki.Location = new System.Drawing.Point(817, 558);
            this.btnWiki.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnWiki.Name = "btnWiki";
            this.btnWiki.Size = new System.Drawing.Size(86, 29);
            this.btnWiki.TabIndex = 25;
            this.btnWiki.Text = "Wiki";
            this.btnWiki.UseVisualStyleBackColor = true;
            this.btnWiki.Click += new System.EventHandler(this.btnWiki_Click);
            // 
            // btnChangelog
            // 
            this.btnChangelog.Location = new System.Drawing.Point(14, 559);
            this.btnChangelog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnChangelog.Name = "btnChangelog";
            this.btnChangelog.Size = new System.Drawing.Size(145, 29);
            this.btnChangelog.TabIndex = 26;
            this.btnChangelog.Text = "GameChangelog";
            this.btnChangelog.UseVisualStyleBackColor = true;
            this.btnChangelog.Click += new System.EventHandler(this.btnChangelog_Click);
            // 
            // rbSequenceDownload
            // 
            this.rbSequenceDownload.AutoSize = true;
            this.rbSequenceDownload.Location = new System.Drawing.Point(44, 30);
            this.rbSequenceDownload.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbSequenceDownload.Name = "rbSequenceDownload";
            this.rbSequenceDownload.Size = new System.Drawing.Size(171, 19);
            this.rbSequenceDownload.TabIndex = 22;
            this.rbSequenceDownload.Text = "SequenceDownloader";
            this.rbSequenceDownload.UseVisualStyleBackColor = true;
            // 
            // rbParallelDownload
            // 
            this.rbParallelDownload.AutoSize = true;
            this.rbParallelDownload.Checked = true;
            this.rbParallelDownload.Location = new System.Drawing.Point(221, 30);
            this.rbParallelDownload.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.rbParallelDownload.Name = "rbParallelDownload";
            this.rbParallelDownload.Size = new System.Drawing.Size(193, 19);
            this.rbParallelDownload.TabIndex = 23;
            this.rbParallelDownload.TabStop = true;
            this.rbParallelDownload.Text = "AsyncParallelDownloader";
            this.rbParallelDownload.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbSkipHashCheck);
            this.groupBox3.Controls.Add(this.cbSkipAssetsDownload);
            this.groupBox3.Controls.Add(this.rbSequenceDownload);
            this.groupBox3.Controls.Add(this.rbParallelDownload);
            this.groupBox3.Location = new System.Drawing.Point(16, 168);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox3.Size = new System.Drawing.Size(440, 98);
            this.groupBox3.TabIndex = 27;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Download Options";
            // 
            // cbSkipHashCheck
            // 
            this.cbSkipHashCheck.AutoSize = true;
            this.cbSkipHashCheck.Location = new System.Drawing.Point(229, 57);
            this.cbSkipHashCheck.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbSkipHashCheck.Name = "cbSkipHashCheck";
            this.cbSkipHashCheck.Size = new System.Drawing.Size(157, 19);
            this.cbSkipHashCheck.TabIndex = 26;
            this.cbSkipHashCheck.Text = "Skip hash checking";
            this.cbSkipHashCheck.UseVisualStyleBackColor = true;
            // 
            // cbSkipAssetsDownload
            // 
            this.cbSkipAssetsDownload.AutoSize = true;
            this.cbSkipAssetsDownload.Location = new System.Drawing.Point(50, 57);
            this.cbSkipAssetsDownload.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cbSkipAssetsDownload.Name = "cbSkipAssetsDownload";
            this.cbSkipAssetsDownload.Size = new System.Drawing.Size(166, 19);
            this.cbSkipAssetsDownload.TabIndex = 25;
            this.cbSkipAssetsDownload.Text = "Skip asset download";
            this.cbSkipAssetsDownload.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnSortFilter);
            this.groupBox4.Controls.Add(this.btnRefreshVersion);
            this.groupBox4.Controls.Add(this.btnForgeInstall);
            this.groupBox4.Controls.Add(this.btnSetLastVersion);
            this.groupBox4.Controls.Add(this.cbVersion);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.btnLaunch);
            this.groupBox4.Location = new System.Drawing.Point(16, 272);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox4.Size = new System.Drawing.Size(440, 190);
            this.groupBox4.TabIndex = 28;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Launch";
            // 
            // btnRefreshVersion
            // 
            this.btnRefreshVersion.Location = new System.Drawing.Point(323, 65);
            this.btnRefreshVersion.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRefreshVersion.Name = "btnRefreshVersion";
            this.btnRefreshVersion.Size = new System.Drawing.Size(86, 29);
            this.btnRefreshVersion.TabIndex = 4;
            this.btnRefreshVersion.Text = "Refresh";
            this.btnRefreshVersion.UseVisualStyleBackColor = true;
            this.btnRefreshVersion.Click += new System.EventHandler(this.btnRefreshVersion_Click);
            // 
            // btnForgeInstall
            // 
            this.btnForgeInstall.Location = new System.Drawing.Point(32, 65);
            this.btnForgeInstall.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnForgeInstall.Name = "btnForgeInstall";
            this.btnForgeInstall.Size = new System.Drawing.Size(111, 29);
            this.btnForgeInstall.TabIndex = 3;
            this.btnForgeInstall.Text = "Install Forge";
            this.btnForgeInstall.UseVisualStyleBackColor = true;
            this.btnForgeInstall.Click += new System.EventHandler(this.btnForgeInstall_Click);
            // 
            // btnSetLastVersion
            // 
            this.btnSetLastVersion.Location = new System.Drawing.Point(323, 31);
            this.btnSetLastVersion.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSetLastVersion.Name = "btnSetLastVersion";
            this.btnSetLastVersion.Size = new System.Drawing.Size(86, 29);
            this.btnSetLastVersion.TabIndex = 2;
            this.btnSetLastVersion.Text = "Lastest\r\n";
            this.btnSetLastVersion.UseVisualStyleBackColor = true;
            this.btnSetLastVersion.Click += new System.EventHandler(this.btnSetLastVersion_Click);
            // 
            // btnMojangServer
            // 
            this.btnMojangServer.Location = new System.Drawing.Point(166, 558);
            this.btnMojangServer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnMojangServer.Name = "btnMojangServer";
            this.btnMojangServer.Size = new System.Drawing.Size(145, 29);
            this.btnMojangServer.TabIndex = 29;
            this.btnMojangServer.Text = "MojangServer";
            this.btnMojangServer.UseVisualStyleBackColor = true;
            this.btnMojangServer.Click += new System.EventHandler(this.btnMojangServer_Click);
            // 
            // btnOptions
            // 
            this.btnOptions.Location = new System.Drawing.Point(318, 559);
            this.btnOptions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(138, 29);
            this.btnOptions.TabIndex = 30;
            this.btnOptions.Text = "options.txt";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // lbLibraryVersion
            // 
            this.lbLibraryVersion.Location = new System.Drawing.Point(485, 576);
            this.lbLibraryVersion.Name = "lbLibraryVersion";
            this.lbLibraryVersion.Size = new System.Drawing.Size(234, 23);
            this.lbLibraryVersion.TabIndex = 31;
            this.lbLibraryVersion.Text = "CmlLib.Core";
            // 
            // btnSortFilter
            // 
            this.btnSortFilter.Location = new System.Drawing.Point(149, 65);
            this.btnSortFilter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSortFilter.Name = "btnSortFilter";
            this.btnSortFilter.Size = new System.Drawing.Size(163, 29);
            this.btnSortFilter.TabIndex = 5;
            this.btnSortFilter.Text = "Sort option";
            this.btnSortFilter.UseVisualStyleBackColor = true;
            this.btnSortFilter.Click += new System.EventHandler(this.btnSortFilter_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(918, 608);
            this.Controls.Add(this.lbLibraryVersion);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.btnMojangServer);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnChangelog);
            this.Controls.Add(this.btnWiki);
            this.Controls.Add(this.btnGithub);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.Pb_Progress);
            this.Controls.Add(this.Pb_File);
            this.Controls.Add(this.Lv_Status);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
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
        private System.Windows.Forms.Button btnMojangServer;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Button btnForgeInstall;
        private System.Windows.Forms.Button btnRefreshVersion;
        private System.Windows.Forms.CheckBox cbFullscreen;
        private System.Windows.Forms.CheckBox cbSkipHashCheck;
    }
}