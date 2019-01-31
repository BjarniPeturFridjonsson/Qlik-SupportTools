namespace FreyrViewer.Ui.Splashes
{
    partial class SplashLoading
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashLoading));
            this.splashControl = new SplashControl();
            this.SuspendLayout();
            // 
            // splashControl
            // 
            this.splashControl.BackColor = System.Drawing.SystemColors.Control;
            this.splashControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splashControl.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.splashControl.Location = new System.Drawing.Point(0, 0);
            this.splashControl.Name = "splashControl";
            this.splashControl.Size = new System.Drawing.Size(244, 90);
            this.splashControl.TabIndex = 2;
            // 
            // SplashLoading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 90);
            this.Controls.Add(this.splashControl);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SplashLoading";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "SplashLoading";
            this.ResumeLayout(false);

        }

        #endregion

        private SplashControl splashControl;
    }
}