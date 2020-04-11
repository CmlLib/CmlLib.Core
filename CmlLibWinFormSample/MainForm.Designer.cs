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
            this.label5 = new System.Windows.Forms.Label();
            this.Btn_loginOption = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Txt_DockIcon = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.Txt_DockName = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.Txt_GLauncherVersion = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.Txt_GLauncherName = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.Txt_ServerPort = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.Txt_JavaArgs = new System.Windows.Forms.TextBox();
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
            this.label19 = new System.Windows.Forms.Label();
            this.Txt_ForgeVersion = new System.Windows.Forms.TextBox();
            this.Cb_Forge = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.Txt_Java = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Txt_Ram = new System.Windows.Forms.TextBox();
            this.Xmx_RAM = new System.Windows.Forms.Label();
            this.Btn_apply = new System.Windows.Forms.Button();
            this.Txt_Path = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Btn_Login = new System.Windows.Forms.Button();
            this.Txt_Email = new System.Windows.Forms.TextBox();
            this.Txt_Pw = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Btn_Launch = new System.Windows.Forms.Button();
            this.Cb_Version = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btnGithub = new System.Windows.Forms.Button();
            this.btnWiki = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 9);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(329, 48);
            this.label5.TabIndex = 22;
            this.label5.Text = "How to use : \r\n1. Enter your email and password and click Login button.\r\n(if auto" +
    "login successed, skip this)\r\n2. Select version and click Launch button";
            // 
            // Btn_loginOption
            // 
            this.Btn_loginOption.Location = new System.Drawing.Point(287, 62);
            this.Btn_loginOption.Name = "Btn_loginOption";
            this.Btn_loginOption.Size = new System.Drawing.Size(90, 23);
            this.Btn_loginOption.TabIndex = 21;
            this.Btn_loginOption.Text = "Login Option";
            this.Btn_loginOption.UseVisualStyleBackColor = true;
            this.Btn_loginOption.Click += new System.EventHandler(this.Btn_loginOption_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Txt_DockIcon);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.Txt_DockName);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.Txt_GLauncherVersion);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.Txt_GLauncherName);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.Txt_ServerPort);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.Txt_JavaArgs);
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
            this.groupBox2.Size = new System.Drawing.Size(385, 297);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options (Empty textbox means using default option)";
            // 
            // Txt_DockIcon
            // 
            this.Txt_DockIcon.Location = new System.Drawing.Point(133, 266);
            this.Txt_DockIcon.Name = "Txt_DockIcon";
            this.Txt_DockIcon.Size = new System.Drawing.Size(224, 21);
            this.Txt_DockIcon.TabIndex = 17;
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
            this.Pb_Progress.Location = new System.Drawing.Point(14, 353);
            this.Pb_Progress.Name = "Pb_Progress";
            this.Pb_Progress.Size = new System.Drawing.Size(776, 23);
            this.Pb_Progress.TabIndex = 19;
            // 
            // Pb_File
            // 
            this.Pb_File.Location = new System.Drawing.Point(14, 324);
            this.Pb_File.Name = "Pb_File";
            this.Pb_File.Size = new System.Drawing.Size(776, 23);
            this.Pb_File.TabIndex = 18;
            // 
            // Lv_Status
            // 
            this.Lv_Status.AutoSize = true;
            this.Lv_Status.Location = new System.Drawing.Point(12, 309);
            this.Lv_Status.Name = "Lv_Status";
            this.Lv_Status.Size = new System.Drawing.Size(41, 12);
            this.Lv_Status.TabIndex = 17;
            this.Lv_Status.Text = "Ready";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.Txt_ForgeVersion);
            this.groupBox1.Controls.Add(this.Cb_Forge);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.Txt_Java);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.Txt_Ram);
            this.groupBox1.Controls.Add(this.Xmx_RAM);
            this.groupBox1.Controls.Add(this.Btn_apply);
            this.groupBox1.Controls.Add(this.Txt_Path);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.Btn_Login);
            this.groupBox1.Controls.Add(this.Txt_Email);
            this.groupBox1.Controls.Add(this.Txt_Pw);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.Btn_Launch);
            this.groupBox1.Controls.Add(this.Cb_Version);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(14, 70);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(385, 203);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CmlLib Sample Launcher";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(293, 140);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(60, 12);
            this.label19.TabIndex = 17;
            this.label19.Text = "ex) 28.2.0";
            // 
            // Txt_ForgeVersion
            // 
            this.Txt_ForgeVersion.Location = new System.Drawing.Point(128, 135);
            this.Txt_ForgeVersion.Name = "Txt_ForgeVersion";
            this.Txt_ForgeVersion.Size = new System.Drawing.Size(164, 21);
            this.Txt_ForgeVersion.TabIndex = 16;
            // 
            // Cb_Forge
            // 
            this.Cb_Forge.AutoSize = true;
            this.Cb_Forge.Location = new System.Drawing.Point(11, 137);
            this.Cb_Forge.Name = "Cb_Forge";
            this.Cb_Forge.Size = new System.Drawing.Size(115, 16);
            this.Cb_Forge.TabIndex = 15;
            this.Cb_Forge.Text = "Forge Version : ";
            this.Cb_Forge.UseVisualStyleBackColor = true;
            this.Cb_Forge.CheckedChanged += new System.EventHandler(this.Cb_Forge_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(44, 184);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(293, 12);
            this.label13.TabIndex = 14;
            this.label13.Text = "(Empty java path will download JRE automatically)";
            // 
            // Txt_Java
            // 
            this.Txt_Java.Location = new System.Drawing.Point(93, 160);
            this.Txt_Java.Name = "Txt_Java";
            this.Txt_Java.Size = new System.Drawing.Size(184, 21);
            this.Txt_Java.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(44, 163);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 12);
            this.label6.TabIndex = 12;
            this.label6.Text = "Java : ";
            // 
            // Txt_Ram
            // 
            this.Txt_Ram.Location = new System.Drawing.Point(283, 109);
            this.Txt_Ram.Name = "Txt_Ram";
            this.Txt_Ram.Size = new System.Drawing.Size(75, 21);
            this.Txt_Ram.TabIndex = 11;
            this.Txt_Ram.Text = "1024";
            // 
            // Xmx_RAM
            // 
            this.Xmx_RAM.AutoSize = true;
            this.Xmx_RAM.Location = new System.Drawing.Point(212, 113);
            this.Xmx_RAM.Name = "Xmx_RAM";
            this.Xmx_RAM.Size = new System.Drawing.Size(71, 12);
            this.Xmx_RAM.TabIndex = 10;
            this.Xmx_RAM.Text = "Xmx(Mb) : ";
            // 
            // Btn_apply
            // 
            this.Btn_apply.Location = new System.Drawing.Point(283, 20);
            this.Btn_apply.Name = "Btn_apply";
            this.Btn_apply.Size = new System.Drawing.Size(75, 23);
            this.Btn_apply.TabIndex = 9;
            this.Btn_apply.Text = "apply";
            this.Btn_apply.UseVisualStyleBackColor = true;
            this.Btn_apply.Click += new System.EventHandler(this.Btn_apply_Click);
            // 
            // Txt_Path
            // 
            this.Txt_Path.Location = new System.Drawing.Point(92, 22);
            this.Txt_Path.Name = "Txt_Path";
            this.Txt_Path.Size = new System.Drawing.Size(182, 21);
            this.Txt_Path.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(44, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(42, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "Path : ";
            // 
            // Btn_Login
            // 
            this.Btn_Login.Location = new System.Drawing.Point(283, 49);
            this.Btn_Login.Name = "Btn_Login";
            this.Btn_Login.Size = new System.Drawing.Size(75, 49);
            this.Btn_Login.TabIndex = 6;
            this.Btn_Login.Text = "Login";
            this.Btn_Login.UseVisualStyleBackColor = true;
            this.Btn_Login.Click += new System.EventHandler(this.Btn_Login_Click);
            // 
            // Txt_Email
            // 
            this.Txt_Email.Location = new System.Drawing.Point(92, 50);
            this.Txt_Email.Name = "Txt_Email";
            this.Txt_Email.Size = new System.Drawing.Size(182, 21);
            this.Txt_Email.TabIndex = 5;
            // 
            // Txt_Pw
            // 
            this.Txt_Pw.Location = new System.Drawing.Point(92, 77);
            this.Txt_Pw.Name = "Txt_Pw";
            this.Txt_Pw.Size = new System.Drawing.Size(182, 21);
            this.Txt_Pw.TabIndex = 5;
            this.Txt_Pw.UseSystemPasswordChar = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "Password : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(37, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Email : ";
            // 
            // Btn_Launch
            // 
            this.Btn_Launch.Location = new System.Drawing.Point(283, 160);
            this.Btn_Launch.Name = "Btn_Launch";
            this.Btn_Launch.Size = new System.Drawing.Size(75, 23);
            this.Btn_Launch.TabIndex = 2;
            this.Btn_Launch.Text = "Launch";
            this.Btn_Launch.UseVisualStyleBackColor = true;
            this.Btn_Launch.Click += new System.EventHandler(this.Btn_Launch_Click);
            // 
            // Cb_Version
            // 
            this.Cb_Version.FormattingEnabled = true;
            this.Cb_Version.Location = new System.Drawing.Point(92, 109);
            this.Cb_Version.Name = "Cb_Version";
            this.Cb_Version.Size = new System.Drawing.Size(114, 20);
            this.Cb_Version.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Version : ";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(23, 285);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(204, 12);
            this.label12.TabIndex = 23;
            this.label12.Text = "AlphaBs (ksi123456ab@naver.com)";
            // 
            // btnGithub
            // 
            this.btnGithub.Location = new System.Drawing.Point(233, 279);
            this.btnGithub.Name = "btnGithub";
            this.btnGithub.Size = new System.Drawing.Size(75, 23);
            this.btnGithub.TabIndex = 24;
            this.btnGithub.Text = "GitHub";
            this.btnGithub.UseVisualStyleBackColor = true;
            this.btnGithub.Click += new System.EventHandler(this.btnGithub_Click);
            // 
            // btnWiki
            // 
            this.btnWiki.Location = new System.Drawing.Point(314, 279);
            this.btnWiki.Name = "btnWiki";
            this.btnWiki.Size = new System.Drawing.Size(75, 23);
            this.btnWiki.TabIndex = 25;
            this.btnWiki.Text = "Wiki";
            this.btnWiki.UseVisualStyleBackColor = true;
            this.btnWiki.Click += new System.EventHandler(this.btnWiki_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(810, 386);
            this.Controls.Add(this.btnWiki);
            this.Controls.Add(this.btnGithub);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Btn_loginOption);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button Btn_loginOption;
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
        private System.Windows.Forms.TextBox Txt_Java;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Txt_Ram;
        private System.Windows.Forms.Label Xmx_RAM;
        private System.Windows.Forms.Button Btn_apply;
        private System.Windows.Forms.TextBox Txt_Path;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button Btn_Login;
        private System.Windows.Forms.TextBox Txt_Email;
        private System.Windows.Forms.TextBox Txt_Pw;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Btn_Launch;
        private System.Windows.Forms.ComboBox Cb_Version;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnGithub;
        private System.Windows.Forms.Label label13;
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
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox Txt_ForgeVersion;
        private System.Windows.Forms.CheckBox Cb_Forge;
    }
}