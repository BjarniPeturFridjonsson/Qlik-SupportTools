namespace FreyrViewer.Ui.MdiForms
{
    partial class FrmBaseForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmBaseForm));
            this.ctrlSplash = new FreyrViewer.Ui.Splashes.SplashControl();
            this.SuspendLayout();
            // 
            // ctrlSplash
            // 
            this.ctrlSplash.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ctrlSplash.BackColor = System.Drawing.SystemColors.Control;
            this.ctrlSplash.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.ctrlSplash.Location = new System.Drawing.Point(173, 132);
            this.ctrlSplash.Name = "ctrlSplash";
            this.ctrlSplash.Size = new System.Drawing.Size(244, 90);
            this.ctrlSplash.TabIndex = 0;
            this.ctrlSplash.Visible = false;
            // 
            // FrmBaseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 413);
            this.Controls.Add(this.ctrlSplash);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmBaseForm";
            this.Text = "FrmBase";
            this.SizeChanged += new System.EventHandler(this.FrmBaseForm_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private Splashes.SplashControl ctrlSplash;
    }
}