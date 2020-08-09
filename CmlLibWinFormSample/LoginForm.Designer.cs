namespace CmlLibWinFormSample
{
    partial class LoginForm
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
            this.lvCTc = new System.Windows.Forms.Label();
            this.lvUsernamec = new System.Windows.Forms.Label();
            this.lvUUIDc = new System.Windows.Forms.Label();
            this.lvATc = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnLogin = new System.Windows.Forms.Button();
            this.btnSignout = new System.Windows.Forms.Button();
            this.btnInvalidate = new System.Windows.Forms.Button();
            this.btnDeleteToken = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.gMojangLogin = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.btnOfflineLogin = new System.Windows.Forms.Button();
            this.btnAutoLogin = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lvUsername = new System.Windows.Forms.Label();
            this.lvUUID = new System.Windows.Forms.Label();
            this.lvAT = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.gMojangLogin.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lvCTc);
            this.groupBox2.Controls.Add(this.lvUsernamec);
            this.groupBox2.Controls.Add(this.lvUUIDc);
            this.groupBox2.Controls.Add(this.lvATc);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(425, 93);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Cached Login Session";
            // 
            // lvCTc
            // 
            this.lvCTc.AutoSize = true;
            this.lvCTc.Location = new System.Drawing.Point(118, 74);
            this.lvCTc.Name = "lvCTc";
            this.lvCTc.Size = new System.Drawing.Size(44, 12);
            this.lvCTc.TabIndex = 7;
            this.lvCTc.Text = "label10";
            // 
            // lvUsernamec
            // 
            this.lvUsernamec.AutoSize = true;
            this.lvUsernamec.Location = new System.Drawing.Point(118, 56);
            this.lvUsernamec.Name = "lvUsernamec";
            this.lvUsernamec.Size = new System.Drawing.Size(38, 12);
            this.lvUsernamec.TabIndex = 6;
            this.lvUsernamec.Text = "label9";
            // 
            // lvUUIDc
            // 
            this.lvUUIDc.AutoSize = true;
            this.lvUUIDc.Location = new System.Drawing.Point(118, 36);
            this.lvUUIDc.Name = "lvUUIDc";
            this.lvUUIDc.Size = new System.Drawing.Size(38, 12);
            this.lvUUIDc.TabIndex = 5;
            this.lvUUIDc.Text = "label8";
            // 
            // lvATc
            // 
            this.lvATc.AutoSize = true;
            this.lvATc.Location = new System.Drawing.Point(118, 19);
            this.lvATc.Name = "lvATc";
            this.lvATc.Size = new System.Drawing.Size(38, 12);
            this.lvATc.TabIndex = 4;
            this.lvATc.Text = "label7";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "ClientToken : ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Username : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "UUID :";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "AccessToken : ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(42, 48);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "Email : ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "Password : ";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(97, 45);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(237, 21);
            this.txtEmail.TabIndex = 7;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(97, 69);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(237, 21);
            this.txtPassword.TabIndex = 8;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(340, 43);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 47);
            this.btnLogin.TabIndex = 9;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnSignout
            // 
            this.btnSignout.Location = new System.Drawing.Point(97, 107);
            this.btnSignout.Name = "btnSignout";
            this.btnSignout.Size = new System.Drawing.Size(75, 23);
            this.btnSignout.TabIndex = 10;
            this.btnSignout.Text = "Signout";
            this.btnSignout.UseVisualStyleBackColor = true;
            this.btnSignout.Click += new System.EventHandler(this.btnSignout_Click);
            // 
            // btnInvalidate
            // 
            this.btnInvalidate.Location = new System.Drawing.Point(178, 107);
            this.btnInvalidate.Name = "btnInvalidate";
            this.btnInvalidate.Size = new System.Drawing.Size(75, 23);
            this.btnInvalidate.TabIndex = 11;
            this.btnInvalidate.Text = "Invalidate";
            this.btnInvalidate.UseVisualStyleBackColor = true;
            this.btnInvalidate.Click += new System.EventHandler(this.btnInvalidate_Click);
            // 
            // btnDeleteToken
            // 
            this.btnDeleteToken.Location = new System.Drawing.Point(259, 107);
            this.btnDeleteToken.Name = "btnDeleteToken";
            this.btnDeleteToken.Size = new System.Drawing.Size(98, 23);
            this.btnDeleteToken.TabIndex = 12;
            this.btnDeleteToken.Text = "Delete Token";
            this.btnDeleteToken.UseVisualStyleBackColor = true;
            this.btnDeleteToken.Click += new System.EventHandler(this.btnDeleteToken_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(36, 112);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 12);
            this.label7.TabIndex = 13;
            this.label7.Text = "Logout : ";
            // 
            // gMojangLogin
            // 
            this.gMojangLogin.Controls.Add(this.btnAutoLogin);
            this.gMojangLogin.Controls.Add(this.btnLogin);
            this.gMojangLogin.Controls.Add(this.label7);
            this.gMojangLogin.Controls.Add(this.label5);
            this.gMojangLogin.Controls.Add(this.btnDeleteToken);
            this.gMojangLogin.Controls.Add(this.label6);
            this.gMojangLogin.Controls.Add(this.btnInvalidate);
            this.gMojangLogin.Controls.Add(this.txtEmail);
            this.gMojangLogin.Controls.Add(this.btnSignout);
            this.gMojangLogin.Controls.Add(this.txtPassword);
            this.gMojangLogin.Location = new System.Drawing.Point(12, 111);
            this.gMojangLogin.Name = "gMojangLogin";
            this.gMojangLogin.Size = new System.Drawing.Size(425, 145);
            this.gMojangLogin.TabIndex = 14;
            this.gMojangLogin.TabStop = false;
            this.gMojangLogin.Text = "Mojang Login";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnOfflineLogin);
            this.groupBox1.Controls.Add(this.txtUsername);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(12, 262);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(425, 58);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Offline Login";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(11, 23);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "Username : ";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(91, 20);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(237, 21);
            this.txtUsername.TabIndex = 1;
            // 
            // btnOfflineLogin
            // 
            this.btnOfflineLogin.Location = new System.Drawing.Point(334, 20);
            this.btnOfflineLogin.Name = "btnOfflineLogin";
            this.btnOfflineLogin.Size = new System.Drawing.Size(75, 21);
            this.btnOfflineLogin.TabIndex = 14;
            this.btnOfflineLogin.Text = "Login";
            this.btnOfflineLogin.UseVisualStyleBackColor = true;
            this.btnOfflineLogin.Click += new System.EventHandler(this.btnOfflineLogin_Click);
            // 
            // btnAutoLogin
            // 
            this.btnAutoLogin.Location = new System.Drawing.Point(309, 15);
            this.btnAutoLogin.Name = "btnAutoLogin";
            this.btnAutoLogin.Size = new System.Drawing.Size(106, 23);
            this.btnAutoLogin.TabIndex = 14;
            this.btnAutoLogin.Text = "Try Auto Login";
            this.btnAutoLogin.UseVisualStyleBackColor = true;
            this.btnAutoLogin.Click += new System.EventHandler(this.btnAutoLogin_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lvUsername);
            this.groupBox4.Controls.Add(this.lvUUID);
            this.groupBox4.Controls.Add(this.lvAT);
            this.groupBox4.Controls.Add(this.label14);
            this.groupBox4.Controls.Add(this.label15);
            this.groupBox4.Controls.Add(this.label16);
            this.groupBox4.Location = new System.Drawing.Point(12, 326);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(425, 78);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Current Login Session";
            // 
            // lvUsername
            // 
            this.lvUsername.AutoSize = true;
            this.lvUsername.Location = new System.Drawing.Point(118, 56);
            this.lvUsername.Name = "lvUsername";
            this.lvUsername.Size = new System.Drawing.Size(38, 12);
            this.lvUsername.TabIndex = 6;
            this.lvUsername.Text = "label9";
            // 
            // lvUUID
            // 
            this.lvUUID.AutoSize = true;
            this.lvUUID.Location = new System.Drawing.Point(118, 36);
            this.lvUUID.Name = "lvUUID";
            this.lvUUID.Size = new System.Drawing.Size(38, 12);
            this.lvUUID.TabIndex = 5;
            this.lvUUID.Text = "label8";
            // 
            // lvAT
            // 
            this.lvAT.AutoSize = true;
            this.lvAT.Location = new System.Drawing.Point(118, 19);
            this.lvAT.Name = "lvAT";
            this.lvAT.Size = new System.Drawing.Size(38, 12);
            this.lvAT.TabIndex = 4;
            this.lvAT.Text = "label7";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(16, 56);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(75, 12);
            this.label14.TabIndex = 2;
            this.label14.Text = "Username : ";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(16, 36);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(40, 12);
            this.label15.TabIndex = 1;
            this.label15.Text = "UUID :";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(16, 19);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(95, 12);
            this.label16.TabIndex = 0;
            this.label16.Text = "AccessToken : ";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(12, 410);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(425, 38);
            this.btnClose.TabIndex = 16;
            this.btnClose.Text = "Save and Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.button1_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 456);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gMojangLogin);
            this.Controls.Add(this.groupBox2);
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.gMojangLogin.ResumeLayout(false);
            this.gMojangLogin.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lvCTc;
        private System.Windows.Forms.Label lvUsernamec;
        private System.Windows.Forms.Label lvUUIDc;
        private System.Windows.Forms.Label lvATc;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Button btnSignout;
        private System.Windows.Forms.Button btnInvalidate;
        private System.Windows.Forms.Button btnDeleteToken;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox gMojangLogin;
        private System.Windows.Forms.Button btnAutoLogin;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOfflineLogin;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label lvUsername;
        private System.Windows.Forms.Label lvUUID;
        private System.Windows.Forms.Label lvAT;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button btnClose;
    }
}