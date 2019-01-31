namespace FreyrViewer.Ui.MdiForms
{
    partial class FrmFilter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFilter));
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmdClose = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radNotContains = new System.Windows.Forms.RadioButton();
            this.radRegex = new System.Windows.Forms.RadioButton();
            this.radStartsWith = new System.Windows.Forms.RadioButton();
            this.radContains = new System.Windows.Forms.RadioButton();
            this.cmdClear = new System.Windows.Forms.Button();
            this.cmdClearQuick = new System.Windows.Forms.Button();
            this.lstCheckBox = new System.Windows.Forms.CheckedListBox();
            this.cmdApplyAndClose = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.cboColumn = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtFilter
            // 
            this.txtFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFilter.Location = new System.Drawing.Point(78, 41);
            this.txtFilter.Multiline = true;
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtFilter.Size = new System.Drawing.Size(532, 58);
            this.txtFilter.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "New Filter:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Active filters:";
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdClose.Location = new System.Drawing.Point(704, 369);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(75, 23);
            this.cmdClose.TabIndex = 11;
            this.cmdClose.Text = "&Clancel";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdFilterCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.radNotContains);
            this.groupBox1.Controls.Add(this.radRegex);
            this.groupBox1.Controls.Add(this.radStartsWith);
            this.groupBox1.Controls.Add(this.radContains);
            this.groupBox1.Location = new System.Drawing.Point(621, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(111, 97);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Filter Type";
            // 
            // radNotContains
            // 
            this.radNotContains.AutoSize = true;
            this.radNotContains.Location = new System.Drawing.Point(5, 53);
            this.radNotContains.Name = "radNotContains";
            this.radNotContains.Size = new System.Drawing.Size(95, 17);
            this.radNotContains.TabIndex = 3;
            this.radNotContains.TabStop = true;
            this.radNotContains.Text = "Not Containing";
            this.radNotContains.UseVisualStyleBackColor = true;
            // 
            // radRegex
            // 
            this.radRegex.AutoSize = true;
            this.radRegex.Location = new System.Drawing.Point(5, 72);
            this.radRegex.Name = "radRegex";
            this.radRegex.Size = new System.Drawing.Size(56, 17);
            this.radRegex.TabIndex = 2;
            this.radRegex.TabStop = true;
            this.radRegex.Text = "Regex";
            this.radRegex.UseVisualStyleBackColor = true;
            // 
            // radStartsWith
            // 
            this.radStartsWith.AutoSize = true;
            this.radStartsWith.Location = new System.Drawing.Point(5, 34);
            this.radStartsWith.Name = "radStartsWith";
            this.radStartsWith.Size = new System.Drawing.Size(77, 17);
            this.radStartsWith.TabIndex = 1;
            this.radStartsWith.TabStop = true;
            this.radStartsWith.Text = "Starts With";
            this.radStartsWith.UseVisualStyleBackColor = true;
            // 
            // radContains
            // 
            this.radContains.AutoSize = true;
            this.radContains.Checked = true;
            this.radContains.Location = new System.Drawing.Point(5, 15);
            this.radContains.Name = "radContains";
            this.radContains.Size = new System.Drawing.Size(66, 17);
            this.radContains.TabIndex = 0;
            this.radContains.TabStop = true;
            this.radContains.Text = "Contains";
            this.radContains.UseVisualStyleBackColor = true;
            // 
            // cmdClear
            // 
            this.cmdClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClear.Location = new System.Drawing.Point(535, 329);
            this.cmdClear.Name = "cmdClear";
            this.cmdClear.Size = new System.Drawing.Size(75, 23);
            this.cmdClear.TabIndex = 12;
            this.cmdClear.Text = "Clear All";
            this.cmdClear.UseVisualStyleBackColor = true;
            this.cmdClear.Click += new System.EventHandler(this.cmdClear_Click);
            // 
            // cmdClearQuick
            // 
            this.cmdClearQuick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClearQuick.Location = new System.Drawing.Point(423, 329);
            this.cmdClearQuick.Name = "cmdClearQuick";
            this.cmdClearQuick.Size = new System.Drawing.Size(106, 23);
            this.cmdClearQuick.TabIndex = 13;
            this.cmdClearQuick.Text = "Clear QuickFilters";
            this.cmdClearQuick.UseVisualStyleBackColor = true;
            this.cmdClearQuick.Click += new System.EventHandler(this.cmdClearQuick_Click);
            // 
            // lstCheckBox
            // 
            this.lstCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstCheckBox.CheckOnClick = true;
            this.lstCheckBox.FormattingEnabled = true;
            this.lstCheckBox.Location = new System.Drawing.Point(78, 132);
            this.lstCheckBox.Name = "lstCheckBox";
            this.lstCheckBox.Size = new System.Drawing.Size(532, 184);
            this.lstCheckBox.TabIndex = 14;
            this.lstCheckBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lstCheckBox_ItemCheck);
            // 
            // cmdApplyAndClose
            // 
            this.cmdApplyAndClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cmdApplyAndClose.Location = new System.Drawing.Point(623, 369);
            this.cmdApplyAndClose.Name = "cmdApplyAndClose";
            this.cmdApplyAndClose.Size = new System.Drawing.Size(75, 23);
            this.cmdApplyAndClose.TabIndex = 15;
            this.cmdApplyAndClose.Text = "&Apply";
            this.cmdApplyAndClose.UseVisualStyleBackColor = true;
            this.cmdApplyAndClose.Click += new System.EventHandler(this.cmdApplyAndClose_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(621, 132);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(176, 184);
            this.groupBox2.TabIndex = 17;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Regex help";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 126);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(160, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "? = 0 or 1 of previous expression";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 106);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(148, 13);
            this.label8.TabIndex = 21;
            this.label8.Text = "* = 0 or more of previous expr.";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 86);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(95, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "| = Alternation ( or )";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(88, 13);
            this.label6.TabIndex = 19;
            this.label6.Text = ". = Any character";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "$ = End of a string.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "^ = Start of a string.";
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(15, 379);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(78, 13);
            this.linkLabel1.TabIndex = 18;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Help for Regex";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // cboColumn
            // 
            this.cboColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboColumn.FormattingEnabled = true;
            this.cboColumn.Items.AddRange(new object[] {
            "(Search all columns)"});
            this.cboColumn.Location = new System.Drawing.Point(78, 14);
            this.cboColumn.Name = "cboColumn";
            this.cboColumn.Size = new System.Drawing.Size(215, 21);
            this.cboColumn.TabIndex = 19;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = " OR = text1|text2|text3";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 166);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(146, 13);
            this.label10.TabIndex = 24;
            this.label10.Text = "AND = (?=.*Text1)(?=.*Text2)";
            // 
            // FrmFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 404);
            this.Controls.Add(this.cboColumn);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.cmdApplyAndClose);
            this.Controls.Add(this.lstCheckBox);
            this.Controls.Add(this.cmdClearQuick);
            this.Controls.Add(this.cmdClear);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmFilter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmFilter_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radRegex;
        private System.Windows.Forms.RadioButton radStartsWith;
        private System.Windows.Forms.RadioButton radContains;
        private System.Windows.Forms.Button cmdClear;
        private System.Windows.Forms.Button cmdClearQuick;
        private System.Windows.Forms.CheckedListBox lstCheckBox;
        private System.Windows.Forms.Button cmdApplyAndClose;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.ComboBox cboColumn;
        private System.Windows.Forms.RadioButton radNotContains;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label3;
    }
}