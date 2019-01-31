namespace FreyrViewer.Ui
{
    partial class FrmAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAbout));
            this.lblVersionInfo = new System.Windows.Forms.Label();
            this.lblMarquee = new System.Windows.Forms.Label();
            this.tmrTick = new System.Windows.Forms.Timer(this.components);
            this.picBackground = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).BeginInit();
            this.SuspendLayout();
            // 
            // lblVersionInfo
            // 
            this.lblVersionInfo.AutoSize = true;
            this.lblVersionInfo.BackColor = System.Drawing.Color.Transparent;
            this.lblVersionInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(86)))), ((int)(((byte)(83)))), ((int)(((byte)(83)))));
            this.lblVersionInfo.Location = new System.Drawing.Point(12, 378);
            this.lblVersionInfo.Name = "lblVersionInfo";
            this.lblVersionInfo.Size = new System.Drawing.Size(70, 13);
            this.lblVersionInfo.TabIndex = 5;
            this.lblVersionInfo.Text = "lblVersionInfo";
            // 
            // lblMarquee
            // 
            this.lblMarquee.BackColor = System.Drawing.Color.Transparent;
            this.lblMarquee.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMarquee.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(185)))), ((int)(((byte)(62)))));
            this.lblMarquee.Location = new System.Drawing.Point(-4, 391);
            this.lblMarquee.Name = "lblMarquee";
            this.lblMarquee.Size = new System.Drawing.Size(830, 23);
            this.lblMarquee.TabIndex = 7;
            // 
            // tmrTick
            // 
            this.tmrTick.Enabled = true;
            this.tmrTick.Interval = 200;
            this.tmrTick.Tick += new System.EventHandler(this.tmrTick_Tick);
            // 
            // picBackground
            // 
            this.picBackground.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.picBackground.Image = global::FreyrViewer.Properties.Resources.QlikCockpitAbout2;
            this.picBackground.Location = new System.Drawing.Point(-4, 0);
            this.picBackground.Name = "picBackground";
            this.picBackground.Size = new System.Drawing.Size(591, 415);
            this.picBackground.TabIndex = 0;
            this.picBackground.TabStop = false;
            // 
            // FrmAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(586, 414);
            this.Controls.Add(this.lblMarquee);
            this.Controls.Add(this.lblVersionInfo);
            this.Controls.Add(this.picBackground);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmAbout";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Qlik Cockpit";
            ((System.ComponentModel.ISupportInitialize)(this.picBackground)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox picBackground;
        private System.Windows.Forms.Label lblVersionInfo;
        private System.Windows.Forms.Label lblMarquee;
        private System.Windows.Forms.Timer tmrTick;
    }
}