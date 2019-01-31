namespace FreyrViewer.Ui.MdiForms
{
    partial class FrmLogCollectorLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLogCollectorLog));
            this.label2 = new System.Windows.Forms.Label();
            this.ctrlTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRawData = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(194, 21);
            this.label2.TabIndex = 18;
            this.label2.Text = "The Log Collector Settings:";
            // 
            // ctrlTableLayout
            // 
            this.ctrlTableLayout.AutoSize = true;
            this.ctrlTableLayout.ColumnCount = 2;
            this.ctrlTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.ctrlTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.ctrlTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlTableLayout.Location = new System.Drawing.Point(0, 0);
            this.ctrlTableLayout.Name = "ctrlTableLayout";
            this.ctrlTableLayout.RowCount = 1;
            this.ctrlTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.ctrlTableLayout.Size = new System.Drawing.Size(997, 350);
            this.ctrlTableLayout.TabIndex = 19;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(1, 33);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AutoScroll = true;
            this.splitContainer1.Panel1.Controls.Add(this.ctrlTableLayout);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.txtRawData);
            this.splitContainer1.Size = new System.Drawing.Size(997, 700);
            this.splitContainer1.SplitterDistance = 350;
            this.splitContainer1.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(171, 21);
            this.label1.TabIndex = 19;
            this.label1.Text = "The Log Collector Logs:";
            // 
            // txtRawData
            // 
            this.txtRawData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRawData.Location = new System.Drawing.Point(3, 28);
            this.txtRawData.Multiline = true;
            this.txtRawData.Name = "txtRawData";
            this.txtRawData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtRawData.Size = new System.Drawing.Size(991, 318);
            this.txtRawData.TabIndex = 1;
            // 
            // FrmLogCollectorLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 745);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmLogCollectorLog";
            this.Load += new System.EventHandler(this.FrmGenericTextViewer_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel ctrlTableLayout;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtRawData;
        private System.Windows.Forms.Label label1;
    }
}