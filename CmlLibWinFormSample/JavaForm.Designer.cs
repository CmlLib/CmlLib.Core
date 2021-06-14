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
            this.label4 = new System.Windows.Forms.Label();
            this.txtUserJava = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.cbAutoJava = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(175, 30);
            this.label4.TabIndex = 5;
            this.label4.Text = "Input java binary file path.\r\nex) java.exe, javaw.exe";
            // 
            // txtUserJava
            // 
            this.txtUserJava.Location = new System.Drawing.Point(107, 52);
            this.txtUserJava.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtUserJava.Name = "txtUserJava";
            this.txtUserJava.Size = new System.Drawing.Size(357, 25);
            this.txtUserJava.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "Java Path : ";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(14, 130);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(450, 45);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // cbAutoJava
            // 
            this.cbAutoJava.Location = new System.Drawing.Point(12, 21);
            this.cbAutoJava.Name = "cbAutoJava";
            this.cbAutoJava.Size = new System.Drawing.Size(215, 24);
            this.cbAutoJava.TabIndex = 7;
            this.cbAutoJava.Text = "Set java path automatically";
            this.cbAutoJava.UseVisualStyleBackColor = true;
            this.cbAutoJava.CheckedChanged += new System.EventHandler(this.cbAutoJava_CheckedChanged);
            // 
            // JavaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(479, 188);
            this.Controls.Add(this.cbAutoJava);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtUserJava);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "JavaForm";
            this.Text = "JavaForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JavaForm_FormClosing);
            this.Load += new System.EventHandler(this.JavaForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.CheckBox cbAutoJava;

        #endregion

        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtUserJava;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnClose;
    }
}