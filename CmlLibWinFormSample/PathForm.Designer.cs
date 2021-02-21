namespace CmlLibWinFormSample
{
    partial class PathForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnSetDefault = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtAssets = new System.Windows.Forms.TextBox();
            this.txtLibrary = new System.Windows.Forms.TextBox();
            this.txtVersion = new System.Windows.Forms.TextBox();
            this.txtRuntime = new System.Windows.Forms.TextBox();
            this.gPaths = new System.Windows.Forms.GroupBox();
            this.btnUseDefaultAssets = new System.Windows.Forms.Button();
            this.cbEditMore = new System.Windows.Forms.CheckBox();
            this.gPaths.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Minecraft Path : ";
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(120, 22);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(582, 21);
            this.txtPath.TabIndex = 1;
            // 
            // btnSetDefault
            // 
            this.btnSetDefault.Location = new System.Drawing.Point(458, 50);
            this.btnSetDefault.Name = "btnSetDefault";
            this.btnSetDefault.Size = new System.Drawing.Size(160, 23);
            this.btnSetDefault.TabIndex = 2;
            this.btnSetDefault.Text = "Set OS Default Path";
            this.btnSetDefault.UseVisualStyleBackColor = true;
            this.btnSetDefault.Click += new System.EventHandler(this.btnSetDefault_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(120, 48);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(332, 31);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(18, 253);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(684, 44);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Save";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(52, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "Assets";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(52, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "Library";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(48, 107);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "Version";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(45, 134);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "Runtime";
            // 
            // txtAssets
            // 
            this.txtAssets.Location = new System.Drawing.Point(102, 20);
            this.txtAssets.Name = "txtAssets";
            this.txtAssets.Size = new System.Drawing.Size(576, 21);
            this.txtAssets.TabIndex = 10;
            // 
            // txtLibrary
            // 
            this.txtLibrary.Location = new System.Drawing.Point(102, 77);
            this.txtLibrary.Name = "txtLibrary";
            this.txtLibrary.Size = new System.Drawing.Size(576, 21);
            this.txtLibrary.TabIndex = 11;
            // 
            // txtVersion
            // 
            this.txtVersion.Location = new System.Drawing.Point(102, 104);
            this.txtVersion.Name = "txtVersion";
            this.txtVersion.Size = new System.Drawing.Size(576, 21);
            this.txtVersion.TabIndex = 12;
            // 
            // txtRuntime
            // 
            this.txtRuntime.Location = new System.Drawing.Point(102, 131);
            this.txtRuntime.Name = "txtRuntime";
            this.txtRuntime.Size = new System.Drawing.Size(576, 21);
            this.txtRuntime.TabIndex = 13;
            // 
            // gPaths
            // 
            this.gPaths.Controls.Add(this.btnUseDefaultAssets);
            this.gPaths.Controls.Add(this.label3);
            this.gPaths.Controls.Add(this.txtRuntime);
            this.gPaths.Controls.Add(this.label4);
            this.gPaths.Controls.Add(this.txtVersion);
            this.gPaths.Controls.Add(this.label5);
            this.gPaths.Controls.Add(this.txtLibrary);
            this.gPaths.Controls.Add(this.label6);
            this.gPaths.Controls.Add(this.txtAssets);
            this.gPaths.Enabled = false;
            this.gPaths.Location = new System.Drawing.Point(18, 79);
            this.gPaths.Name = "gPaths";
            this.gPaths.Size = new System.Drawing.Size(684, 168);
            this.gPaths.TabIndex = 15;
            this.gPaths.TabStop = false;
            this.gPaths.Text = "Paths";
            // 
            // btnUseDefaultAssets
            // 
            this.btnUseDefaultAssets.Location = new System.Drawing.Point(238, 47);
            this.btnUseDefaultAssets.Name = "btnUseDefaultAssets";
            this.btnUseDefaultAssets.Size = new System.Drawing.Size(300, 24);
            this.btnUseDefaultAssets.TabIndex = 15;
            this.btnUseDefaultAssets.Text = "Use default assets path (download speed up)\r\n";
            this.btnUseDefaultAssets.UseVisualStyleBackColor = true;
            this.btnUseDefaultAssets.Click += new System.EventHandler(this.btnUseDefaultAssets_Click);
            // 
            // cbEditMore
            // 
            this.cbEditMore.AutoSize = true;
            this.cbEditMore.Location = new System.Drawing.Point(624, 57);
            this.cbEditMore.Name = "cbEditMore";
            this.cbEditMore.Size = new System.Drawing.Size(78, 16);
            this.cbEditMore.TabIndex = 16;
            this.cbEditMore.Text = "Edit more";
            this.cbEditMore.UseVisualStyleBackColor = true;
            this.cbEditMore.CheckedChanged += new System.EventHandler(this.cbEditMore_CheckedChanged);
            // 
            // PathForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 308);
            this.Controls.Add(this.cbEditMore);
            this.Controls.Add(this.gPaths);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnSetDefault);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.label1);
            this.Name = "PathForm";
            this.Text = "InitializingForm";
            this.Load += new System.EventHandler(this.InitializingForm_Load);
            this.gPaths.ResumeLayout(false);
            this.gPaths.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button btnSetDefault;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtAssets;
        private System.Windows.Forms.TextBox txtLibrary;
        private System.Windows.Forms.TextBox txtVersion;
        private System.Windows.Forms.TextBox txtRuntime;
        private System.Windows.Forms.GroupBox gPaths;
        private System.Windows.Forms.Button btnUseDefaultAssets;
        private System.Windows.Forms.CheckBox cbEditMore;
    }
}