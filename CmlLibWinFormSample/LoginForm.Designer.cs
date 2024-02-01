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
            this.gOfflineLogin = new System.Windows.Forms.GroupBox();
            this.btnOfflineLogin = new System.Windows.Forms.Button();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSignout = new System.Windows.Forms.Button();
            this.btnAddNewAccount = new System.Windows.Forms.Button();
            this.btnXboxLogin = new System.Windows.Forms.Button();
            this.txtXboxUsername = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.gOfflineLogin.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gOfflineLogin
            // 
            this.gOfflineLogin.Controls.Add(this.btnOfflineLogin);
            this.gOfflineLogin.Controls.Add(this.txtUsername);
            this.gOfflineLogin.Controls.Add(this.label8);
            this.gOfflineLogin.Location = new System.Drawing.Point(11, 159);
            this.gOfflineLogin.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gOfflineLogin.Name = "gOfflineLogin";
            this.gOfflineLogin.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.gOfflineLogin.Size = new System.Drawing.Size(527, 93);
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
            this.label2.Font = new System.Drawing.Font("굴림", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(8, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(440, 15);
            this.label2.TabIndex = 17;
            this.label2.Text = "CmlLib.Core (.NET Framework WinForm) Sample Launcher";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSignout);
            this.groupBox1.Controls.Add(this.btnAddNewAccount);
            this.groupBox1.Controls.Add(this.btnXboxLogin);
            this.groupBox1.Controls.Add(this.txtXboxUsername);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 47);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(527, 93);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Microsoft Xbox Login";
            // 
            // btnSignout
            // 
            this.btnSignout.Location = new System.Drawing.Point(267, 50);
            this.btnSignout.Name = "btnSignout";
            this.btnSignout.Size = new System.Drawing.Size(158, 23);
            this.btnSignout.TabIndex = 4;
            this.btnSignout.Text = "Signout";
            this.btnSignout.UseVisualStyleBackColor = true;
            this.btnSignout.Click += new System.EventHandler(this.btnSignout_Click);
            // 
            // btnAddNewAccount
            // 
            this.btnAddNewAccount.Location = new System.Drawing.Point(92, 50);
            this.btnAddNewAccount.Name = "btnAddNewAccount";
            this.btnAddNewAccount.Size = new System.Drawing.Size(169, 23);
            this.btnAddNewAccount.TabIndex = 3;
            this.btnAddNewAccount.Text = "Add a new account";
            this.btnAddNewAccount.UseVisualStyleBackColor = true;
            this.btnAddNewAccount.Click += new System.EventHandler(this.btnAddNewAccount_Click);
            // 
            // btnXboxLogin
            // 
            this.btnXboxLogin.Location = new System.Drawing.Point(434, 19);
            this.btnXboxLogin.Name = "btnXboxLogin";
            this.btnXboxLogin.Size = new System.Drawing.Size(86, 54);
            this.btnXboxLogin.TabIndex = 2;
            this.btnXboxLogin.Text = "Login";
            this.btnXboxLogin.UseVisualStyleBackColor = true;
            this.btnXboxLogin.Click += new System.EventHandler(this.btnXboxLogin_Click);
            // 
            // txtXboxUsername
            // 
            this.txtXboxUsername.Location = new System.Drawing.Point(92, 19);
            this.txtXboxUsername.Name = "txtXboxUsername";
            this.txtXboxUsername.ReadOnly = true;
            this.txtXboxUsername.Size = new System.Drawing.Size(333, 25);
            this.txtXboxUsername.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 15);
            this.label3.TabIndex = 0;
            this.label3.Text = "Username: ";
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 260);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.gOfflineLogin);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.gOfflineLogin.ResumeLayout(false);
            this.gOfflineLogin.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.GroupBox gOfflineLogin;
        private System.Windows.Forms.Button btnOfflineLogin;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSignout;
        private System.Windows.Forms.Button btnAddNewAccount;
        private System.Windows.Forms.Button btnXboxLogin;
        private System.Windows.Forms.TextBox txtXboxUsername;
        private System.Windows.Forms.Label label3;
    }
}