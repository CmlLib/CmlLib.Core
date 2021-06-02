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
            this.btnAutoLoginMojangLauncher = new System.Windows.Forms.Button();
            this.btnAutoLogin = new System.Windows.Forms.Button();
            this.gOfflineLogin = new System.Windows.Forms.GroupBox();
            this.btnOfflineLogin = new System.Windows.Forms.Button();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.gMojangLogin.SuspendLayout();
            this.gOfflineLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(41, 105);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 15);
            this.label5.TabIndex = 5;
            this.label5.Text = "Email : ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 135);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 15);
            this.label6.TabIndex = 6;
            this.label6.Text = "Password : ";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(104, 101);
            this.txtEmail.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(325, 25);
            this.txtEmail.TabIndex = 7;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(104, 131);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(325, 25);
            this.txtPassword.TabIndex = 8;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(435, 100);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(86, 59);
            this.btnLogin.TabIndex = 9;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnSignout
            // 
            this.btnSignout.Location = new System.Drawing.Point(104, 179);
            this.btnSignout.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSignout.Name = "btnSignout";
            this.btnSignout.Size = new System.Drawing.Size(86, 29);
            this.btnSignout.TabIndex = 10;
            this.btnSignout.Text = "Signout";
            this.btnSignout.UseVisualStyleBackColor = true;
            this.btnSignout.Click += new System.EventHandler(this.btnSignout_Click);
            // 
            // btnInvalidate
            // 
            this.btnInvalidate.Location = new System.Drawing.Point(197, 179);
            this.btnInvalidate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnInvalidate.Name = "btnInvalidate";
            this.btnInvalidate.Size = new System.Drawing.Size(86, 29);
            this.btnInvalidate.TabIndex = 11;
            this.btnInvalidate.Text = "Invalidate";
            this.btnInvalidate.UseVisualStyleBackColor = true;
            this.btnInvalidate.Click += new System.EventHandler(this.btnInvalidate_Click);
            // 
            // btnDeleteToken
            // 
            this.btnDeleteToken.Location = new System.Drawing.Point(289, 179);
            this.btnDeleteToken.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDeleteToken.Name = "btnDeleteToken";
            this.btnDeleteToken.Size = new System.Drawing.Size(112, 29);
            this.btnDeleteToken.TabIndex = 12;
            this.btnDeleteToken.Text = "Delete Token";
            this.btnDeleteToken.UseVisualStyleBackColor = true;
            this.btnDeleteToken.Click += new System.EventHandler(this.btnDeleteToken_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(34, 185);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 15);
            this.label7.TabIndex = 13;
            this.label7.Text = "Logout : ";
            // 
            // gMojangLogin
            // 
            this.gMojangLogin.Controls.Add(this.btnAutoLoginMojangLauncher);
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
            this.gMojangLogin.Location = new System.Drawing.Point(12, 65);
            this.gMojangLogin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gMojangLogin.Name = "gMojangLogin";
            this.gMojangLogin.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gMojangLogin.Size = new System.Drawing.Size(527, 220);
            this.gMojangLogin.TabIndex = 14;
            this.gMojangLogin.TabStop = false;
            this.gMojangLogin.Text = "Mojang Login";
            // 
            // btnAutoLoginMojangLauncher
            // 
            this.btnAutoLoginMojangLauncher.Location = new System.Drawing.Point(104, 64);
            this.btnAutoLoginMojangLauncher.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAutoLoginMojangLauncher.Name = "btnAutoLoginMojangLauncher";
            this.btnAutoLoginMojangLauncher.Size = new System.Drawing.Size(322, 29);
            this.btnAutoLoginMojangLauncher.TabIndex = 15;
            this.btnAutoLoginMojangLauncher.Text = "TryAutoLogin (launcher_accounts.json)";
            this.btnAutoLoginMojangLauncher.UseVisualStyleBackColor = true;
            this.btnAutoLoginMojangLauncher.Click += new System.EventHandler(this.btnAutoLoginMojangLauncher_Click);
            // 
            // btnAutoLogin
            // 
            this.btnAutoLogin.Location = new System.Drawing.Point(104, 27);
            this.btnAutoLogin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAutoLogin.Name = "btnAutoLogin";
            this.btnAutoLogin.Size = new System.Drawing.Size(322, 29);
            this.btnAutoLogin.TabIndex = 14;
            this.btnAutoLogin.Text = "TryAutoLogin (CmlLib.Core cache file)";
            this.btnAutoLogin.UseVisualStyleBackColor = true;
            this.btnAutoLogin.Click += new System.EventHandler(this.btnAutoLogin_Click);
            // 
            // gOfflineLogin
            // 
            this.gOfflineLogin.Controls.Add(this.btnOfflineLogin);
            this.gOfflineLogin.Controls.Add(this.txtUsername);
            this.gOfflineLogin.Controls.Add(this.label8);
            this.gOfflineLogin.Location = new System.Drawing.Point(12, 326);
            this.gOfflineLogin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gOfflineLogin.Name = "gOfflineLogin";
            this.gOfflineLogin.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gOfflineLogin.Size = new System.Drawing.Size(527, 72);
            this.gOfflineLogin.TabIndex = 15;
            this.gOfflineLogin.TabStop = false;
            this.gOfflineLogin.Text = "Offline Login";
            // 
            // btnOfflineLogin
            // 
            this.btnOfflineLogin.Location = new System.Drawing.Point(435, 25);
            this.btnOfflineLogin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnOfflineLogin.Name = "btnOfflineLogin";
            this.btnOfflineLogin.Size = new System.Drawing.Size(86, 26);
            this.btnOfflineLogin.TabIndex = 14;
            this.btnOfflineLogin.Text = "Login";
            this.btnOfflineLogin.UseVisualStyleBackColor = true;
            this.btnOfflineLogin.Click += new System.EventHandler(this.btnOfflineLogin_Click);
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(104, 25);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(322, 25);
            this.txtUsername.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 29);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(87, 15);
            this.label8.TabIndex = 0;
            this.label8.Text = "Username : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (129)));
            this.label2.Location = new System.Drawing.Point(8, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(532, 19);
            this.label2.TabIndex = 17;
            this.label2.Text = "CmlLib.Core (.NET Framework WinForm) Sample Launcher";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (129)));
            this.label1.Location = new System.Drawing.Point(19, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(317, 15);
            this.label1.TabIndex = 18;
            this.label1.Text = "Please login with your mojang account : ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (129)));
            this.label3.Location = new System.Drawing.Point(12, 298);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(280, 15);
            this.label3.TabIndex = 19;
            this.label3.Text = "or, Just type a username you want :";
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 410);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.gOfflineLogin);
            this.Controls.Add(this.gMojangLogin);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.gMojangLogin.ResumeLayout(false);
            this.gMojangLogin.PerformLayout();
            this.gOfflineLogin.ResumeLayout(false);
            this.gOfflineLogin.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
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
        private System.Windows.Forms.GroupBox gOfflineLogin;
        private System.Windows.Forms.Button btnOfflineLogin;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnAutoLoginMojangLauncher;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
    }
}