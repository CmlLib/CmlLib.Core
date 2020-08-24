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
            this.btnLogin = new System.Windows.Forms.Button();
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
            this.cbSkipAssetsDownload = new System.Windows.Forms.CheckBox();
            this.cbCheckFileHash = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnForgeInstall = new System.Windows.Forms.Button();
            this.btnSetLastVersion = new System.Windows.Forms.Button();
            this.btnMojangServer = new System.Windows.Forms.Button();
            this.btnOptions = new System.Windows.Forms.Button();
            this.btnRefreshVersion = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
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
            this.groupBox2.Location = new System.Drawing.Point(405, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(385, 368);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options (Empty textbox means using default option)";
            // 
            // btnAutoRamSet
            // 
            this.btnAutoRamSet.Location = new System.Drawing.Point(295, 316);
            this.btnAutoRamSet.Name = "btnAutoRamSet";
            this.btnAutoRamSet.Size = new System.Drawing.Size(75, 23);
            this.btnAutoRamSet.TabIndex = 24;
            this.btnAutoRamSet.Text = "Auto Set";
            this.btnAutoRamSet.UseVisualStyleBackColor = true;
            this.btnAutoRamSet.Click += new System.EventHandler(this.btnAutoRamSet_Click);
            // 
            // Txt_DockIcon
            // 
            this.Txt_DockIcon.Location = new System.Drawing.Point(133, 266);
            this.Txt_DockIcon.Name = "Txt_DockIcon";
            this.Txt_DockIcon.Size = new System.Drawing.Size(224, 21);
            this.Txt_DockIcon.TabIndex = 17;
            // 
            // txtXms
            // 
            this.txtXms.Location = new System.Drawing.Point(104, 301);
            this.txtXms.Name = "txtXms";
            this.txtXms.Size = new System.Drawing.Size(182, 21);
            this.txtXms.TabIndex = 23;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(57, 269);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(69, 12);
            this.label17.TabIndex = 16;
            this.label17.Text = "DockIcon : ";
            // 
            // Txt_DockName
            // 
            this.Txt_DockName.Location = new System.Drawing.Point(133, 239);
            this.Txt_DockName.Name = "Txt_DockName";
            this.Txt_DockName.Size = new System.Drawing.Size(224, 21);
            this.Txt_DockName.TabIndex = 15;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(10, 306);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(92, 12);
            this.label21.TabIndex = 22;
            this.label21.Text = "Xms(MinMb) : ";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(47, 242);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(79, 12);
            this.label18.TabIndex = 14;
            this.label18.Text = "DockName : ";
            // 
            // Txt_GLauncherVersion
            // 
            this.Txt_GLauncherVersion.Location = new System.Drawing.Point(133, 212);
            this.Txt_GLauncherVersion.Name = "Txt_GLauncherVersion";
            this.Txt_GLauncherVersion.Size = new System.Drawing.Size(224, 21);
            this.Txt_GLauncherVersion.TabIndex = 13;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 215);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(122, 12);
            this.label16.TabIndex = 12;
            this.label16.Text = "GLauncherVersion : ";
            // 
            // Txt_GLauncherName
            // 
            this.Txt_GLauncherName.Location = new System.Drawing.Point(133, 185);
            this.Txt_GLauncherName.Name = "Txt_GLauncherName";
            this.Txt_GLauncherName.Size = new System.Drawing.Size(224, 21);
            this.Txt_GLauncherName.TabIndex = 11;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(14, 188);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(113, 12);
            this.label15.TabIndex = 10;
            this.label15.Text = "GLauncherName : ";
            // 
            // Txt_ServerPort
            // 
            this.Txt_ServerPort.Location = new System.Drawing.Point(133, 49);
            this.Txt_ServerPort.Name = "Txt_ServerPort";
            this.Txt_ServerPort.Size = new System.Drawing.Size(224, 21);
            this.Txt_ServerPort.TabIndex = 9;
            // 
            // TxtXmx
            // 
            this.TxtXmx.Location = new System.Drawing.Point(104, 328);
            this.TxtXmx.Name = "TxtXmx";
            this.TxtXmx.Size = new System.Drawing.Size(182, 21);
            this.TxtXmx.TabIndex = 11;
            this.TxtXmx.Text = "1024";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(48, 52);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(79, 12);
            this.label14.TabIndex = 8;
            this.label14.Text = "Server Port : ";
            // 
            // Txt_JavaArgs
            // 
            this.Txt_JavaArgs.Location = new System.Drawing.Point(133, 131);
            this.Txt_JavaArgs.Name = "Txt_JavaArgs";
            this.Txt_JavaArgs.Size = new System.Drawing.Size(224, 21);
            this.Txt_JavaArgs.TabIndex = 7;
            // 
            // Xmx_RAM
            // 
            this.Xmx_RAM.AutoSize = true;
            this.Xmx_RAM.Location = new System.Drawing.Point(6, 331);
            this.Xmx_RAM.Name = "Xmx_RAM";
            this.Xmx_RAM.Size = new System.Drawing.Size(96, 12);
            this.Xmx_RAM.TabIndex = 10;
            this.Xmx_RAM.Text = "Xmx(MaxMb) : ";
            // 
            // Txt_ScHt
            // 
            this.Txt_ScHt.Location = new System.Drawing.Point(133, 104);
            this.Txt_ScHt.Name = "Txt_ScHt";
            this.Txt_ScHt.Size = new System.Drawing.Size(224, 21);
            this.Txt_ScHt.TabIndex = 6;
            // 
            // Txt_ScWd
            // 
            this.Txt_ScWd.Location = new System.Drawing.Point(133, 77);
            this.Txt_ScWd.Name = "Txt_ScWd";
            this.Txt_ScWd.Size = new System.Drawing.Size(224, 21);
            this.Txt_ScWd.TabIndex = 5;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(19, 134);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(107, 12);
            this.label11.TabIndex = 4;
            this.label11.Text = "JVM Arguments : ";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(31, 107);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(96, 12);
            this.label10.TabIndex = 3;
            this.label10.Text = "Screen Height : ";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(36, 80);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(91, 12);
            this.label9.TabIndex = 2;
            this.label9.Text = "Screen Width : ";
            // 
            // Txt_ServerIp
            // 
            this.Txt_ServerIp.Location = new System.Drawing.Point(133, 22);
            this.Txt_ServerIp.Name = "Txt_ServerIp";
            this.Txt_ServerIp.Size = new System.Drawing.Size(224, 21);
            this.Txt_ServerIp.TabIndex = 1;
            // 
            // Txt_VersionType
            // 
            this.Txt_VersionType.Location = new System.Drawing.Point(133, 158);
            this.Txt_VersionType.Name = "Txt_VersionType";
            this.Txt_VersionType.Size = new System.Drawing.Size(224, 21);
            this.Txt_VersionType.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(59, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "Server IP : ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(38, 161);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "VersionType : ";
            // 
            // Pb_Progress
            // 
            this.Pb_Progress.Location = new System.Drawing.Point(14, 417);
            this.Pb_Progress.Name = "Pb_Progress";
            this.Pb_Progress.Size = new System.Drawing.Size(776, 23);
            this.Pb_Progress.TabIndex = 19;
            // 
            // Pb_File
            // 
            this.Pb_File.Location = new System.Drawing.Point(14, 388);
            this.Pb_File.Name = "Pb_File";
            this.Pb_File.Size = new System.Drawing.Size(776, 23);
            this.Pb_File.TabIndex = 18;
            // 
            // Lv_Status
            // 
            this.Lv_Status.AutoSize = true;
            this.Lv_Status.Location = new System.Drawing.Point(12, 373);
            this.Lv_Status.Name = "Lv_Status";
            this.Lv_Status.Size = new System.Drawing.Size(41, 12);
            this.Lv_Status.TabIndex = 17;
            this.Lv_Status.Text = "Ready";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnChangeJava);
            this.groupBox1.Controls.Add(this.lbJavaPath);
            this.groupBox1.Controls.Add(this.btnLogin);
            this.groupBox1.Controls.Add(this.lbUsername);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btnChangePath);
            this.groupBox1.Controls.Add(this.txtPath);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(14, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(385, 116);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CmlLib Sample Launcher";
            // 
            // btnChangeJava
            // 
            this.btnChangeJava.Location = new System.Drawing.Point(317, 90);
            this.btnChangeJava.Name = "btnChangeJava";
            this.btnChangeJava.Size = new System.Drawing.Size(58, 23);
            this.btnChangeJava.TabIndex = 21;
            this.btnChangeJava.Text = "Change";
            this.btnChangeJava.UseVisualStyleBackColor = true;
            this.btnChangeJava.Click += new System.EventHandler(this.btnChangeJava_Click);
            // 
            // lbJavaPath
            // 
            this.lbJavaPath.AutoSize = true;
            this.lbJavaPath.Location = new System.Drawing.Point(88, 92);
            this.lbJavaPath.Name = "lbJavaPath";
            this.lbJavaPath.Size = new System.Drawing.Size(95, 12);
            this.lbJavaPath.TabIndex = 20;
            this.lbJavaPath.Text = "Use default java";
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(317, 63);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(58, 23);
            this.btnLogin.TabIndex = 19;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // lbUsername
            // 
            this.lbUsername.AutoSize = true;
            this.lbUsername.Location = new System.Drawing.Point(88, 67);
            this.lbUsername.Name = "lbUsername";
            this.lbUsername.Size = new System.Drawing.Size(56, 12);
            this.lbUsername.TabIndex = 18;
            this.lbUsername.Text = "test_user";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(41, 92);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "Java : ";
            // 
            // btnChangePath
            // 
            this.btnChangePath.Location = new System.Drawing.Point(317, 36);
            this.btnChangePath.Name = "btnChangePath";
            this.btnChangePath.Size = new System.Drawing.Size(58, 23);
            this.btnChangePath.TabIndex = 9;
            this.btnChangePath.Text = "Change";
            this.btnChangePath.UseVisualStyleBackColor = true;
            this.btnChangePath.Click += new System.EventHandler(this.btnChangePath_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(17, 37);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(294, 21);
            this.txtPath.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "Game Path : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Account : ";
            // 
            // btnLaunch
            // 
            this.btnLaunch.Location = new System.Drawing.Point(28, 82);
            this.btnLaunch.Name = "btnLaunch";
            this.btnLaunch.Size = new System.Drawing.Size(330, 55);
            this.btnLaunch.TabIndex = 2;
            this.btnLaunch.Text = "Download and Launch";
            this.btnLaunch.UseVisualStyleBackColor = true;
            this.btnLaunch.Click += new System.EventHandler(this.Btn_Launch_Click);
            // 
            // cbVersion
            // 
            this.cbVersion.FormattingEnabled = true;
            this.cbVersion.Location = new System.Drawing.Point(92, 27);
            this.cbVersion.Name = "cbVersion";
            this.cbVersion.Size = new System.Drawing.Size(182, 20);
            this.cbVersion.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Version : ";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(424, 452);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(204, 12);
            this.label12.TabIndex = 23;
            this.label12.Text = "AlphaBs (ksi123456ab@naver.com)";
            // 
            // btnGithub
            // 
            this.btnGithub.Location = new System.Drawing.Point(634, 446);
            this.btnGithub.Name = "btnGithub";
            this.btnGithub.Size = new System.Drawing.Size(75, 23);
            this.btnGithub.TabIndex = 24;
            this.btnGithub.Text = "GitHub";
            this.btnGithub.UseVisualStyleBackColor = true;
            this.btnGithub.Click += new System.EventHandler(this.btnGithub_Click);
            // 
            // btnWiki
            // 
            this.btnWiki.Location = new System.Drawing.Point(715, 446);
            this.btnWiki.Name = "btnWiki";
            this.btnWiki.Size = new System.Drawing.Size(75, 23);
            this.btnWiki.TabIndex = 25;
            this.btnWiki.Text = "Wiki";
            this.btnWiki.UseVisualStyleBackColor = true;
            this.btnWiki.Click += new System.EventHandler(this.btnWiki_Click);
            // 
            // btnChangelog
            // 
            this.btnChangelog.Location = new System.Drawing.Point(12, 447);
            this.btnChangelog.Name = "btnChangelog";
            this.btnChangelog.Size = new System.Drawing.Size(127, 23);
            this.btnChangelog.TabIndex = 26;
            this.btnChangelog.Text = "GameChangelog";
            this.btnChangelog.UseVisualStyleBackColor = true;
            this.btnChangelog.Click += new System.EventHandler(this.btnChangelog_Click);
            // 
            // rbSequenceDownload
            // 
            this.rbSequenceDownload.AutoSize = true;
            this.rbSequenceDownload.Checked = true;
            this.rbSequenceDownload.Location = new System.Drawing.Point(17, 24);
            this.rbSequenceDownload.Name = "rbSequenceDownload";
            this.rbSequenceDownload.Size = new System.Drawing.Size(140, 16);
            this.rbSequenceDownload.TabIndex = 22;
            this.rbSequenceDownload.TabStop = true;
            this.rbSequenceDownload.Text = "Sequence Download";
            this.rbSequenceDownload.UseVisualStyleBackColor = true;
            // 
            // rbParallelDownload
            // 
            this.rbParallelDownload.AutoSize = true;
            this.rbParallelDownload.Location = new System.Drawing.Point(163, 24);
            this.rbParallelDownload.Name = "rbParallelDownload";
            this.rbParallelDownload.Size = new System.Drawing.Size(212, 16);
            this.rbParallelDownload.TabIndex = 23;
            this.rbParallelDownload.TabStop = true;
            this.rbParallelDownload.Text = "Parallel Download (experimental)";
            this.rbParallelDownload.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cbSkipAssetsDownload);
            this.groupBox3.Controls.Add(this.cbCheckFileHash);
            this.groupBox3.Controls.Add(this.rbSequenceDownload);
            this.groupBox3.Controls.Add(this.rbParallelDownload);
            this.groupBox3.Location = new System.Drawing.Point(14, 134);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(385, 78);
            this.groupBox3.TabIndex = 27;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Download Options";
            // 
            // cbSkipAssetsDownload
            // 
            this.cbSkipAssetsDownload.AutoSize = true;
            this.cbSkipAssetsDownload.Location = new System.Drawing.Point(163, 51);
            this.cbSkipAssetsDownload.Name = "cbSkipAssetsDownload";
            this.cbSkipAssetsDownload.Size = new System.Drawing.Size(151, 16);
            this.cbSkipAssetsDownload.TabIndex = 25;
            this.cbSkipAssetsDownload.Text = "Skip Assets Download";
            this.cbSkipAssetsDownload.UseVisualStyleBackColor = true;
            // 
            // cbCheckFileHash
            // 
            this.cbCheckFileHash.AutoSize = true;
            this.cbCheckFileHash.Checked = true;
            this.cbCheckFileHash.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCheckFileHash.Location = new System.Drawing.Point(40, 51);
            this.cbCheckFileHash.Name = "cbCheckFileHash";
            this.cbCheckFileHash.Size = new System.Drawing.Size(117, 16);
            this.cbCheckFileHash.TabIndex = 24;
            this.cbCheckFileHash.Text = "Check File Hash";
            this.cbCheckFileHash.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnRefreshVersion);
            this.groupBox4.Controls.Add(this.btnForgeInstall);
            this.groupBox4.Controls.Add(this.btnSetLastVersion);
            this.groupBox4.Controls.Add(this.cbVersion);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Controls.Add(this.btnLaunch);
            this.groupBox4.Location = new System.Drawing.Point(14, 218);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(385, 152);
            this.groupBox4.TabIndex = 28;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Launch";
            // 
            // btnForgeInstall
            // 
            this.btnForgeInstall.Location = new System.Drawing.Point(92, 53);
            this.btnForgeInstall.Name = "btnForgeInstall";
            this.btnForgeInstall.Size = new System.Drawing.Size(182, 23);
            this.btnForgeInstall.TabIndex = 3;
            this.btnForgeInstall.Text = "Install Forge";
            this.btnForgeInstall.UseVisualStyleBackColor = true;
            this.btnForgeInstall.Click += new System.EventHandler(this.btnForgeInstall_Click);
            // 
            // btnSetLastVersion
            // 
            this.btnSetLastVersion.Location = new System.Drawing.Point(283, 25);
            this.btnSetLastVersion.Name = "btnSetLastVersion";
            this.btnSetLastVersion.Size = new System.Drawing.Size(75, 23);
            this.btnSetLastVersion.TabIndex = 2;
            this.btnSetLastVersion.Text = "Lastest\r\n";
            this.btnSetLastVersion.UseVisualStyleBackColor = true;
            this.btnSetLastVersion.Click += new System.EventHandler(this.btnSetLastVersion_Click);
            // 
            // btnMojangServer
            // 
            this.btnMojangServer.Location = new System.Drawing.Point(145, 446);
            this.btnMojangServer.Name = "btnMojangServer";
            this.btnMojangServer.Size = new System.Drawing.Size(127, 23);
            this.btnMojangServer.TabIndex = 29;
            this.btnMojangServer.Text = "MojangServer";
            this.btnMojangServer.UseVisualStyleBackColor = true;
            this.btnMojangServer.Click += new System.EventHandler(this.btnMojangServer_Click);
            // 
            // btnOptions
            // 
            this.btnOptions.Location = new System.Drawing.Point(278, 447);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(121, 23);
            this.btnOptions.TabIndex = 30;
            this.btnOptions.Text = "options.txt";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // btnRefreshVersion
            // 
            this.btnRefreshVersion.Location = new System.Drawing.Point(283, 52);
            this.btnRefreshVersion.Name = "btnRefreshVersion";
            this.btnRefreshVersion.Size = new System.Drawing.Size(75, 23);
            this.btnRefreshVersion.TabIndex = 4;
            this.btnRefreshVersion.Text = "Refresh";
            this.btnRefreshVersion.UseVisualStyleBackColor = true;
            this.btnRefreshVersion.Click += new System.EventHandler(this.btnRefreshVersion_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(803, 486);
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
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
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
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lbUsername;
        private System.Windows.Forms.RadioButton rbSequenceDownload;
        private System.Windows.Forms.RadioButton rbParallelDownload;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox cbSkipAssetsDownload;
        private System.Windows.Forms.CheckBox cbCheckFileHash;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnSetLastVersion;
        private System.Windows.Forms.Button btnMojangServer;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Button btnForgeInstall;
        private System.Windows.Forms.Button btnRefreshVersion;
    }
}