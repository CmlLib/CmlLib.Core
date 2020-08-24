namespace CmlLibWinFormSample
{
    partial class ForgeInstall
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMC = new System.Windows.Forms.TextBox();
            this.txtForge = new System.Windows.Forms.TextBox();
            this.btnInstall = new System.Windows.Forms.Button();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.lbStatus = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Minecraft Version : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Forge Version : ";
            // 
            // txtMC
            // 
            this.txtMC.Location = new System.Drawing.Point(141, 15);
            this.txtMC.Name = "txtMC";
            this.txtMC.Size = new System.Drawing.Size(162, 21);
            this.txtMC.TabIndex = 2;
            // 
            // txtForge
            // 
            this.txtForge.Location = new System.Drawing.Point(141, 42);
            this.txtForge.Name = "txtForge";
            this.txtForge.Size = new System.Drawing.Size(162, 21);
            this.txtForge.TabIndex = 3;
            // 
            // btnInstall
            // 
            this.btnInstall.Location = new System.Drawing.Point(309, 15);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(75, 48);
            this.btnInstall.TabIndex = 4;
            this.btnInstall.Text = "Install";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(21, 97);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(363, 23);
            this.pbProgress.TabIndex = 5;
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Location = new System.Drawing.Point(19, 123);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(38, 12);
            this.lbStatus.TabIndex = 6;
            this.lbStatus.Text = "label3";
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(21, 142);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.txtLog.Size = new System.Drawing.Size(363, 296);
            this.txtLog.TabIndex = 7;
            this.txtLog.Text = "";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(139, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(140, 24);
            this.label4.TabIndex = 8;
            this.label4.Text = "ex) 1.12.2 / 14.23.5.2768\r\n      1.16.2 / 33.0.5";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // ForgeInstall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 450);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.btnInstall);
            this.Controls.Add(this.txtForge);
            this.Controls.Add(this.txtMC);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ForgeInstall";
            this.Text = "ForgeInstall";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMC;
        private System.Windows.Forms.TextBox txtForge;
        private System.Windows.Forms.Button btnInstall;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Timer timer1;
    }
}