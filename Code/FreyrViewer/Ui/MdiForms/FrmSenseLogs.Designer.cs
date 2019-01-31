namespace FreyrViewer.Ui.MdiForms
{
    partial class FrmSenseLogs
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
       

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSenseLogs));
            this.mnuContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.moveToSplitTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllLogsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllOnRightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllOnLeftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeAllEmptyLogsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctrlImagesForTabs = new System.Windows.Forms.ImageList(this.components);
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.chkUseDate = new System.Windows.Forms.CheckBox();
            this.dteToTime = new System.Windows.Forms.DateTimePicker();
            this.dteFromTime = new System.Windows.Forms.DateTimePicker();
            this.cmdFilterApply = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dteTo = new System.Windows.Forms.DateTimePicker();
            this.dteFrom = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdApplyAll = new System.Windows.Forms.Button();
            this.picDown = new System.Windows.Forms.PictureBox();
            this.ctrlSplitLogs = new System.Windows.Forms.SplitContainer();
            this.ctrlTab1 = new System.Windows.Forms.TabControl();
            this.ctrlTab2 = new System.Windows.Forms.TabControl();
            this.ctrlPreview = new FreyrViewer.Ui.Controls.TextPreview.TextFilePreview();
            this.ctrlLinePreview = new FreyrViewer.Ui.Controls.TextPreview.PreviewText();
            this.ctrlOpenFiles = new System.Windows.Forms.ListView();
            this.colErrror = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colWarning = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFilename = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.mnuContext.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ctrlSplitLogs)).BeginInit();
            this.ctrlSplitLogs.Panel1.SuspendLayout();
            this.ctrlSplitLogs.Panel2.SuspendLayout();
            this.ctrlSplitLogs.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuContext
            // 
            this.mnuContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.moveToSplitTabToolStripMenuItem,
            this.toolStripMenuItem1,
            this.closeToolStripMenuItem,
            this.closeAllLogsToolStripMenuItem,
            this.closeAllOnRightToolStripMenuItem,
            this.closeAllOnLeftToolStripMenuItem,
            this.closeAllEmptyLogsToolStripMenuItem});
            this.mnuContext.Name = "contextMenuStrip1";
            this.mnuContext.Size = new System.Drawing.Size(180, 142);
            // 
            // moveToSplitTabToolStripMenuItem
            // 
            this.moveToSplitTabToolStripMenuItem.Name = "moveToSplitTabToolStripMenuItem";
            this.moveToSplitTabToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.moveToSplitTabToolStripMenuItem.Text = "Move to split tab";
            this.moveToSplitTabToolStripMenuItem.Click += new System.EventHandler(this.moveToSplitTabToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(176, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.closeToolStripMenuItem.Text = "Close all but this";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // closeAllLogsToolStripMenuItem
            // 
            this.closeAllLogsToolStripMenuItem.Name = "closeAllLogsToolStripMenuItem";
            this.closeAllLogsToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.closeAllLogsToolStripMenuItem.Text = "Close all logs";
            this.closeAllLogsToolStripMenuItem.Click += new System.EventHandler(this.closeAllLogsToolStripMenuItem_Click);
            // 
            // closeAllOnRightToolStripMenuItem
            // 
            this.closeAllOnRightToolStripMenuItem.Name = "closeAllOnRightToolStripMenuItem";
            this.closeAllOnRightToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.closeAllOnRightToolStripMenuItem.Text = "Close all on right";
            this.closeAllOnRightToolStripMenuItem.Click += new System.EventHandler(this.closeAllOnRightToolStripMenuItem_Click);
            // 
            // closeAllOnLeftToolStripMenuItem
            // 
            this.closeAllOnLeftToolStripMenuItem.Name = "closeAllOnLeftToolStripMenuItem";
            this.closeAllOnLeftToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.closeAllOnLeftToolStripMenuItem.Text = "Close all on left";
            this.closeAllOnLeftToolStripMenuItem.Click += new System.EventHandler(this.closeAllOnLeftToolStripMenuItem_Click);
            // 
            // closeAllEmptyLogsToolStripMenuItem
            // 
            this.closeAllEmptyLogsToolStripMenuItem.Name = "closeAllEmptyLogsToolStripMenuItem";
            this.closeAllEmptyLogsToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.closeAllEmptyLogsToolStripMenuItem.Text = "Close all empty files";
            this.closeAllEmptyLogsToolStripMenuItem.Click += new System.EventHandler(this.closeAllEmptyLogsToolStripMenuItem_Click);
            // 
            // ctrlImagesForTabs
            // 
            this.ctrlImagesForTabs.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ctrlImagesForTabs.ImageStream")));
            this.ctrlImagesForTabs.TransparentColor = System.Drawing.Color.Transparent;
            this.ctrlImagesForTabs.Images.SetKeyName(0, "close.ico");
            this.ctrlImagesForTabs.Images.SetKeyName(1, "closeHover.ico");
            this.ctrlImagesForTabs.Images.SetKeyName(2, "WarningCloseStandard.png");
            this.ctrlImagesForTabs.Images.SetKeyName(3, "WarningCloseHoover.png");
            this.ctrlImagesForTabs.Images.SetKeyName(4, "ErrorCloseStandard.png");
            this.ctrlImagesForTabs.Images.SetKeyName(5, "ErrorCloseHoover.png");
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(280, 3);
            this.txtFilter.Multiline = true;
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtFilter.Size = new System.Drawing.Size(505, 45);
            this.txtFilter.TabIndex = 29;
            this.txtFilter.Click += new System.EventHandler(this.txtFilter_Click);
            this.txtFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFilter_KeyDown);
            // 
            // chkUseDate
            // 
            this.chkUseDate.AutoSize = true;
            this.chkUseDate.Location = new System.Drawing.Point(245, 30);
            this.chkUseDate.Name = "chkUseDate";
            this.chkUseDate.Size = new System.Drawing.Size(15, 14);
            this.chkUseDate.TabIndex = 38;
            this.chkUseDate.UseVisualStyleBackColor = true;
            // 
            // dteToTime
            // 
            this.dteToTime.CalendarForeColor = System.Drawing.Color.Black;
            this.dteToTime.CalendarTitleForeColor = System.Drawing.Color.Fuchsia;
            this.dteToTime.CalendarTrailingForeColor = System.Drawing.Color.Red;
            this.dteToTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dteToTime.Location = new System.Drawing.Point(162, 24);
            this.dteToTime.Name = "dteToTime";
            this.dteToTime.ShowUpDown = true;
            this.dteToTime.Size = new System.Drawing.Size(77, 20);
            this.dteToTime.TabIndex = 40;
            this.dteToTime.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // dteFromTime
            // 
            this.dteFromTime.CalendarForeColor = System.Drawing.Color.Black;
            this.dteFromTime.CalendarTitleForeColor = System.Drawing.Color.Fuchsia;
            this.dteFromTime.CalendarTrailingForeColor = System.Drawing.Color.Red;
            this.dteFromTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dteFromTime.Location = new System.Drawing.Point(162, 3);
            this.dteFromTime.Name = "dteFromTime";
            this.dteFromTime.ShowUpDown = true;
            this.dteFromTime.Size = new System.Drawing.Size(77, 20);
            this.dteFromTime.TabIndex = 39;
            this.dteFromTime.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            // 
            // cmdFilterApply
            // 
            this.cmdFilterApply.Location = new System.Drawing.Point(791, 1);
            this.cmdFilterApply.Name = "cmdFilterApply";
            this.cmdFilterApply.Size = new System.Drawing.Size(75, 23);
            this.cmdFilterApply.TabIndex = 37;
            this.cmdFilterApply.Text = "&Apply";
            this.cmdFilterApply.UseVisualStyleBackColor = true;
            this.cmdFilterApply.Click += new System.EventHandler(this.cmdFilterApply_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 13);
            this.label3.TabIndex = 36;
            this.label3.Text = "To:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 35;
            this.label2.Text = "From:";
            // 
            // dteTo
            // 
            this.dteTo.CalendarForeColor = System.Drawing.Color.Black;
            this.dteTo.CalendarTitleForeColor = System.Drawing.Color.Fuchsia;
            this.dteTo.CalendarTrailingForeColor = System.Drawing.Color.Red;
            this.dteTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dteTo.Location = new System.Drawing.Point(39, 24);
            this.dteTo.Name = "dteTo";
            this.dteTo.Size = new System.Drawing.Size(117, 20);
            this.dteTo.TabIndex = 34;
            // 
            // dteFrom
            // 
            this.dteFrom.CalendarForeColor = System.Drawing.Color.Black;
            this.dteFrom.CalendarTitleForeColor = System.Drawing.Color.Fuchsia;
            this.dteFrom.CalendarTrailingForeColor = System.Drawing.Color.Red;
            this.dteFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dteFrom.Location = new System.Drawing.Point(39, 3);
            this.dteFrom.Name = "dteFrom";
            this.dteFrom.Size = new System.Drawing.Size(117, 20);
            this.dteFrom.TabIndex = 33;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(245, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Filter";
            // 
            // cmdApplyAll
            // 
            this.cmdApplyAll.Location = new System.Drawing.Point(791, 25);
            this.cmdApplyAll.Name = "cmdApplyAll";
            this.cmdApplyAll.Size = new System.Drawing.Size(75, 23);
            this.cmdApplyAll.TabIndex = 41;
            this.cmdApplyAll.Text = "Apply All";
            this.cmdApplyAll.UseVisualStyleBackColor = true;
            this.cmdApplyAll.Click += new System.EventHandler(this.cmdApplyAll_Click);
            // 
            // picDown
            // 
            this.picDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picDown.Image = global::FreyrViewer.Properties.Resources.Down1;
            this.picDown.Location = new System.Drawing.Point(1629, 56);
            this.picDown.Name = "picDown";
            this.picDown.Size = new System.Drawing.Size(16, 16);
            this.picDown.TabIndex = 42;
            this.picDown.TabStop = false;
            this.picDown.Visible = false;
            this.picDown.Click += new System.EventHandler(this.picDown_Click);
            // 
            // ctrlSplitLogs
            // 
            this.ctrlSplitLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ctrlSplitLogs.Location = new System.Drawing.Point(7, 53);
            this.ctrlSplitLogs.Name = "ctrlSplitLogs";
            // 
            // ctrlSplitLogs.Panel1
            // 
            this.ctrlSplitLogs.Panel1.Controls.Add(this.ctrlTab1);
            // 
            // ctrlSplitLogs.Panel2
            // 
            this.ctrlSplitLogs.Panel2.Controls.Add(this.ctrlTab2);
            this.ctrlSplitLogs.Size = new System.Drawing.Size(1669, 686);
            this.ctrlSplitLogs.SplitterDistance = 556;
            this.ctrlSplitLogs.TabIndex = 44;
            // 
            // ctrlTab1
            // 
            this.ctrlTab1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlTab1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ctrlTab1.ImageList = this.ctrlImagesForTabs;
            this.ctrlTab1.Location = new System.Drawing.Point(0, 0);
            this.ctrlTab1.Name = "ctrlTab1";
            this.ctrlTab1.SelectedIndex = 0;
            this.ctrlTab1.Size = new System.Drawing.Size(556, 686);
            this.ctrlTab1.TabIndex = 3;
            this.ctrlTab1.SelectedIndexChanged += new System.EventHandler(this.ctrlTabLogFiles_SelectedIndexChanged);
            this.ctrlTab1.Enter += new System.EventHandler(this.ctrlTab1_Enter);
            // 
            // ctrlTab2
            // 
            this.ctrlTab2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlTab2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ctrlTab2.ImageList = this.ctrlImagesForTabs;
            this.ctrlTab2.Location = new System.Drawing.Point(0, 0);
            this.ctrlTab2.Name = "ctrlTab2";
            this.ctrlTab2.SelectedIndex = 0;
            this.ctrlTab2.Size = new System.Drawing.Size(1109, 686);
            this.ctrlTab2.TabIndex = 2;
            this.ctrlTab2.SelectedIndexChanged += new System.EventHandler(this.ctrlTabLogFiles_SelectedIndexChanged);
            this.ctrlTab2.Enter += new System.EventHandler(this.ctrlTab2_Enter);
            // 
            // ctrlPreview
            // 
            this.ctrlPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ctrlPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.ctrlPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ctrlPreview.Location = new System.Drawing.Point(1372, 80);
            this.ctrlPreview.Name = "ctrlPreview";
            this.ctrlPreview.OnMouseClickRowAction = null;
            this.ctrlPreview.Size = new System.Drawing.Size(279, 646);
            this.ctrlPreview.TabIndex = 45;
            // 
            // ctrlLinePreview
            // 
            this.ctrlLinePreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ctrlLinePreview.Location = new System.Drawing.Point(7, 58);
            this.ctrlLinePreview.Name = "ctrlLinePreview";
            this.ctrlLinePreview.Size = new System.Drawing.Size(1352, 150);
            this.ctrlLinePreview.TabIndex = 46;
            this.ctrlLinePreview.Visible = false;
            // 
            // ctrlOpenFiles
            // 
            this.ctrlOpenFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ctrlOpenFiles.BackColor = System.Drawing.Color.LightGray;
            this.ctrlOpenFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colErrror,
            this.colWarning,
            this.colFilename});
            this.ctrlOpenFiles.FullRowSelect = true;
            this.ctrlOpenFiles.Location = new System.Drawing.Point(992, 72);
            this.ctrlOpenFiles.MultiSelect = false;
            this.ctrlOpenFiles.Name = "ctrlOpenFiles";
            this.ctrlOpenFiles.ShowItemToolTips = true;
            this.ctrlOpenFiles.Size = new System.Drawing.Size(682, 656);
            this.ctrlOpenFiles.TabIndex = 47;
            this.ctrlOpenFiles.UseCompatibleStateImageBehavior = false;
            this.ctrlOpenFiles.View = System.Windows.Forms.View.Details;
            this.ctrlOpenFiles.Visible = false;
            this.ctrlOpenFiles.DoubleClick += new System.EventHandler(this.ctrlOpenFiles_DoubleClick);
            this.ctrlOpenFiles.Leave += new System.EventHandler(this.ctrlOpenFiles_Leave);
            // 
            // colErrror
            // 
            this.colErrror.Text = "E";
            // 
            // colWarning
            // 
            this.colWarning.Text = "W";
            // 
            // colFilename
            // 
            this.colFilename.Text = "File name";
            // 
            // FrmSenseLogs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1682, 740);
            this.Controls.Add(this.ctrlOpenFiles);
            this.Controls.Add(this.ctrlLinePreview);
            this.Controls.Add(this.ctrlPreview);
            this.Controls.Add(this.picDown);
            this.Controls.Add(this.cmdApplyAll);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.chkUseDate);
            this.Controls.Add(this.dteToTime);
            this.Controls.Add(this.dteFromTime);
            this.Controls.Add(this.cmdFilterApply);
            this.Controls.Add(this.dteTo);
            this.Controls.Add(this.dteFrom);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ctrlSplitLogs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmSenseLogs";
            this.mnuContext.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picDown)).EndInit();
            this.ctrlSplitLogs.Panel1.ResumeLayout(false);
            this.ctrlSplitLogs.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ctrlSplitLogs)).EndInit();
            this.ctrlSplitLogs.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ContextMenuStrip mnuContext;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.CheckBox chkUseDate;
        private System.Windows.Forms.DateTimePicker dteToTime;
        private System.Windows.Forms.DateTimePicker dteFromTime;
        private System.Windows.Forms.Button cmdFilterApply;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dteTo;
        private System.Windows.Forms.DateTimePicker dteFrom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ImageList ctrlImagesForTabs;
        private System.Windows.Forms.Button cmdApplyAll;
        private System.Windows.Forms.PictureBox picDown;
        private System.Windows.Forms.SplitContainer ctrlSplitLogs;
        private System.Windows.Forms.TabControl ctrlTab1;
        private System.Windows.Forms.TabControl ctrlTab2;
        private System.Windows.Forms.ToolStripMenuItem moveToSplitTabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllLogsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllOnRightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeAllOnLeftToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private Controls.TextPreview.TextFilePreview ctrlPreview;
        private Controls.TextPreview.PreviewText ctrlLinePreview;
        private System.Windows.Forms.ToolStripMenuItem closeAllEmptyLogsToolStripMenuItem;
        private System.Windows.Forms.ListView ctrlOpenFiles;
        private System.Windows.Forms.ColumnHeader colErrror;
        private System.Windows.Forms.ColumnHeader colWarning;
        private System.Windows.Forms.ColumnHeader colFilename;
    }
}