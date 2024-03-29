using System.ComponentModel;

namespace CmlLibWinFormSample
{
    partial class VersionSortOptionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.txtTypeOrder = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbPropertyOrder = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbCustomAsRelease = new System.Windows.Forms.CheckBox();
            this.cbAscending = new System.Windows.Forms.CheckBox();
            this.cbTypeClassification = new System.Windows.Forms.CheckBox();
            this.btnPreset1 = new System.Windows.Forms.Button();
            this.btnPreset2 = new System.Windows.Forms.Button();
            this.btnPreset3 = new System.Windows.Forms.Button();
            this.btnPreset4 = new System.Windows.Forms.Button();
            this.listPreview = new System.Windows.Forms.ListBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtTypeOrder
            // 
            this.txtTypeOrder.Location = new System.Drawing.Point(12, 35);
            this.txtTypeOrder.Name = "txtTypeOrder";
            this.txtTypeOrder.Size = new System.Drawing.Size(246, 90);
            this.txtTypeOrder.TabIndex = 0;
            this.txtTypeOrder.Text = "";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Type order : ";
            // 
            // cbPropertyOrder
            // 
            this.cbPropertyOrder.FormattingEnabled = true;
            this.cbPropertyOrder.Items.AddRange(new object[] { "Name", "ReleaseDate" });
            this.cbPropertyOrder.Location = new System.Drawing.Point(83, 131);
            this.cbPropertyOrder.Name = "cbPropertyOrder";
            this.cbPropertyOrder.Size = new System.Drawing.Size(175, 23);
            this.cbPropertyOrder.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 23);
            this.label3.TabIndex = 4;
            this.label3.Text = "OrderBy:";
            // 
            // cbCustomAsRelease
            // 
            this.cbCustomAsRelease.Location = new System.Drawing.Point(13, 160);
            this.cbCustomAsRelease.Name = "cbCustomAsRelease";
            this.cbCustomAsRelease.Size = new System.Drawing.Size(245, 24);
            this.cbCustomAsRelease.TabIndex = 5;
            this.cbCustomAsRelease.Text = "Sort Custom version as Release version";
            this.cbCustomAsRelease.UseVisualStyleBackColor = true;
            // 
            // cbAscending
            // 
            this.cbAscending.Location = new System.Drawing.Point(13, 181);
            this.cbAscending.Name = "cbAscending";
            this.cbAscending.Size = new System.Drawing.Size(245, 24);
            this.cbAscending.TabIndex = 6;
            this.cbAscending.Text = "Ascending order";
            this.cbAscending.UseVisualStyleBackColor = true;
            // 
            // cbTypeClassification
            // 
            this.cbTypeClassification.Location = new System.Drawing.Point(13, 205);
            this.cbTypeClassification.Name = "cbTypeClassification";
            this.cbTypeClassification.Size = new System.Drawing.Size(245, 24);
            this.cbTypeClassification.TabIndex = 7;
            this.cbTypeClassification.Text = "Type classification";
            this.cbTypeClassification.UseVisualStyleBackColor = true;
            // 
            // btnPreset1
            // 
            this.btnPreset1.Location = new System.Drawing.Point(12, 235);
            this.btnPreset1.Name = "btnPreset1";
            this.btnPreset1.Size = new System.Drawing.Size(113, 26);
            this.btnPreset1.TabIndex = 8;
            this.btnPreset1.Text = "Preset1";
            this.btnPreset1.UseVisualStyleBackColor = true;
            this.btnPreset1.Click += new System.EventHandler(this.btnPreset1_Click);
            // 
            // btnPreset2
            // 
            this.btnPreset2.Location = new System.Drawing.Point(145, 235);
            this.btnPreset2.Name = "btnPreset2";
            this.btnPreset2.Size = new System.Drawing.Size(113, 26);
            this.btnPreset2.TabIndex = 9;
            this.btnPreset2.Text = "Preset2";
            this.btnPreset2.UseVisualStyleBackColor = true;
            this.btnPreset2.Click += new System.EventHandler(this.btnPreset2_Click);
            // 
            // btnPreset3
            // 
            this.btnPreset3.Location = new System.Drawing.Point(12, 267);
            this.btnPreset3.Name = "btnPreset3";
            this.btnPreset3.Size = new System.Drawing.Size(113, 26);
            this.btnPreset3.TabIndex = 10;
            this.btnPreset3.Text = "Preset3";
            this.btnPreset3.UseVisualStyleBackColor = true;
            this.btnPreset3.Click += new System.EventHandler(this.btnPreset3_Click);
            // 
            // btnPreset4
            // 
            this.btnPreset4.Location = new System.Drawing.Point(145, 267);
            this.btnPreset4.Name = "btnPreset4";
            this.btnPreset4.Size = new System.Drawing.Size(113, 26);
            this.btnPreset4.TabIndex = 11;
            this.btnPreset4.Text = "Preset4";
            this.btnPreset4.UseVisualStyleBackColor = true;
            this.btnPreset4.Click += new System.EventHandler(this.btnPreset4_Click);
            // 
            // listPreview
            // 
            this.listPreview.FormattingEnabled = true;
            this.listPreview.ItemHeight = 15;
            this.listPreview.Location = new System.Drawing.Point(275, 35);
            this.listPreview.Name = "listPreview";
            this.listPreview.Size = new System.Drawing.Size(281, 289);
            this.listPreview.TabIndex = 12;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(159, 350);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(245, 48);
            this.btnClose.TabIndex = 13;
            this.btnClose.Text = "Save and Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(13, 299);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(245, 33);
            this.btnPreview.TabIndex = 14;
            this.btnPreview.Text = "Preview";
            this.btnPreview.UseVisualStyleBackColor = true;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // VersionSortOptionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 410);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.listPreview);
            this.Controls.Add(this.btnPreset4);
            this.Controls.Add(this.btnPreset3);
            this.Controls.Add(this.btnPreset2);
            this.Controls.Add(this.btnPreset1);
            this.Controls.Add(this.cbTypeClassification);
            this.Controls.Add(this.cbAscending);
            this.Controls.Add(this.cbCustomAsRelease);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbPropertyOrder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTypeOrder);
            this.Name = "VersionSortOptionForm";
            this.Text = "VersionSortOptionForm";
            this.Load += new System.EventHandler(this.VersionSortOptionForm_Load);
            this.ResumeLayout(false);
        }
        private System.Windows.Forms.CheckBox cbTypeClassification;
        private System.Windows.Forms.ComboBox cbPropertyOrder;
        private System.Windows.Forms.CheckBox cbAscending;
        private System.Windows.Forms.CheckBox cbCustomAsRelease;

        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnPreset1;
        private System.Windows.Forms.Button btnPreset2;
        private System.Windows.Forms.Button btnPreset3;
        private System.Windows.Forms.Button btnPreset4;

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        
        private System.Windows.Forms.RichTextBox txtTypeOrder;

        private System.Windows.Forms.ListBox listPreview;

        #endregion
    }
}