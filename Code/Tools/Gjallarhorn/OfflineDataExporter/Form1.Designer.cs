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
            this.cmdChoose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdExport
            // 
            this.cmdExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdExport.Location = new System.Drawing.Point(352, 98);
            this.cmdExport.Margin = new System.Windows.Forms.Padding(2);
            this.cmdExport.Name = "cmdExport";
            this.cmdExport.Size = new System.Drawing.Size(85, 24);
            this.cmdExport.TabIndex = 0;
            this.cmdExport.Text = "Export";
            this.toolTip1.SetToolTip(this.cmdExport, "Exports to a zip file any statistics that have not\r\npreviously been exported.");
            this.cmdExport.UseVisualStyleBackColor = true;
            this.cmdExport.Click += new System.EventHandler(this.cmdRun_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(8, 21);
            this.lblInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(58, 13);
            this.lblInfo.TabIndex = 2;
            this.lblInfo.Text = "Starting up";
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClose.Location = new System.Drawing.Point(441, 98);
            this.cmdClose.Margin = new System.Windows.Forms.Padding(2);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(85, 24);
            this.cmdClose.TabIndex = 3;
            this.cmdClose.Text = "Exit";
            this.toolTip1.SetToolTip(this.cmdClose, "Exits the program");
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.button1_Click);
            // 
            // cmdReExport
            // 
            this.cmdReExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdReExport.Location = new System.Drawing.Point(8, 98);
            this.cmdReExport.Margin = new System.Windows.Forms.Padding(2);
            this.cmdReExport.Name = "cmdReExport";
            this.cmdReExport.Size = new System.Drawing.Size(114, 24);
            this.cmdReExport.TabIndex = 4;
            this.cmdReExport.Text = "Export again";
            this.toolTip1.SetToolTip(this.cmdReExport, "Will export again the previous \r\nexported data again to a new zip file.");
            this.cmdReExport.UseVisualStyleBackColor = true;
            this.cmdReExport.Click += new System.EventHandler(this.cmdReExport_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 65);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Zip file path:";
            // 
            // txtZipPath
            // 
            this.txtZipPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtZipPath.Location = new System.Drawing.Point(75, 65);
            this.txtZipPath.Margin = new System.Windows.Forms.Padding(2);
            this.txtZipPath.Name = "txtZipPath";
            this.txtZipPath.Size = new System.Drawing.Size(422, 20);
            this.txtZipPath.TabIndex = 5;
            this.toolTip1.SetToolTip(this.txtZipPath, "The path to the exported zip file.");
            // 
            // cmdChoose
            // 
            this.cmdChoose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdChoose.Location = new System.Drawing.Point(501, 63);
            this.cmdChoose.Margin = new System.Windows.Forms.Padding(2);
            this.cmdChoose.Name = "cmdChoose";
            this.cmdChoose.Size = new System.Drawing.Size(26, 24);
            this.cmdChoose.TabIndex = 7;
            this.cmdChoose.Text = "...";
            this.toolTip1.SetToolTip(this.cmdChoose, "Lets you choose a path for the export and \r\nif you have exported opens Explorer w" +
        "ith the file selected");
            this.cmdChoose.UseVisualStyleBackColor = true;
            this.cmdChoose.Click += new System.EventHandler(this.cmdChoose_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 129);
            this.Controls.Add(this.cmdChoose);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtZipPath);
            this.Controls.Add(this.cmdReExport);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.cmdExport);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(325, 157);
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
        private System.Windows.Forms.Button cmdChoose;
    }
}

