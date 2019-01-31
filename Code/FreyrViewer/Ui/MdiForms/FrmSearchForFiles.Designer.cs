
namespace FreyrViewer.Ui.MdiForms
{
    partial class FrmSearchForFiles
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSearchForFiles));
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdClose = new System.Windows.Forms.Button();
            this.cmdApplyAndClose = new System.Windows.Forms.Button();
            this.lblError = new System.Windows.Forms.Label();
            this.tmrKeystrokes = new System.Windows.Forms.Timer(this.components);
            this.dteFrom = new System.Windows.Forms.DateTimePicker();
            this.dteTo = new System.Windows.Forms.DateTimePicker();
            this.chkFlip = new System.Windows.Forms.CheckBox();
            this.cmdClearDate = new System.Windows.Forms.Button();
            this.ctrlToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.CtrlGrid = new BrightIdeasSoftware.FastDataListView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.CtrlGrid)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilter.Location = new System.Drawing.Point(12, 33);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtFilter.Size = new System.Drawing.Size(526, 20);
            this.txtFilter.TabIndex = 3;
            this.ctrlToolTip.SetToolTip(this.txtFilter, "Search in all filenames");
            this.txtFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFilter_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Search for files:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 6;
            this.label2.Tag = "";
            this.label2.Text = "Files:";
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClose.Location = new System.Drawing.Point(609, 631);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(75, 23);
            this.cmdClose.TabIndex = 11;
            this.cmdClose.Text = "&Cancel";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdFilterCancel_Click);
            // 
            // cmdApplyAndClose
            // 
            this.cmdApplyAndClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdApplyAndClose.Location = new System.Drawing.Point(690, 631);
            this.cmdApplyAndClose.Name = "cmdApplyAndClose";
            this.cmdApplyAndClose.Size = new System.Drawing.Size(122, 23);
            this.cmdApplyAndClose.TabIndex = 15;
            this.cmdApplyAndClose.Text = "&Open Selected Files";
            this.cmdApplyAndClose.UseVisualStyleBackColor = true;
            this.cmdApplyAndClose.Click += new System.EventHandler(this.cmdApplyAndClose_Click);
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.BackColor = System.Drawing.SystemColors.Window;
            this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblError.ForeColor = System.Drawing.Color.Maroon;
            this.lblError.Location = new System.Drawing.Point(23, 83);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(51, 16);
            this.lblError.TabIndex = 16;
            this.lblError.Text = "label3";
            this.lblError.Visible = false;
            // 
            // tmrKeystrokes
            // 
            this.tmrKeystrokes.Interval = 1000;
            this.tmrKeystrokes.Tick += new System.EventHandler(this.tmrKeystrokes_Tick);
            // 
            // dteFrom
            // 
            this.dteFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dteFrom.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dteFrom.CalendarForeColor = System.Drawing.Color.Black;
            this.dteFrom.CalendarTitleForeColor = System.Drawing.Color.Fuchsia;
            this.dteFrom.CalendarTrailingForeColor = System.Drawing.Color.Red;
            this.dteFrom.Enabled = false;
            this.dteFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dteFrom.Location = new System.Drawing.Point(544, 33);
            this.dteFrom.Name = "dteFrom";
            this.dteFrom.Size = new System.Drawing.Size(117, 20);
            this.dteFrom.TabIndex = 17;
            this.ctrlToolTip.SetToolTip(this.dteFrom, "Filters file names based on the \r\nModified date on the file.");
            this.dteFrom.Value = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
            this.dteFrom.ValueChanged += new System.EventHandler(this.dteFrom_ValueChanged);
            // 
            // dteTo
            // 
            this.dteTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.dteTo.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dteTo.CalendarForeColor = System.Drawing.Color.Black;
            this.dteTo.CalendarTitleForeColor = System.Drawing.Color.Fuchsia;
            this.dteTo.CalendarTrailingForeColor = System.Drawing.Color.Red;
            this.dteTo.Enabled = false;
            this.dteTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dteTo.Location = new System.Drawing.Point(667, 33);
            this.dteTo.Name = "dteTo";
            this.dteTo.Size = new System.Drawing.Size(117, 20);
            this.dteTo.TabIndex = 18;
            this.ctrlToolTip.SetToolTip(this.dteTo, "Filters file names based on the \r\nModified date on the file.");
            this.dteTo.Value = new System.DateTime(2018, 3, 19, 13, 13, 12, 0);
            this.dteTo.ValueChanged += new System.EventHandler(this.dteTo_ValueChanged);
            // 
            // chkFlip
            // 
            this.chkFlip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkFlip.AutoSize = true;
            this.chkFlip.Checked = true;
            this.chkFlip.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFlip.Location = new System.Drawing.Point(215, 10);
            this.chkFlip.Name = "chkFlip";
            this.chkFlip.Size = new System.Drawing.Size(74, 17);
            this.chkFlip.TabIndex = 19;
            this.chkFlip.Text = "All / None";
            this.chkFlip.UseVisualStyleBackColor = true;
            this.chkFlip.Visible = false;
            // 
            // cmdClearDate
            // 
            this.cmdClearDate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClearDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdClearDate.ForeColor = System.Drawing.Color.Maroon;
            this.cmdClearDate.Location = new System.Drawing.Point(790, 33);
            this.cmdClearDate.Name = "cmdClearDate";
            this.cmdClearDate.Size = new System.Drawing.Size(22, 20);
            this.cmdClearDate.TabIndex = 20;
            this.cmdClearDate.Text = "X";
            this.ctrlToolTip.SetToolTip(this.cmdClearDate, "Cancel date filter");
            this.cmdClearDate.UseVisualStyleBackColor = true;
            this.cmdClearDate.Click += new System.EventHandler(this.cmdClearDate_Click);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(541, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 21;
            this.label3.Text = "Modified date from:";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(664, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Modified date to:";
            // 
            // CtrlGrid
            // 
            this.CtrlGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CtrlGrid.CellEditUseWholeCell = false;
            this.CtrlGrid.DataSource = null;
            this.CtrlGrid.Location = new System.Drawing.Point(12, 72);
            this.CtrlGrid.Name = "CtrlGrid";
            this.CtrlGrid.ShowGroups = false;
            this.CtrlGrid.Size = new System.Drawing.Size(800, 540);
            this.CtrlGrid.TabIndex = 26;
            this.CtrlGrid.UseCompatibleStateImageBehavior = false;
            this.CtrlGrid.View = System.Windows.Forms.View.Details;
            this.CtrlGrid.VirtualMode = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Location = new System.Drawing.Point(12, 618);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(291, 43);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Regex help";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 21);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(146, 13);
            this.label10.TabIndex = 24;
            this.label10.Text = "AND = (?=.*Text1)(?=.*Text2)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(172, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(111, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = " OR = text1|text2|text3";
            // 
            // FrmSearchForFiles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 667);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.CtrlGrid);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmdClearDate);
            this.Controls.Add(this.chkFlip);
            this.Controls.Add(this.dteTo);
            this.Controls.Add(this.dteFrom);
            this.Controls.Add(this.cmdApplyAndClose);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmSearchForFiles";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Search in filenames";
            this.Load += new System.EventHandler(this.FrmSearchForFiles_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.FrmSearchForFiles_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.CtrlGrid)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.Button cmdApplyAndClose;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.Timer tmrKeystrokes;
        private System.Windows.Forms.DateTimePicker dteFrom;
        private System.Windows.Forms.DateTimePicker dteTo;
        private System.Windows.Forms.CheckBox chkFlip;
        private System.Windows.Forms.Button cmdClearDate;
        private System.Windows.Forms.ToolTip ctrlToolTip;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private BrightIdeasSoftware.FastDataListView CtrlGrid;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label5;
    }
}