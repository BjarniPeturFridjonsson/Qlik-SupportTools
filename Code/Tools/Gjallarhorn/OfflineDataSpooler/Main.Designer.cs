﻿namespace OfflineDataSpooler
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.label1 = new System.Windows.Forms.Label();
            this.txtZipPath = new System.Windows.Forms.TextBox();
            this.cmdGetZip = new System.Windows.Forms.Button();
            this.cmdClose = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.cmdExport = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 20);
            this.label1.TabIndex = 12;
            this.label1.Text = "Zip file path:";
            // 
            // txtZipPath
            // 
            this.txtZipPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtZipPath.Location = new System.Drawing.Point(118, 92);
            this.txtZipPath.Name = "txtZipPath";
            this.txtZipPath.Size = new System.Drawing.Size(766, 26);
            this.txtZipPath.TabIndex = 11;
            // 
            // cmdGetZip
            // 
            this.cmdGetZip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdGetZip.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdGetZip.Location = new System.Drawing.Point(891, 83);
            this.cmdGetZip.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.cmdGetZip.Name = "cmdGetZip";
            this.cmdGetZip.Size = new System.Drawing.Size(46, 42);
            this.cmdGetZip.TabIndex = 10;
            this.cmdGetZip.Text = "...";
            this.cmdGetZip.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.cmdGetZip.UseVisualStyleBackColor = true;
            this.cmdGetZip.Click += new System.EventHandler(this.cmdGetZip_Click);
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClose.Location = new System.Drawing.Point(810, 866);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(128, 37);
            this.cmdClose.TabIndex = 9;
            this.cmdClose.Text = "Exit";
            this.cmdClose.UseVisualStyleBackColor = true;
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(18, 7);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(87, 20);
            this.lblInfo.TabIndex = 8;
            this.lblInfo.Text = "Starting up";
            // 
            // cmdExport
            // 
            this.cmdExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdExport.Location = new System.Drawing.Point(676, 866);
            this.cmdExport.Name = "cmdExport";
            this.cmdExport.Size = new System.Drawing.Size(128, 37);
            this.cmdExport.TabIndex = 7;
            this.cmdExport.Text = "Export";
            this.cmdExport.UseVisualStyleBackColor = true;
            this.cmdExport.Click += new System.EventHandler(this.cmdExport_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // txtResults
            // 
            this.txtResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtResults.Location = new System.Drawing.Point(118, 148);
            this.txtResults.Multiline = true;
            this.txtResults.Name = "txtResults";
            this.txtResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResults.Size = new System.Drawing.Size(818, 702);
            this.txtResults.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 151);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 20);
            this.label2.TabIndex = 14;
            this.label2.Text = "Results:";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(951, 917);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtZipPath);
            this.Controls.Add(this.cmdGetZip);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.cmdExport);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.Text = "Offline data spooler";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtZipPath;
        private System.Windows.Forms.Button cmdGetZip;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button cmdExport;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox txtResults;
        private System.Windows.Forms.Label label2;
    }
}

