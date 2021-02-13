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
            this.btnAutoLogin = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnOfflineLogin = new System.Windows.Forms.Button();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnAutoLoginMojangLauncher = new System.Windows.Forms.Button();
            this.gMojangLogin.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(36, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "Email : ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 108);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "Password : ";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(91, 81);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(237, 21);
            this.txtEmail.TabIndex = 7;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(91, 105);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(237, 21);
            this.txtPassword.TabIndex = 8;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(334, 79);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(75, 47);
            this.btnLogin.TabIndex = 9;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // btnSignout
            // 
            this.btnSignout.Location = new System.Drawing.Point(91, 143);
            this.btnSignout.Name = "btnSignout";
            this.btnSignout.Size = new System.Drawing.Size(75, 23);
            this.btnSignout.TabIndex = 10;
            this.btnSignout.Text = "Signout";
            this.btnSignout.UseVisualStyleBackColor = true;
            this.btnSignout.Click += new System.EventHandler(this.btnSignout_Click);
            // 
            // btnInvalidate
            // 
            this.btnInvalidate.Location = new System.Drawing.Point(172, 143);
            this.btnInvalidate.Name = "btnInvalidate";
            this.btnInvalidate.Size = new System.Drawing.Size(75, 23);
            this.btnInvalidate.TabIndex = 11;
            this.btnInvalidate.Text = "Invalidate";
            this.btnInvalidate.UseVisualStyleBackColor = true;
            this.btnInvalidate.Click += new System.EventHandler(this.btnInvalidate_Click);
            // 
            // btnDeleteToken
            // 
            this.btnDeleteToken.Location = new System.Drawing.Point(253, 143);
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
            this.label7.Location = new System.Drawing.Point(30, 148);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 12);
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
            this.gMojangLogin.Location = new System.Drawing.Point(12, 12);
            this.gMojangLogin.Name = "gMojangLogin";
            this.gMojangLogin.Size = new System.Drawing.Size(425, 176);
            this.gMojangLogin.TabIndex = 14;
            this.gMojangLogin.TabStop = false;
            this.gMojangLogin.Text = "Mojang Login";
            // 
            // btnAutoLogin
            // 
            this.btnAutoLogin.Location = new System.Drawing.Point(105, 23);
            this.btnAutoLogin.Name = "btnAutoLogin";
            this.btnAutoLogin.Size = new System.Drawing.Size(282, 23);
            this.btnAutoLogin.TabIndex = 14;
            this.btnAutoLogin.Text = "TryAutoLogin (CmlLib.Core cache file)";
            this.btnAutoLogin.UseVisualStyleBackColor = true;
            this.btnAutoLogin.Click += new System.EventHandler(this.btnAutoLogin_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnOfflineLogin);
            this.groupBox1.Controls.Add(this.txtUsername);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(12, 194);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(425, 58);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Offline Login";
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
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(91, 20);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(237, 21);
            this.txtUsername.TabIndex = 1;
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
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(12, 258);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(425, 38);
            this.btnClose.TabIndex = 16;
            this.btnClose.Text = "Save and Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnAutoLoginMojangLauncher
            // 
            this.btnAutoLoginMojangLauncher.Location = new System.Drawing.Point(105, 52);
            this.btnAutoLoginMojangLauncher.Name = "btnAutoLoginMojangLauncher";
            this.btnAutoLoginMojangLauncher.Size = new System.Drawing.Size(282, 23);
            this.btnAutoLoginMojangLauncher.TabIndex = 15;
            this.btnAutoLoginMojangLauncher.Text = "TryAutoLogin (launcher_accounts.json)";
            this.btnAutoLoginMojangLauncher.UseVisualStyleBackColor = true;
            this.btnAutoLoginMojangLauncher.Click += new System.EventHandler(this.btnAutoLoginMojangLauncher_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 308);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gMojangLogin);
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.gMojangLogin.ResumeLayout(false);
            this.gMojangLogin.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOfflineLogin;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnAutoLoginMojangLauncher;
    }
}