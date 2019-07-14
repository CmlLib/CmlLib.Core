namespace CmlLibSample
{
    partial class Form3
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lvAT = new System.Windows.Forms.Label();
            this.lvUUID = new System.Windows.Forms.Label();
            this.lvUsername = new System.Windows.Forms.Label();
            this.lvCT = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(273, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(65, 69);
            this.button1.TabIndex = 0;
            this.button1.Text = "Signout";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(362, 165);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 40);
            this.button2.TabIndex = 1;
            this.button2.Text = "Invalidate\r\nSession";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.txtEmail);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(12, 132);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(344, 100);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Signout";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lvCT);
            this.groupBox2.Controls.Add(this.lvUsername);
            this.groupBox2.Controls.Add(this.lvUUID);
            this.groupBox2.Controls.Add(this.lvAT);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(425, 114);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Login Cache";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "AccessToken : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "UUID :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Username : ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 82);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "ClientToken : ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 34);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(49, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "Email : ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 59);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "PW : ";
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(69, 31);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(198, 21);
            this.txtEmail.TabIndex = 3;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(69, 56);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(198, 21);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // lvAT
            // 
            this.lvAT.AutoSize = true;
            this.lvAT.Location = new System.Drawing.Point(126, 27);
            this.lvAT.Name = "lvAT";
            this.lvAT.Size = new System.Drawing.Size(38, 12);
            this.lvAT.TabIndex = 4;
            this.lvAT.Text = "label7";
            // 
            // lvUUID
            // 
            this.lvUUID.AutoSize = true;
            this.lvUUID.Location = new System.Drawing.Point(126, 44);
            this.lvUUID.Name = "lvUUID";
            this.lvUUID.Size = new System.Drawing.Size(38, 12);
            this.lvUUID.TabIndex = 5;
            this.lvUUID.Text = "label8";
            // 
            // lvUsername
            // 
            this.lvUsername.AutoSize = true;
            this.lvUsername.Location = new System.Drawing.Point(126, 64);
            this.lvUsername.Name = "lvUsername";
            this.lvUsername.Size = new System.Drawing.Size(38, 12);
            this.lvUsername.TabIndex = 6;
            this.lvUsername.Text = "label9";
            // 
            // lvCT
            // 
            this.lvCT.AutoSize = true;
            this.lvCT.Location = new System.Drawing.Point(126, 82);
            this.lvCT.Name = "lvCT";
            this.lvCT.Size = new System.Drawing.Size(44, 12);
            this.lvCT.TabIndex = 7;
            this.lvCT.Text = "label10";
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 244);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Name = "Form3";
            this.Text = "Form3";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lvCT;
        private System.Windows.Forms.Label lvUsername;
        private System.Windows.Forms.Label lvUUID;
        private System.Windows.Forms.Label lvAT;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}