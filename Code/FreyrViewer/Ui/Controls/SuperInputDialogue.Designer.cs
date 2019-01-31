namespace FreyrViewer.Ui.Controls
{
    partial class SuperInputDialogue
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblPrompt = new System.Windows.Forms.Label();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.panCboControls = new System.Windows.Forms.Panel();
            this.lblForCboTextbox = new System.Windows.Forms.Label();
            this.txtCboInput = new System.Windows.Forms.TextBox();
            this.lblCbo = new System.Windows.Forms.Label();
            this.cboInput = new System.Windows.Forms.ComboBox();
            this.panCboControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(231, 243);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(312, 243);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblPrompt
            // 
            this.lblPrompt.Location = new System.Drawing.Point(12, 9);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new System.Drawing.Size(375, 63);
            this.lblPrompt.TabIndex = 0;
            this.lblPrompt.Text = "(prompt message goes here)";
            // 
            // txtInput
            // 
            this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInput.Location = new System.Drawing.Point(15, 75);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(372, 20);
            this.txtInput.TabIndex = 8;
            // 
            // panCboControls
            // 
            this.panCboControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panCboControls.Controls.Add(this.lblForCboTextbox);
            this.panCboControls.Controls.Add(this.txtCboInput);
            this.panCboControls.Controls.Add(this.lblCbo);
            this.panCboControls.Controls.Add(this.cboInput);
            this.panCboControls.Location = new System.Drawing.Point(0, 101);
            this.panCboControls.Name = "panCboControls";
            this.panCboControls.Size = new System.Drawing.Size(387, 59);
            this.panCboControls.TabIndex = 9;
            // 
            // lblForCboTextbox
            // 
            this.lblForCboTextbox.AutoSize = true;
            this.lblForCboTextbox.Location = new System.Drawing.Point(5, 9);
            this.lblForCboTextbox.Name = "lblForCboTextbox";
            this.lblForCboTextbox.Size = new System.Drawing.Size(59, 13);
            this.lblForCboTextbox.TabIndex = 14;
            this.lblForCboTextbox.Text = "Promt msg:";
            // 
            // txtCboInput
            // 
            this.txtCboInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCboInput.Location = new System.Drawing.Point(70, 6);
            this.txtCboInput.Name = "txtCboInput";
            this.txtCboInput.Size = new System.Drawing.Size(312, 20);
            this.txtCboInput.TabIndex = 12;
            // 
            // lblCbo
            // 
            this.lblCbo.AutoSize = true;
            this.lblCbo.Location = new System.Drawing.Point(5, 35);
            this.lblCbo.Name = "lblCbo";
            this.lblCbo.Size = new System.Drawing.Size(59, 13);
            this.lblCbo.TabIndex = 15;
            this.lblCbo.Text = "Promt msg:";
            // 
            // cboInput
            // 
            this.cboInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboInput.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInput.FormattingEnabled = true;
            this.cboInput.Location = new System.Drawing.Point(70, 32);
            this.cboInput.Name = "cboInput";
            this.cboInput.Size = new System.Drawing.Size(312, 21);
            this.cboInput.TabIndex = 13;
            // 
            // SuperInputDialogue
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(399, 278);
            this.ControlBox = false;
            this.Controls.Add(this.panCboControls);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.lblPrompt);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SuperInputDialogue";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PromptDialog";
            this.Load += new System.EventHandler(this.PromptDialog_Load);
            this.Shown += new System.EventHandler(this.SuperInputDialogue_Shown);
            this.panCboControls.ResumeLayout(false);
            this.panCboControls.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblPrompt;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Panel panCboControls;
        private System.Windows.Forms.Label lblForCboTextbox;
        private System.Windows.Forms.TextBox txtCboInput;
        private System.Windows.Forms.Label lblCbo;
        private System.Windows.Forms.ComboBox cboInput;
    }
}