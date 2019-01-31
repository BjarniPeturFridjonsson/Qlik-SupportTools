namespace FreyrSenseCollector.Dialogues
{
    partial class FrmConnectionIssuesInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmConnectionIssuesInfo));
            this.cmdExit = new System.Windows.Forms.Button();
            this.CtrlRichTxtBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // cmdExit
            // 
            this.cmdExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(225)))), ((int)(((byte)(221)))));
            this.cmdExit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(167)))), ((int)(((byte)(41)))));
            this.cmdExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdExit.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cmdExit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdExit.Location = new System.Drawing.Point(560, 601);
            this.cmdExit.Name = "cmdExit";
            this.cmdExit.Size = new System.Drawing.Size(110, 41);
            this.cmdExit.TabIndex = 37;
            this.cmdExit.Text = "Close";
            this.cmdExit.UseVisualStyleBackColor = false;
            this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);
            // 
            // CtrlRichTxtBox
            // 
            this.CtrlRichTxtBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CtrlRichTxtBox.BackColor = System.Drawing.SystemColors.Control;
            this.CtrlRichTxtBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CtrlRichTxtBox.Location = new System.Drawing.Point(12, 12);
            this.CtrlRichTxtBox.Name = "CtrlRichTxtBox";
            this.CtrlRichTxtBox.ReadOnly = true;
            this.CtrlRichTxtBox.Size = new System.Drawing.Size(658, 583);
            this.CtrlRichTxtBox.TabIndex = 38;
            this.CtrlRichTxtBox.Text = "";
            // 
            // FrmConnectionIssuesInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(682, 654);
            this.Controls.Add(this.CtrlRichTxtBox);
            this.Controls.Add(this.cmdExit);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmConnectionIssuesInfo";
            this.Text = "Qlik Sense Log Reader - How to connect to Qlik Sense";
            this.Load += new System.EventHandler(this.FrmConnectionIssues_Info_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdExit;
        private System.Windows.Forms.RichTextBox CtrlRichTxtBox;
    }
}