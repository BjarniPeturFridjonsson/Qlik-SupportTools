namespace FreyrViewer.Ui.Controls.TextPreview
{
    partial class PreviewText
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panPreview = new System.Windows.Forms.Panel();
            this.panPreviewTextPadding = new System.Windows.Forms.Panel();
            this.txtPreview = new System.Windows.Forms.TextBox();
            this.panPreview.SuspendLayout();
            this.panPreviewTextPadding.SuspendLayout();
            this.SuspendLayout();
            // 
            // panPreview
            // 
            this.panPreview.BackColor = System.Drawing.Color.White;
            this.panPreview.Controls.Add(this.panPreviewTextPadding);
            this.panPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panPreview.ForeColor = System.Drawing.Color.Coral;
            this.panPreview.Location = new System.Drawing.Point(0, 0);
            this.panPreview.Name = "panPreview";
            this.panPreview.Padding = new System.Windows.Forms.Padding(3);
            this.panPreview.Size = new System.Drawing.Size(552, 150);
            this.panPreview.TabIndex = 6;
            // 
            // panPreviewTextPadding
            // 
            this.panPreviewTextPadding.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panPreviewTextPadding.Controls.Add(this.txtPreview);
            this.panPreviewTextPadding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panPreviewTextPadding.ForeColor = System.Drawing.Color.Coral;
            this.panPreviewTextPadding.Location = new System.Drawing.Point(3, 3);
            this.panPreviewTextPadding.Name = "panPreviewTextPadding";
            this.panPreviewTextPadding.Padding = new System.Windows.Forms.Padding(4);
            this.panPreviewTextPadding.Size = new System.Drawing.Size(546, 144);
            this.panPreviewTextPadding.TabIndex = 6;
            // 
            // txtPreview
            // 
            this.txtPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtPreview.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPreview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.txtPreview.Location = new System.Drawing.Point(4, 4);
            this.txtPreview.Multiline = true;
            this.txtPreview.Name = "txtPreview";
            this.txtPreview.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtPreview.Size = new System.Drawing.Size(538, 136);
            this.txtPreview.TabIndex = 0;
            this.txtPreview.Text = " ";
            this.txtPreview.WordWrap = false;
            // 
            // PreviewTextDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panPreview);
            this.Name = "PreviewTextDisplay";
            this.Size = new System.Drawing.Size(552, 150);
            this.VisibleChanged += new System.EventHandler(this.PreviewTextDisplay_VisibleChanged);
            this.panPreview.ResumeLayout(false);
            this.panPreviewTextPadding.ResumeLayout(false);
            this.panPreviewTextPadding.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panPreview;
        private System.Windows.Forms.Panel panPreviewTextPadding;
        private System.Windows.Forms.TextBox txtPreview;
    }
}
