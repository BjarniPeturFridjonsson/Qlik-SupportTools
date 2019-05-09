namespace OfflineDataExporter
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.cmdExport = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.cmdClose = new System.Windows.Forms.Button();
            this.cmdReExport = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtZipPath = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // cmdExport
            // 
            this.cmdExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdExport.Location = new System.Drawing.Point(469, 121);
            this.cmdExport.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmdExport.Name = "cmdExport";
            this.cmdExport.Size = new System.Drawing.Size(113, 29);
            this.cmdExport.TabIndex = 0;
            this.cmdExport.Text = "Export";
            this.toolTip1.SetToolTip(this.cmdExport, "Exports to a zip file any statistics that have not\r\npreviously been exported.");
            this.cmdExport.UseVisualStyleBackColor = true;
            this.cmdExport.Click += new System.EventHandler(this.cmdRun_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(11, 26);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(77, 17);
            this.lblInfo.TabIndex = 2;
            this.lblInfo.Text = "Starting up";
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClose.Location = new System.Drawing.Point(588, 121);
            this.cmdClose.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(113, 29);
            this.cmdClose.TabIndex = 3;
            this.cmdClose.Text = "Exit";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmdReExport
            // 
            this.cmdReExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdReExport.Location = new System.Drawing.Point(11, 121);
            this.cmdReExport.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cmdReExport.Name = "cmdReExport";
            this.cmdReExport.Size = new System.Drawing.Size(152, 29);
            this.cmdReExport.TabIndex = 4;
            this.cmdReExport.Text = "Export again";
            this.toolTip1.SetToolTip(this.cmdReExport, "Will export again the previous \r\nexported data again to a new zip file.");
            this.cmdReExport.UseVisualStyleBackColor = true;
            this.cmdReExport.Click += new System.EventHandler(this.cmdReExport_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 17);
            this.label1.TabIndex = 6;
            this.label1.Text = "Zip file path:";
            // 
            // txtZipPath
            // 
            this.txtZipPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtZipPath.Location = new System.Drawing.Point(100, 80);
            this.txtZipPath.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtZipPath.Name = "txtZipPath";
            this.txtZipPath.Size = new System.Drawing.Size(600, 22);
            this.txtZipPath.TabIndex = 5;
            this.toolTip1.SetToolTip(this.txtZipPath, "The path to the exported zip file.");
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 159);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtZipPath);
            this.Controls.Add(this.cmdReExport);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.cmdExport);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FrmMain";
            this.Text = "Export Offline Statistics";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdExport;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.Button cmdReExport;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtZipPath;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

