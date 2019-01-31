namespace FreyrViewer.Ui.Controls.TextPreview
{
    partial class TextFilePreview
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextFilePreview));
            this.panViewer = new System.Windows.Forms.Panel();
            this.picOverview = new System.Windows.Forms.PictureBox();
            this.picLoader = new System.Windows.Forms.PictureBox();
            this.tmrHide = new System.Windows.Forms.Timer(this.components);
            this.picClose = new System.Windows.Forms.PictureBox();
            this.panViewer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picOverview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLoader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picClose)).BeginInit();
            this.SuspendLayout();
            // 
            // panViewer
            // 
            this.panViewer.AutoScroll = true;
            this.panViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panViewer.Controls.Add(this.picOverview);
            this.panViewer.Location = new System.Drawing.Point(0, 0);
            this.panViewer.Name = "panViewer";
            this.panViewer.Size = new System.Drawing.Size(87, 298);
            this.panViewer.TabIndex = 3;
            this.panViewer.Scroll += new System.Windows.Forms.ScrollEventHandler(this.panViewer_Scroll);
            this.panViewer.Paint += new System.Windows.Forms.PaintEventHandler(this.panViewer_Paint);
            // 
            // picOverview
            // 
            this.picOverview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.picOverview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picOverview.Location = new System.Drawing.Point(0, 1);
            this.picOverview.Margin = new System.Windows.Forms.Padding(4);
            this.picOverview.Name = "picOverview";
            this.picOverview.Size = new System.Drawing.Size(69, 105);
            this.picOverview.TabIndex = 2;
            this.picOverview.TabStop = false;
            this.picOverview.MouseClick += new System.Windows.Forms.MouseEventHandler(this.picOverview_MouseClick);
            this.picOverview.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picOverview_MouseMove);
            // 
            // picLoader
            // 
            this.picLoader.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picLoader.Image = ((System.Drawing.Image)(resources.GetObject("picLoader.Image")));
            this.picLoader.Location = new System.Drawing.Point(56, 117);
            this.picLoader.Name = "picLoader";
            this.picLoader.Size = new System.Drawing.Size(75, 75);
            this.picLoader.TabIndex = 4;
            this.picLoader.TabStop = false;
            this.picLoader.Visible = false;
            // 
            // tmrHide
            // 
            this.tmrHide.Interval = 500;
            this.tmrHide.Tick += new System.EventHandler(this.tmrHide_Tick);
            // 
            // picClose
            // 
            this.picClose.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picClose.Image = global::FreyrViewer.Properties.Resources.close2;
            this.picClose.Location = new System.Drawing.Point(3, 3);
            this.picClose.Name = "picClose";
            this.picClose.Size = new System.Drawing.Size(18, 18);
            this.picClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picClose.TabIndex = 4;
            this.picClose.TabStop = false;
            this.picClose.Click += new System.EventHandler(this.picClose_Click);
            // 
            // TextFilePreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.picLoader);
            this.Controls.Add(this.picClose);
            this.Controls.Add(this.panViewer);
            this.Name = "TextFilePreview";
            this.Size = new System.Drawing.Size(218, 296);
            this.Load += new System.EventHandler(this.TextFilePreview_Load);
            this.Resize += new System.EventHandler(this.TextFilePreview_Resize);
            this.panViewer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picOverview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLoader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picClose)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panViewer;
        private System.Windows.Forms.PictureBox picOverview;
        private System.Windows.Forms.Timer tmrHide;
        private System.Windows.Forms.PictureBox picLoader;
        private System.Windows.Forms.PictureBox picClose;
    }
}
