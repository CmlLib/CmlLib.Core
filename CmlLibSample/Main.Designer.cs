namespace CmlLibSample
{
    partial class Main
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Txt_Java = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Txt_Ram = new System.Windows.Forms.TextBox();
            this.Xmx_RAM = new System.Windows.Forms.Label();
            this.Btn_apply = new System.Windows.Forms.Button();
            this.Path = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Btn_Login = new System.Windows.Forms.Button();
            this.Txt_Email = new System.Windows.Forms.TextBox();
            this.Txt_Pw = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Btn_Launch = new System.Windows.Forms.Button();
            this.Cb_Version = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Lv_Status = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.progressBar2 = new System.Windows.Forms.ProgressBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Txt_JavaArgs = new System.Windows.Forms.TextBox();
            this.Txt_ScHt = new System.Windows.Forms.TextBox();
            this.Txt_ScWd = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.Txt_ServerIp = new System.Windows.Forms.TextBox();
            this.Txt_LauncherName = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.Btn_loginOption = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Txt_Java);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.Txt_Ram);
            this.groupBox1.Controls.Add(this.Xmx_RAM);
            this.groupBox1.Controls.Add(this.Btn_apply);
            this.groupBox1.Controls.Add(this.Path);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.Btn_Login);
            this.groupBox1.Controls.Add(this.Txt_Email);
            this.groupBox1.Controls.Add(this.Txt_Pw);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.Btn_Launch);
            this.groupBox1.Controls.Add(this.Cb_Version);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(385, 168);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CML Launcher";
            // 
            // Txt_Java
            // 
            this.Txt_Java.Location = new System.Drawing.Point(92, 135);
            this.Txt_Java.Name = "Txt_Java";
            this.Txt_Java.Size = new System.Drawing.Size(184, 21);
            this.Txt_Java.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(43, 138);
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
            this.Xmx_RAM.Size = new System.Drawing.Size(64, 12);
            this.Xmx_RAM.TabIndex = 10;
            this.Xmx_RAM.Text = "Xmx(M) : ";
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
            // Path
            // 
            this.Path.Location = new System.Drawing.Point(92, 22);
            this.Path.Name = "Path";
            this.Path.Size = new System.Drawing.Size(182, 21);
            this.Path.TabIndex = 8;
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
            this.Btn_Launch.Location = new System.Drawing.Point(283, 133);
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
            // Lv_Status
            // 
            this.Lv_Status.AutoSize = true;
            this.Lv_Status.Location = new System.Drawing.Point(10, 413);
            this.Lv_Status.Name = "Lv_Status";
            this.Lv_Status.Size = new System.Drawing.Size(41, 12);
            this.Lv_Status.TabIndex = 1;
            this.Lv_Status.Text = "Ready";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 433);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(385, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // progressBar2
            // 
            this.progressBar2.Location = new System.Drawing.Point(12, 462);
            this.progressBar2.Name = "progressBar2";
            this.progressBar2.Size = new System.Drawing.Size(385, 23);
            this.progressBar2.TabIndex = 3;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Txt_JavaArgs);
            this.groupBox2.Controls.Add(this.Txt_ScHt);
            this.groupBox2.Controls.Add(this.Txt_ScWd);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.Txt_ServerIp);
            this.groupBox2.Controls.Add(this.Txt_LauncherName);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Location = new System.Drawing.Point(12, 237);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(385, 170);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options (not required)";
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
            this.label11.Location = new System.Drawing.Point(13, 134);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(114, 12);
            this.label11.TabIndex = 4;
            this.label11.Text = "Custom Java Arg : ";
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
            this.Txt_ServerIp.Location = new System.Drawing.Point(133, 48);
            this.Txt_ServerIp.Name = "Txt_ServerIp";
            this.Txt_ServerIp.Size = new System.Drawing.Size(224, 21);
            this.Txt_ServerIp.TabIndex = 1;
            // 
            // Txt_LauncherName
            // 
            this.Txt_LauncherName.Location = new System.Drawing.Point(133, 21);
            this.Txt_LauncherName.Name = "Txt_LauncherName";
            this.Txt_LauncherName.Size = new System.Drawing.Size(224, 21);
            this.Txt_LauncherName.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(59, 51);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "Server IP : ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(19, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(108, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "Launcher Name : ";
            // 
            // Btn_loginOption
            // 
            this.Btn_loginOption.Location = new System.Drawing.Point(285, 55);
            this.Btn_loginOption.Name = "Btn_loginOption";
            this.Btn_loginOption.Size = new System.Drawing.Size(90, 23);
            this.Btn_loginOption.TabIndex = 14;
            this.Btn_loginOption.Text = "Login Option";
            this.Btn_loginOption.UseVisualStyleBackColor = true;
            this.Btn_loginOption.Click += new System.EventHandler(this.Button2_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(329, 48);
            this.label5.TabIndex = 15;
            this.label5.Text = "How to use : \r\n1. Enter your email and password and click Login button.\r\n(if auto" +
    "login successed, skip this)\r\n2. Select version and click Launch button";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 497);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Btn_loginOption);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.progressBar2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.Lv_Status);
            this.Controls.Add(this.groupBox1);
            this.Name = "Main";
            this.Text = "Main";
            this.Load += new System.EventHandler(this.Main_Load);
            this.Shown += new System.EventHandler(this.Main_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Btn_Login;
        private System.Windows.Forms.TextBox Txt_Email;
        private System.Windows.Forms.TextBox Txt_Pw;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Btn_Launch;
        private System.Windows.Forms.ComboBox Cb_Version;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label Lv_Status;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.ProgressBar progressBar2;
        private System.Windows.Forms.Button Btn_apply;
        private System.Windows.Forms.TextBox Path;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Txt_Java;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Txt_Ram;
        private System.Windows.Forms.Label Xmx_RAM;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox Txt_LauncherName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox Txt_ServerIp;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox Txt_JavaArgs;
        private System.Windows.Forms.TextBox Txt_ScHt;
        private System.Windows.Forms.TextBox Txt_ScWd;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button Btn_loginOption;
        private System.Windows.Forms.Label label5;
    }
}

