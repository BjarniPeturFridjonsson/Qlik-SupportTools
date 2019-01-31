namespace FreyrViewer.Ui.Controls
{
    partial class SuperMessageBox
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.panContainer = new System.Windows.Forms.Panel();
            this.lblIngress = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.panContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(404, 239);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(323, 239);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 11;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // panContainer
            // 
            this.panContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panContainer.BackColor = System.Drawing.Color.White;
            this.panContainer.Controls.Add(this.lblIngress);
            this.panContainer.Controls.Add(this.lblMessage);
            this.panContainer.Location = new System.Drawing.Point(2, 0);
            this.panContainer.Name = "panContainer";
            this.panContainer.Size = new System.Drawing.Size(489, 233);
            this.panContainer.TabIndex = 14;
            // 
            // lblIngress
            // 
            this.lblIngress.AutoSize = true;
            this.lblIngress.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIngress.ForeColor = System.Drawing.Color.Blue;
            this.lblIngress.Location = new System.Drawing.Point(12, 9);
            this.lblIngress.Name = "lblIngress";
            this.lblIngress.Size = new System.Drawing.Size(62, 20);
            this.lblIngress.TabIndex = 0;
            this.lblIngress.Text = "Ingress";
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Location = new System.Drawing.Point(13, 33);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(49, 13);
            this.lblMessage.TabIndex = 1;
            this.lblMessage.Text = "message";
            // 
            // SuperMessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 274);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.panContainer);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SuperMessageBox";
            this.ShowIcon = false;
            this.Text = "SuperMessageBox";
            this.Load += new System.EventHandler(this.SuperMessageBox_Load);
            this.Resize += new System.EventHandler(this.SuperMessageBox_Resize);
            this.panContainer.ResumeLayout(false);
            this.panContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel panContainer;
        private System.Windows.Forms.Label lblIngress;
        private System.Windows.Forms.Label lblMessage;
    }
}