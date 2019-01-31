namespace FreyrViewer.Ui.Controls
{
    partial class FrmExceptionDialogue
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
            this.cmdDetails = new System.Windows.Forms.Button();
            this.txtDetails = new System.Windows.Forms.TextBox();
            this.cmdOk = new System.Windows.Forms.Button();
            this.panContainer = new System.Windows.Forms.Panel();
            this.lblIngress = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.lblMessage = new System.Windows.Forms.Label();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdCopyToClipboard = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdDetails
            // 
            this.cmdDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdDetails.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdDetails.Location = new System.Drawing.Point(12, 111);
            this.cmdDetails.Name = "cmdDetails";
            this.cmdDetails.Size = new System.Drawing.Size(75, 23);
            this.cmdDetails.TabIndex = 3;
            this.cmdDetails.Text = "Details";
            this.cmdDetails.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdDetails.UseVisualStyleBackColor = true;
            this.cmdDetails.Click += new System.EventHandler(this.CmdDetails_Click);
            // 
            // txtDetails
            // 
            this.txtDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDetails.BackColor = System.Drawing.SystemColors.Control;
            this.txtDetails.Location = new System.Drawing.Point(12, 103);
            this.txtDetails.Multiline = true;
            this.txtDetails.Name = "txtDetails";
            this.txtDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDetails.Size = new System.Drawing.Size(387, 2);
            this.txtDetails.TabIndex = 2;
            this.txtDetails.Visible = false;
            this.txtDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtDetails_KeyDown);
            // 
            // cmdOk
            // 
            this.cmdOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.cmdOk.Location = new System.Drawing.Point(19, 0);
            this.cmdOk.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(75, 23);
            this.cmdOk.TabIndex = 5;
            this.cmdOk.Text = "OK";
            this.cmdOk.UseVisualStyleBackColor = true;
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // panContainer
            // 
            this.panContainer.BackColor = System.Drawing.Color.White;
            this.panContainer.Controls.Add(this.lblIngress);
            this.panContainer.Controls.Add(this.pictureBox);
            this.panContainer.Controls.Add(this.lblMessage);
            this.panContainer.Dock = System.Windows.Forms.DockStyle.Top;
            this.panContainer.Location = new System.Drawing.Point(0, 0);
            this.panContainer.Name = "panContainer";
            this.panContainer.Size = new System.Drawing.Size(411, 97);
            this.panContainer.TabIndex = 1;
            // 
            // lblIngress
            // 
            this.lblIngress.AutoSize = true;
            this.lblIngress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIngress.ForeColor = System.Drawing.Color.Blue;
            this.lblIngress.Location = new System.Drawing.Point(82, 10);
            this.lblIngress.Name = "lblIngress";
            this.lblIngress.Size = new System.Drawing.Size(62, 20);
            this.lblIngress.TabIndex = 0;
            this.lblIngress.Text = "Ingress";
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(7, 4);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(64, 64);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox.TabIndex = 8;
            this.pictureBox.TabStop = false;
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(83, 34);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(49, 13);
            this.lblMessage.TabIndex = 1;
            this.lblMessage.Text = "message";
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(97, 0);
            this.cmdCancel.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 0;
            this.cmdCancel.Text = "Cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdCopyToClipboard
            // 
            this.cmdCopyToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdCopyToClipboard.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cmdCopyToClipboard.Location = new System.Drawing.Point(93, 111);
            this.cmdCopyToClipboard.Name = "cmdCopyToClipboard";
            this.cmdCopyToClipboard.Size = new System.Drawing.Size(104, 23);
            this.cmdCopyToClipboard.TabIndex = 4;
            this.cmdCopyToClipboard.Text = "Copy to clipboard";
            this.cmdCopyToClipboard.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.cmdCopyToClipboard.UseVisualStyleBackColor = true;
            this.cmdCopyToClipboard.Click += new System.EventHandler(this.cmdCopyToClipboard_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.cmdCancel);
            this.flowLayoutPanel1.Controls.Add(this.cmdOk);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(230, 111);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(172, 26);
            this.flowLayoutPanel1.TabIndex = 6;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 97);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(411, 1);
            this.panel1.TabIndex = 7;
            // 
            // FrmExceptionDialogue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(411, 146);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.panContainer);
            this.Controls.Add(this.txtDetails);
            this.Controls.Add(this.cmdCopyToClipboard);
            this.Controls.Add(this.cmdDetails);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(427, 185);
            this.Name = "FrmExceptionDialogue";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Proactive Desktop";
            this.Load += new System.EventHandler(this.FrmExceptionViewer_Load);
            this.Resize += new System.EventHandler(this.FrmExceptionViewer_Resize);
            this.panContainer.ResumeLayout(false);
            this.panContainer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdDetails;
        private System.Windows.Forms.TextBox txtDetails;
        private System.Windows.Forms.Button cmdOk;
        private System.Windows.Forms.Panel panContainer;
        private System.Windows.Forms.Label lblIngress;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdCopyToClipboard;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
    }
}