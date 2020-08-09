namespace CmlLibWinFormSample
{
    partial class JavaForm
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
            this.rbAutoJava = new System.Windows.Forms.RadioButton();
            this.rbUserJava = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.txtJavaDirectory = new System.Windows.Forms.TextBox();
            this.gAutoJava = new System.Windows.Forms.GroupBox();
            this.gUserJava = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUserJava = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.gAutoJava.SuspendLayout();
            this.gUserJava.SuspendLayout();
            this.SuspendLayout();
            // 
            // rbAutoJava
            // 
            this.rbAutoJava.AutoSize = true;
            this.rbAutoJava.Checked = true;
            this.rbAutoJava.Location = new System.Drawing.Point(12, 12);
            this.rbAutoJava.Name = "rbAutoJava";
            this.rbAutoJava.Size = new System.Drawing.Size(283, 16);
            this.rbAutoJava.TabIndex = 0;
            this.rbAutoJava.TabStop = true;
            this.rbAutoJava.Text = "Check Java and Download Java automatically";
            this.rbAutoJava.UseVisualStyleBackColor = true;
            this.rbAutoJava.CheckedChanged += new System.EventHandler(this.rbAutoJava_CheckedChanged);
            // 
            // rbUserJava
            // 
            this.rbUserJava.AutoSize = true;
            this.rbUserJava.Location = new System.Drawing.Point(12, 139);
            this.rbUserJava.Name = "rbUserJava";
            this.rbUserJava.Size = new System.Drawing.Size(96, 16);
            this.rbUserJava.TabIndex = 1;
            this.rbUserJava.Text = "Set java path";
            this.rbUserJava.UseVisualStyleBackColor = true;
            this.rbUserJava.CheckedChanged += new System.EventHandler(this.rbUserJava_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Java Directory : ";
            // 
            // txtJavaDirectory
            // 
            this.txtJavaDirectory.Location = new System.Drawing.Point(117, 20);
            this.txtJavaDirectory.Name = "txtJavaDirectory";
            this.txtJavaDirectory.Size = new System.Drawing.Size(258, 21);
            this.txtJavaDirectory.TabIndex = 3;
            // 
            // gAutoJava
            // 
            this.gAutoJava.Controls.Add(this.label2);
            this.gAutoJava.Controls.Add(this.txtJavaDirectory);
            this.gAutoJava.Controls.Add(this.label1);
            this.gAutoJava.Location = new System.Drawing.Point(12, 34);
            this.gAutoJava.Name = "gAutoJava";
            this.gAutoJava.Size = new System.Drawing.Size(394, 88);
            this.gAutoJava.TabIndex = 4;
            this.gAutoJava.TabStop = false;
            this.gAutoJava.Text = "MJava";
            // 
            // gUserJava
            // 
            this.gUserJava.Controls.Add(this.label4);
            this.gUserJava.Controls.Add(this.txtUserJava);
            this.gUserJava.Controls.Add(this.label3);
            this.gUserJava.Enabled = false;
            this.gUserJava.Location = new System.Drawing.Point(12, 161);
            this.gUserJava.Name = "gUserJava";
            this.gUserJava.Size = new System.Drawing.Size(394, 90);
            this.gUserJava.TabIndex = 5;
            this.gUserJava.TabStop = false;
            this.gUserJava.Text = "Your Java";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(277, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "Launcher will check the existence of JRE and\r\ndownload JRE into this directory if" +
    " it is not exist.\r\n";
            // 
            // txtUserJava
            // 
            this.txtUserJava.Location = new System.Drawing.Point(117, 20);
            this.txtUserJava.Name = "txtUserJava";
            this.txtUserJava.Size = new System.Drawing.Size(258, 21);
            this.txtUserJava.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "Java Path : ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(39, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(150, 24);
            this.label4.TabIndex = 5;
            this.label4.Text = "Input java binary file path.\r\nex) java.exe, javaw.exe";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(12, 257);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(394, 36);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // JavaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 299);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.gUserJava);
            this.Controls.Add(this.gAutoJava);
            this.Controls.Add(this.rbUserJava);
            this.Controls.Add(this.rbAutoJava);
            this.Name = "JavaForm";
            this.Text = "JavaForm";
            this.Load += new System.EventHandler(this.JavaForm_Load);
            this.gAutoJava.ResumeLayout(false);
            this.gAutoJava.PerformLayout();
            this.gUserJava.ResumeLayout(false);
            this.gUserJava.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rbAutoJava;
        private System.Windows.Forms.RadioButton rbUserJava;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtJavaDirectory;
        private System.Windows.Forms.GroupBox gAutoJava;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox gUserJava;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtUserJava;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnClose;
    }
}