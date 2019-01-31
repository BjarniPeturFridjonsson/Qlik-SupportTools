namespace FreyrSenseCollector.Dialogues
{
    partial class FrmConnectionIssues
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmConnectionIssues));
            this.lblMessage = new System.Windows.Forms.Label();
            this.cmdExit = new System.Windows.Forms.Button();
            this.cmdConnectToRemoteHost = new System.Windows.Forms.Button();
            this.cmdRunOnlyLocalLogs = new System.Windows.Forms.Button();
            this.cmdShowConnectionHowTo = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblMessage.Location = new System.Drawing.Point(12, 9);
            this.lblMessage.MaximumSize = new System.Drawing.Size(500, 500);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(485, 38);
            this.lblMessage.TabIndex = 34;
            this.lblMessage.Text = "We could not connect to Qlik Sense using the credentials that this program is run" +
    "ning as.";
            // 
            // cmdExit
            // 
            this.cmdExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(225)))), ((int)(((byte)(221)))));
            this.cmdExit.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(167)))), ((int)(((byte)(41)))));
            this.cmdExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdExit.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cmdExit.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdExit.Location = new System.Drawing.Point(522, 250);
            this.cmdExit.Name = "cmdExit";
            this.cmdExit.Size = new System.Drawing.Size(110, 41);
            this.cmdExit.TabIndex = 35;
            this.cmdExit.Text = "Exit Tool";
            this.cmdExit.UseVisualStyleBackColor = false;
            this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);
            // 
            // cmdConnectToRemoteHost
            // 
            this.cmdConnectToRemoteHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(225)))), ((int)(((byte)(221)))));
            this.cmdConnectToRemoteHost.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(167)))), ((int)(((byte)(41)))));
            this.cmdConnectToRemoteHost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdConnectToRemoteHost.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cmdConnectToRemoteHost.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdConnectToRemoteHost.Location = new System.Drawing.Point(448, 69);
            this.cmdConnectToRemoteHost.Name = "cmdConnectToRemoteHost";
            this.cmdConnectToRemoteHost.Size = new System.Drawing.Size(184, 136);
            this.cmdConnectToRemoteHost.TabIndex = 37;
            this.cmdConnectToRemoteHost.Text = "Qlik Sense is located elsewhere\r\n ";
            this.cmdConnectToRemoteHost.UseVisualStyleBackColor = false;
            this.cmdConnectToRemoteHost.Click += new System.EventHandler(this.cmdConnectToRemoteHost_Click);
            // 
            // cmdRunOnlyLocalLogs
            // 
            this.cmdRunOnlyLocalLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(225)))), ((int)(((byte)(221)))));
            this.cmdRunOnlyLocalLogs.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(167)))), ((int)(((byte)(41)))));
            this.cmdRunOnlyLocalLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdRunOnlyLocalLogs.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cmdRunOnlyLocalLogs.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdRunOnlyLocalLogs.Location = new System.Drawing.Point(232, 69);
            this.cmdRunOnlyLocalLogs.Name = "cmdRunOnlyLocalLogs";
            this.cmdRunOnlyLocalLogs.Size = new System.Drawing.Size(184, 136);
            this.cmdRunOnlyLocalLogs.TabIndex = 38;
            this.cmdRunOnlyLocalLogs.Text = "Qlik Sense is not running or accessible        ";
            this.cmdRunOnlyLocalLogs.UseVisualStyleBackColor = false;
            this.cmdRunOnlyLocalLogs.Click += new System.EventHandler(this.cmdRunOnlyLocalLogs_Click);
            // 
            // cmdShowConnectionHowTo
            // 
            this.cmdShowConnectionHowTo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(225)))), ((int)(((byte)(221)))));
            this.cmdShowConnectionHowTo.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(167)))), ((int)(((byte)(41)))));
            this.cmdShowConnectionHowTo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdShowConnectionHowTo.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cmdShowConnectionHowTo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdShowConnectionHowTo.Location = new System.Drawing.Point(16, 69);
            this.cmdShowConnectionHowTo.Name = "cmdShowConnectionHowTo";
            this.cmdShowConnectionHowTo.Size = new System.Drawing.Size(184, 136);
            this.cmdShowConnectionHowTo.TabIndex = 39;
            this.cmdShowConnectionHowTo.Text = "Qlik Sense is running and on this host?\r\n\r\n";
            this.cmdShowConnectionHowTo.UseVisualStyleBackColor = false;
            this.cmdShowConnectionHowTo.Click += new System.EventHandler(this.cmdShowConnectionHowTo_Click);
            // 
            // FrmConnectionIssues
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 317);
            this.Controls.Add(this.cmdShowConnectionHowTo);
            this.Controls.Add(this.cmdRunOnlyLocalLogs);
            this.Controls.Add(this.cmdConnectToRemoteHost);
            this.Controls.Add(this.cmdExit);
            this.Controls.Add(this.lblMessage);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FrmConnectionIssues";
            this.Text = "Qlik Sense Log Reader - Can\'t connect to Sense.";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button cmdExit;
        private System.Windows.Forms.Button cmdConnectToRemoteHost;
        private System.Windows.Forms.Button cmdRunOnlyLocalLogs;
        private System.Windows.Forms.Button cmdShowConnectionHowTo;
    }
}