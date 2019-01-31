﻿namespace FreyrSenseCollector
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.lblMsg = new System.Windows.Forms.Label();
            this.cmdStart = new System.Windows.Forms.Button();
            this.lblPrivacy = new System.Windows.Forms.LinkLabel();
            this.lblGeneralInfo = new System.Windows.Forms.LinkLabel();
            this.dteStop = new System.Windows.Forms.DateTimePicker();
            this.dteStart = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCaseNr = new System.Windows.Forms.TextBox();
            this.ctrlProgressbar = new System.Windows.Forms.ProgressBar();
            this.chkOnline = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmdLogFile = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.ctrlToolTTip = new System.Windows.Forms.ToolTip(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.chkSysInfo = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.chkWinLogs = new System.Windows.Forms.CheckBox();
            this.ChkScriptLogs = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.lblAttachMsg = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.tmr = new System.Windows.Forms.Timer(this.components);
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.picCheckbox = new System.Windows.Forms.PictureBox();
            this.picCircles = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.cmdProblems = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCheckbox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCircles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblMsg.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblMsg.Location = new System.Drawing.Point(60, 178);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(107, 21);
            this.lblMsg.TabIndex = 3;
            this.lblMsg.Text = "Collected logs";
            this.lblMsg.Visible = false;
            // 
            // cmdStart
            // 
            this.cmdStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(225)))), ((int)(((byte)(221)))));
            this.cmdStart.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(167)))), ((int)(((byte)(41)))));
            this.cmdStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdStart.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cmdStart.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdStart.Location = new System.Drawing.Point(21, 431);
            this.cmdStart.Name = "cmdStart";
            this.cmdStart.Size = new System.Drawing.Size(110, 41);
            this.cmdStart.TabIndex = 12;
            this.cmdStart.Text = "Collect";
            this.cmdStart.UseVisualStyleBackColor = false;
            this.cmdStart.Click += new System.EventHandler(this.cmdStart_Click);
            // 
            // lblPrivacy
            // 
            this.lblPrivacy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPrivacy.AutoSize = true;
            this.lblPrivacy.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblPrivacy.Location = new System.Drawing.Point(322, 33);
            this.lblPrivacy.Name = "lblPrivacy";
            this.lblPrivacy.Size = new System.Drawing.Size(133, 21);
            this.lblPrivacy.TabIndex = 13;
            this.lblPrivacy.TabStop = true;
            this.lblPrivacy.Text = "Privacy statement";
            this.lblPrivacy.Visible = false;
            // 
            // lblGeneralInfo
            // 
            this.lblGeneralInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGeneralInfo.AutoSize = true;
            this.lblGeneralInfo.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.lblGeneralInfo.Location = new System.Drawing.Point(630, 484);
            this.lblGeneralInfo.Name = "lblGeneralInfo";
            this.lblGeneralInfo.Size = new System.Drawing.Size(141, 21);
            this.lblGeneralInfo.TabIndex = 14;
            this.lblGeneralInfo.TabStop = true;
            this.lblGeneralInfo.Text = "What do we collect";
            this.lblGeneralInfo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblGeneralInfo_LinkClicked);
            // 
            // dteStop
            // 
            this.dteStop.CustomFormat = "yyyy-MM-dd";
            this.dteStop.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dteStop.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dteStop.Location = new System.Drawing.Point(324, 76);
            this.dteStop.Name = "dteStop";
            this.dteStop.Size = new System.Drawing.Size(130, 29);
            this.dteStop.TabIndex = 19;
            // 
            // dteStart
            // 
            this.dteStart.CustomFormat = "yyyy-MM-dd";
            this.dteStart.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dteStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dteStart.Location = new System.Drawing.Point(153, 76);
            this.dteStart.Name = "dteStart";
            this.dteStart.Size = new System.Drawing.Size(130, 29);
            this.dteStart.TabIndex = 18;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(66, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 21);
            this.label2.TabIndex = 17;
            this.label2.Text = "Logs from:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label1.Location = new System.Drawing.Point(39, 115);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(108, 21);
            this.label1.TabIndex = 16;
            this.label1.Text = "Case Number:";
            // 
            // txtCaseNr
            // 
            this.txtCaseNr.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtCaseNr.Location = new System.Drawing.Point(153, 115);
            this.txtCaseNr.Name = "txtCaseNr";
            this.txtCaseNr.Size = new System.Drawing.Size(302, 25);
            this.txtCaseNr.TabIndex = 15;
            this.txtCaseNr.TextChanged += new System.EventHandler(this.txtKey_TextChanged);
            // 
            // ctrlProgressbar
            // 
            this.ctrlProgressbar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ctrlProgressbar.Location = new System.Drawing.Point(21, 471);
            this.ctrlProgressbar.Name = "ctrlProgressbar";
            this.ctrlProgressbar.Size = new System.Drawing.Size(110, 10);
            this.ctrlProgressbar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.ctrlProgressbar.TabIndex = 20;
            this.ctrlProgressbar.Visible = false;
            // 
            // chkOnline
            // 
            this.chkOnline.AutoSize = true;
            this.chkOnline.Checked = true;
            this.chkOnline.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOnline.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.chkOnline.Location = new System.Drawing.Point(432, 17);
            this.chkOnline.Name = "chkOnline";
            this.chkOnline.Size = new System.Drawing.Size(15, 14);
            this.chkOnline.TabIndex = 28;
            this.chkOnline.UseVisualStyleBackColor = true;
            this.chkOnline.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.label5.Location = new System.Drawing.Point(320, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 19);
            this.label5.TabIndex = 27;
            this.label5.Text = "Online Delivery:";
            this.label5.Visible = false;
            // 
            // cmdLogFile
            // 
            this.cmdLogFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdLogFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(225)))), ((int)(((byte)(221)))));
            this.cmdLogFile.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(167)))), ((int)(((byte)(41)))));
            this.cmdLogFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdLogFile.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cmdLogFile.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdLogFile.Location = new System.Drawing.Point(137, 431);
            this.cmdLogFile.Name = "cmdLogFile";
            this.cmdLogFile.Size = new System.Drawing.Size(110, 41);
            this.cmdLogFile.TabIndex = 29;
            this.cmdLogFile.Text = "See Result";
            this.cmdLogFile.UseVisualStyleBackColor = false;
            this.cmdLogFile.Visible = false;
            this.cmdLogFile.Click += new System.EventHandler(this.cmdLogFile_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(290, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(28, 21);
            this.label6.TabIndex = 31;
            this.label6.Text = "To:";
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(17, 499);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(264, 13);
            this.label7.TabIndex = 32;
            this.label7.Text = "Use this tool only in conjunction with Product Support";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(645, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 21);
            this.label3.TabIndex = 23;
            this.label3.Text = "System Info:";
            this.ctrlToolTTip.SetToolTip(this.label3, "Will collect general server info");
            // 
            // chkSysInfo
            // 
            this.chkSysInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSysInfo.AutoSize = true;
            this.chkSysInfo.Checked = true;
            this.chkSysInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSysInfo.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSysInfo.Location = new System.Drawing.Point(744, 91);
            this.chkSysInfo.Name = "chkSysInfo";
            this.chkSysInfo.Size = new System.Drawing.Size(15, 14);
            this.chkSysInfo.TabIndex = 24;
            this.ctrlToolTTip.SetToolTip(this.chkSysInfo, "Will collect general server info");
            this.chkSysInfo.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(630, 109);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(111, 21);
            this.label4.TabIndex = 25;
            this.label4.Text = "Windows logs:";
            this.ctrlToolTTip.SetToolTip(this.label4, "Will collect Windows event errors and warnings");
            // 
            // chkWinLogs
            // 
            this.chkWinLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkWinLogs.AutoSize = true;
            this.chkWinLogs.Checked = true;
            this.chkWinLogs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWinLogs.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkWinLogs.Location = new System.Drawing.Point(744, 113);
            this.chkWinLogs.Name = "chkWinLogs";
            this.chkWinLogs.Size = new System.Drawing.Size(15, 14);
            this.chkWinLogs.TabIndex = 26;
            this.ctrlToolTTip.SetToolTip(this.chkWinLogs, "Will collect Windows event errors and warnings");
            this.chkWinLogs.UseVisualStyleBackColor = true;
            // 
            // ChkScriptLogs
            // 
            this.ChkScriptLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ChkScriptLogs.AutoSize = true;
            this.ChkScriptLogs.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChkScriptLogs.Location = new System.Drawing.Point(744, 135);
            this.ChkScriptLogs.Name = "ChkScriptLogs";
            this.ChkScriptLogs.Size = new System.Drawing.Size(15, 14);
            this.ChkScriptLogs.TabIndex = 38;
            this.ctrlToolTTip.SetToolTip(this.ChkScriptLogs, "Will collect Windows event errors and warnings");
            this.ChkScriptLogs.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(651, 131);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(90, 21);
            this.label9.TabIndex = 37;
            this.label9.Text = "Script Logs:";
            this.ctrlToolTTip.SetToolTip(this.label9, "Will collect the Qlik Script logs for the selected dates");
            // 
            // lblAttachMsg
            // 
            this.lblAttachMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblAttachMsg.AutoSize = true;
            this.lblAttachMsg.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAttachMsg.Location = new System.Drawing.Point(17, 395);
            this.lblAttachMsg.Name = "lblAttachMsg";
            this.lblAttachMsg.Size = new System.Drawing.Size(359, 21);
            this.lblAttachMsg.TabIndex = 33;
            this.lblAttachMsg.Text = "Please attach the zip file to your support case.";
            this.lblAttachMsg.Visible = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(225)))), ((int)(((byte)(221)))));
            this.button1.Enabled = false;
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(167)))), ((int)(((byte)(41)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.button1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button1.Location = new System.Drawing.Point(600, 78);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(172, 78);
            this.button1.TabIndex = 35;
            this.button1.UseVisualStyleBackColor = false;
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.label8.Location = new System.Drawing.Point(604, 67);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 21);
            this.label8.TabIndex = 36;
            this.label8.Text = "Options";
            // 
            // tmr
            // 
            this.tmr.Enabled = true;
            this.tmr.Interval = 1000;
            this.tmr.Tick += new System.EventHandler(this.tmr_Tick);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox4.Image")));
            this.pictureBox4.Location = new System.Drawing.Point(724, 2);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(36, 32);
            this.pictureBox4.TabIndex = 4;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Visible = false;
            // 
            // picCheckbox
            // 
            this.picCheckbox.Location = new System.Drawing.Point(34, 182);
            this.picCheckbox.Name = "picCheckbox";
            this.picCheckbox.Size = new System.Drawing.Size(20, 19);
            this.picCheckbox.TabIndex = 2;
            this.picCheckbox.TabStop = false;
            this.picCheckbox.Visible = false;
            // 
            // picCircles
            // 
            this.picCircles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.picCircles.Image = ((System.Drawing.Image)(resources.GetObject("picCircles.Image")));
            this.picCircles.Location = new System.Drawing.Point(418, 219);
            this.picCircles.Name = "picCircles";
            this.picCircles.Size = new System.Drawing.Size(366, 343);
            this.picCircles.TabIndex = 1;
            this.picCircles.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(-30, -22);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(800, 89);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(0, 162);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(786, 10);
            this.pictureBox2.TabIndex = 21;
            this.pictureBox2.TabStop = false;
            // 
            // cmdProblems
            // 
            this.cmdProblems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdProblems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(225)))), ((int)(((byte)(221)))));
            this.cmdProblems.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(167)))), ((int)(((byte)(41)))));
            this.cmdProblems.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdProblems.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.cmdProblems.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmdProblems.Location = new System.Drawing.Point(253, 431);
            this.cmdProblems.Name = "cmdProblems";
            this.cmdProblems.Size = new System.Drawing.Size(110, 41);
            this.cmdProblems.TabIndex = 69;
            this.cmdProblems.Text = "Issues Found";
            this.cmdProblems.UseVisualStyleBackColor = false;
            this.cmdProblems.Visible = false;
            this.cmdProblems.Click += new System.EventHandler(this.cmdProblems_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(225)))), ((int)(((byte)(221)))));
            this.ClientSize = new System.Drawing.Size(784, 525);
            this.Controls.Add(this.cmdProblems);
            this.Controls.Add(this.ChkScriptLogs);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lblAttachMsg);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmdLogFile);
            this.Controls.Add(this.chkOnline);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.chkWinLogs);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.chkSysInfo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmdStart);
            this.Controls.Add(this.ctrlProgressbar);
            this.Controls.Add(this.dteStop);
            this.Controls.Add(this.dteStart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCaseNr);
            this.Controls.Add(this.lblGeneralInfo);
            this.Controls.Add(this.lblPrivacy);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.lblMsg);
            this.Controls.Add(this.picCheckbox);
            this.Controls.Add(this.picCircles);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(660, 560);
            this.Name = "FrmMain";
            this.Text = "Qlik Sense Log Collector";
            this.Load += new System.EventHandler(this.Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCheckbox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picCircles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox picCircles;
        private System.Windows.Forms.PictureBox picCheckbox;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Button cmdStart;
        private System.Windows.Forms.LinkLabel lblPrivacy;
        private System.Windows.Forms.LinkLabel lblGeneralInfo;
        private System.Windows.Forms.DateTimePicker dteStop;
        private System.Windows.Forms.DateTimePicker dteStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCaseNr;
        private System.Windows.Forms.ProgressBar ctrlProgressbar;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.CheckBox chkOnline;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button cmdLogFile;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ToolTip ctrlToolTTip;
        private System.Windows.Forms.Label lblAttachMsg;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkSysInfo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkWinLogs;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Timer tmr;
        private System.Windows.Forms.CheckBox ChkScriptLogs;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button cmdProblems;
    }
}