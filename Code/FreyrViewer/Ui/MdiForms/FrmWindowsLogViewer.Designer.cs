namespace FreyrViewer.Ui.MdiForms
{
    partial class FrmWindowsLogViewer
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
            this.CtrlGridTabs = new System.Windows.Forms.TabControl();
            this.tabApplication = new System.Windows.Forms.TabPage();
            this.lblNoApplicationLog = new System.Windows.Forms.Label();
            this.tabSystem = new System.Windows.Forms.TabPage();
            this.lblNoSystemLog = new System.Windows.Forms.Label();
            this.tabSecurity = new System.Windows.Forms.TabPage();
            this.lblNoSecurityLog = new System.Windows.Forms.Label();
            this.ctrlContext1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.dteFrom = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dteTo = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmdFilterApply = new System.Windows.Forms.Button();
            this.dteFromTime = new System.Windows.Forms.DateTimePicker();
            this.dteToTime = new System.Windows.Forms.DateTimePicker();
            this.chkUseDate = new System.Windows.Forms.CheckBox();
            this.ctrlTooltip = new System.Windows.Forms.ToolTip(this.components);
            this.ctrlSplitLogs = new System.Windows.Forms.SplitContainer();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.CtrlGridTabs.SuspendLayout();
            this.tabApplication.SuspendLayout();
            this.tabSystem.SuspendLayout();
            this.tabSecurity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ctrlSplitLogs)).BeginInit();
            this.ctrlSplitLogs.Panel1.SuspendLayout();
            this.ctrlSplitLogs.Panel2.SuspendLayout();
            this.ctrlSplitLogs.SuspendLayout();
            this.SuspendLayout();
            // 
            // CtrlGridTabs
            // 
            this.CtrlGridTabs.Controls.Add(this.tabApplication);
            this.CtrlGridTabs.Controls.Add(this.tabSystem);
            this.CtrlGridTabs.Controls.Add(this.tabSecurity);
            this.CtrlGridTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CtrlGridTabs.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.CtrlGridTabs.Location = new System.Drawing.Point(0, 0);
            this.CtrlGridTabs.Name = "CtrlGridTabs";
            this.CtrlGridTabs.SelectedIndex = 0;
            this.CtrlGridTabs.Size = new System.Drawing.Size(1246, 548);
            this.CtrlGridTabs.TabIndex = 0;
            this.CtrlGridTabs.SelectedIndexChanged += new System.EventHandler(this.CtrlGridTabs_SelectedIndexChanged);
            // 
            // tabApplication
            // 
            this.tabApplication.Controls.Add(this.lblNoApplicationLog);
            this.tabApplication.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.tabApplication.Location = new System.Drawing.Point(4, 22);
            this.tabApplication.Name = "tabApplication";
            this.tabApplication.Padding = new System.Windows.Forms.Padding(3);
            this.tabApplication.Size = new System.Drawing.Size(1238, 522);
            this.tabApplication.TabIndex = 0;
            this.tabApplication.Text = "Application";
            this.tabApplication.UseVisualStyleBackColor = true;
            // 
            // lblNoApplicationLog
            // 
            this.lblNoApplicationLog.AutoSize = true;
            this.lblNoApplicationLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoApplicationLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblNoApplicationLog.Location = new System.Drawing.Point(7, 3);
            this.lblNoApplicationLog.Name = "lblNoApplicationLog";
            this.lblNoApplicationLog.Size = new System.Drawing.Size(123, 13);
            this.lblNoApplicationLog.TabIndex = 3;
            this.lblNoApplicationLog.Text = "No Application log found";
            // 
            // tabSystem
            // 
            this.tabSystem.Controls.Add(this.lblNoSystemLog);
            this.tabSystem.Location = new System.Drawing.Point(4, 22);
            this.tabSystem.Name = "tabSystem";
            this.tabSystem.Padding = new System.Windows.Forms.Padding(3);
            this.tabSystem.Size = new System.Drawing.Size(1238, 522);
            this.tabSystem.TabIndex = 1;
            this.tabSystem.Text = "System";
            this.tabSystem.UseVisualStyleBackColor = true;
            // 
            // lblNoSystemLog
            // 
            this.lblNoSystemLog.AutoSize = true;
            this.lblNoSystemLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoSystemLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblNoSystemLog.Location = new System.Drawing.Point(7, 3);
            this.lblNoSystemLog.Name = "lblNoSystemLog";
            this.lblNoSystemLog.Size = new System.Drawing.Size(105, 13);
            this.lblNoSystemLog.TabIndex = 4;
            this.lblNoSystemLog.Text = "No System log found";
            // 
            // tabSecurity
            // 
            this.tabSecurity.Controls.Add(this.lblNoSecurityLog);
            this.tabSecurity.Location = new System.Drawing.Point(4, 22);
            this.tabSecurity.Name = "tabSecurity";
            this.tabSecurity.Padding = new System.Windows.Forms.Padding(3);
            this.tabSecurity.Size = new System.Drawing.Size(1238, 522);
            this.tabSecurity.TabIndex = 2;
            this.tabSecurity.Text = "Security";
            this.tabSecurity.UseVisualStyleBackColor = true;
            // 
            // lblNoSecurityLog
            // 
            this.lblNoSecurityLog.AutoSize = true;
            this.lblNoSecurityLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNoSecurityLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblNoSecurityLog.Location = new System.Drawing.Point(7, 3);
            this.lblNoSecurityLog.Name = "lblNoSecurityLog";
            this.lblNoSecurityLog.Size = new System.Drawing.Size(109, 13);
            this.lblNoSecurityLog.TabIndex = 4;
            this.lblNoSecurityLog.Text = "No Security log found";
            // 
            // ctrlContext1
            // 
            this.ctrlContext1.Name = "contextMenuStrip1";
            this.ctrlContext1.Size = new System.Drawing.Size(61, 4);
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(283, 3);
            this.txtFilter.Multiline = true;
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtFilter.Size = new System.Drawing.Size(505, 61);
            this.txtFilter.TabIndex = 1;
            this.ctrlTooltip.SetToolTip(this.txtFilter, "List of filters to run.");
            this.txtFilter.Click += new System.EventHandler(this.txtFilter_Click);
            this.txtFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFilter_KeyDown);
            // 
            // dteFrom
            // 
            this.dteFrom.CalendarForeColor = System.Drawing.Color.Black;
            this.dteFrom.CalendarTitleForeColor = System.Drawing.Color.Fuchsia;
            this.dteFrom.CalendarTrailingForeColor = System.Drawing.Color.Red;
            this.dteFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dteFrom.Location = new System.Drawing.Point(42, 3);
            this.dteFrom.Name = "dteFrom";
            this.dteFrom.Size = new System.Drawing.Size(117, 20);
            this.dteFrom.TabIndex = 5;
            this.dteFrom.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dteFrom_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(248, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Filter";
            // 
            // dteTo
            // 
            this.dteTo.CalendarForeColor = System.Drawing.Color.Black;
            this.dteTo.CalendarTitleForeColor = System.Drawing.Color.Fuchsia;
            this.dteTo.CalendarTrailingForeColor = System.Drawing.Color.Red;
            this.dteTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dteTo.Location = new System.Drawing.Point(42, 24);
            this.dteTo.Name = "dteTo";
            this.dteTo.Size = new System.Drawing.Size(117, 20);
            this.dteTo.TabIndex = 6;
            this.dteTo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dteFrom_MouseDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "From:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "To:";
            // 
            // cmdFilterApply
            // 
            this.cmdFilterApply.Location = new System.Drawing.Point(794, 6);
            this.cmdFilterApply.Name = "cmdFilterApply";
            this.cmdFilterApply.Size = new System.Drawing.Size(75, 23);
            this.cmdFilterApply.TabIndex = 9;
            this.cmdFilterApply.Text = "&Apply";
            this.cmdFilterApply.UseVisualStyleBackColor = true;
            this.cmdFilterApply.Click += new System.EventHandler(this.cmdFilterApply_Click);
            // 
            // dteFromTime
            // 
            this.dteFromTime.CalendarForeColor = System.Drawing.Color.Black;
            this.dteFromTime.CalendarTitleForeColor = System.Drawing.Color.Fuchsia;
            this.dteFromTime.CalendarTrailingForeColor = System.Drawing.Color.Red;
            this.dteFromTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dteFromTime.Location = new System.Drawing.Point(165, 3);
            this.dteFromTime.Name = "dteFromTime";
            this.dteFromTime.ShowUpDown = true;
            this.dteFromTime.Size = new System.Drawing.Size(77, 20);
            this.dteFromTime.TabIndex = 27;
            this.dteFromTime.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dteFromTime.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dteFrom_MouseDown);
            // 
            // dteToTime
            // 
            this.dteToTime.CalendarForeColor = System.Drawing.Color.Black;
            this.dteToTime.CalendarTitleForeColor = System.Drawing.Color.Fuchsia;
            this.dteToTime.CalendarTrailingForeColor = System.Drawing.Color.Red;
            this.dteToTime.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dteToTime.Location = new System.Drawing.Point(165, 24);
            this.dteToTime.Name = "dteToTime";
            this.dteToTime.ShowUpDown = true;
            this.dteToTime.Size = new System.Drawing.Size(77, 20);
            this.dteToTime.TabIndex = 28;
            this.dteToTime.Value = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dteToTime.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dteFrom_MouseDown);
            // 
            // chkUseDate
            // 
            this.chkUseDate.AutoSize = true;
            this.chkUseDate.Location = new System.Drawing.Point(248, 30);
            this.chkUseDate.Name = "chkUseDate";
            this.chkUseDate.Size = new System.Drawing.Size(15, 14);
            this.chkUseDate.TabIndex = 11;
            this.ctrlTooltip.SetToolTip(this.chkUseDate, "Toggles date filters on and off.");
            this.chkUseDate.UseVisualStyleBackColor = true;
            // 
            // ctrlSplitLogs
            // 
            this.ctrlSplitLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ctrlSplitLogs.Location = new System.Drawing.Point(1, 50);
            this.ctrlSplitLogs.Name = "ctrlSplitLogs";
            this.ctrlSplitLogs.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // ctrlSplitLogs.Panel1
            // 
            this.ctrlSplitLogs.Panel1.Controls.Add(this.CtrlGridTabs);
            // 
            // ctrlSplitLogs.Panel2
            // 
            this.ctrlSplitLogs.Panel2.Controls.Add(this.txtInfo);
            this.ctrlSplitLogs.Size = new System.Drawing.Size(1246, 744);
            this.ctrlSplitLogs.SplitterDistance = 548;
            this.ctrlSplitLogs.TabIndex = 29;
            // 
            // txtInfo
            // 
            this.txtInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtInfo.Location = new System.Drawing.Point(0, 0);
            this.txtInfo.Multiline = true;
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInfo.Size = new System.Drawing.Size(1246, 192);
            this.txtInfo.TabIndex = 27;
            // 
            // FrmWindowsLogViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1246, 799);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.chkUseDate);
            this.Controls.Add(this.dteToTime);
            this.Controls.Add(this.dteFromTime);
            this.Controls.Add(this.cmdFilterApply);
            this.Controls.Add(this.dteTo);
            this.Controls.Add(this.dteFrom);
            this.Controls.Add(this.ctrlSplitLogs);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FrmWindowsLogViewer";
            this.Load += new System.EventHandler(this.FrmWindowsLogViewer_Load);
            this.CtrlGridTabs.ResumeLayout(false);
            this.tabApplication.ResumeLayout(false);
            this.tabApplication.PerformLayout();
            this.tabSystem.ResumeLayout(false);
            this.tabSystem.PerformLayout();
            this.tabSecurity.ResumeLayout(false);
            this.tabSecurity.PerformLayout();
            this.ctrlSplitLogs.Panel1.ResumeLayout(false);
            this.ctrlSplitLogs.Panel2.ResumeLayout(false);
            this.ctrlSplitLogs.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ctrlSplitLogs)).EndInit();
            this.ctrlSplitLogs.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl CtrlGridTabs;
        private System.Windows.Forms.ContextMenuStrip ctrlContext1;
        private System.Windows.Forms.TabPage tabSystem;
        private System.Windows.Forms.TabPage tabSecurity;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.DateTimePicker dteFrom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dteTo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button cmdFilterApply;
        private System.Windows.Forms.DateTimePicker dteFromTime;
        private System.Windows.Forms.DateTimePicker dteToTime;
        private System.Windows.Forms.CheckBox chkUseDate;
        private System.Windows.Forms.ToolTip ctrlTooltip;
        private System.Windows.Forms.SplitContainer ctrlSplitLogs;
        private System.Windows.Forms.TextBox txtInfo;
        private System.Windows.Forms.TabPage tabApplication;
        private System.Windows.Forms.Label lblNoApplicationLog;
        private System.Windows.Forms.Label lblNoSystemLog;
        private System.Windows.Forms.Label lblNoSecurityLog;
    }
}