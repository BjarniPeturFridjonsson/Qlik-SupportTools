namespace FreyrViewer.Ui.Splashes
{
    sealed partial class SplashControl
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
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.textLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.Image = global::FreyrViewer.Properties.Resources.load;
            this.pictureBox.Location = new System.Drawing.Point(12, 53);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(220, 19);
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // textLabel
            // 
            this.textLabel.Location = new System.Drawing.Point(12, 23);
            this.textLabel.Name = "textLabel";
            this.textLabel.Size = new System.Drawing.Size(220, 13);
            this.textLabel.TabIndex = 3;
            this.textLabel.Text = "Loading...";
            this.textLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // SplashControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textLabel);
            this.Controls.Add(this.pictureBox);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.Name = "SplashControl";
            this.Size = new System.Drawing.Size(244, 90);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label textLabel;
    }
}
