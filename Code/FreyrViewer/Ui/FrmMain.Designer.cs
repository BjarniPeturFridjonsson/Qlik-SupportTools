namespace FreyrViewer.Ui
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
            this.CtrlBottomToolStrip = new System.Windows.Forms.ToolStrip();
            this.lblStrip1 = new System.Windows.Forms.ToolStripLabel();
            this.CtrlTopMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchForFilesShiftCtrlFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.increaseFontSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decreaseFontSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.enablementVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CtrlDockPanelMain = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.tmrReadingFile = new System.Windows.Forms.Timer(this.components);
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleParalellScrollingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CtrlBottomToolStrip.SuspendLayout();
            this.CtrlTopMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // CtrlBottomToolStrip
            // 
            this.CtrlBottomToolStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.CtrlBottomToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStrip1});
            this.CtrlBottomToolStrip.Location = new System.Drawing.Point(0, 837);
            this.CtrlBottomToolStrip.Name = "CtrlBottomToolStrip";
            this.CtrlBottomToolStrip.Size = new System.Drawing.Size(1705, 25);
            this.CtrlBottomToolStrip.TabIndex = 0;
            // 
            // lblStrip1
            // 
            this.lblStrip1.Name = "lblStrip1";
            this.lblStrip1.Size = new System.Drawing.Size(10, 22);
            this.lblStrip1.Text = " ";
            // 
            // CtrlTopMenu
            // 
            this.CtrlTopMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.CtrlTopMenu.Location = new System.Drawing.Point(0, 0);
            this.CtrlTopMenu.Name = "CtrlTopMenu";
            this.CtrlTopMenu.Size = new System.Drawing.Size(1705, 24);
            this.CtrlTopMenu.TabIndex = 1;
            this.CtrlTopMenu.Text = "  ";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.openFolderToolStripMenuItem,
            this.searchForFilesShiftCtrlFToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.openToolStripMenuItem.Text = "Open Zip file";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // openFolderToolStripMenuItem
            // 
            this.openFolderToolStripMenuItem.Name = "openFolderToolStripMenuItem";
            this.openFolderToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.openFolderToolStripMenuItem.Text = "Open Folder";
            this.openFolderToolStripMenuItem.Click += new System.EventHandler(this.openFolderToolStripMenuItem_Click);
            // 
            // searchForFilesShiftCtrlFToolStripMenuItem
            // 
            this.searchForFilesShiftCtrlFToolStripMenuItem.Name = "searchForFilesShiftCtrlFToolStripMenuItem";
            this.searchForFilesShiftCtrlFToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
            this.searchForFilesShiftCtrlFToolStripMenuItem.Text = "Search for files (Shift - Ctrl - F)";
            this.searchForFilesShiftCtrlFToolStripMenuItem.Click += new System.EventHandler(this.searchForFilesShiftCtrlFToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.increaseFontSizeToolStripMenuItem,
            this.decreaseFontSizeToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // increaseFontSizeToolStripMenuItem
            // 
            this.increaseFontSizeToolStripMenuItem.Name = "increaseFontSizeToolStripMenuItem";
            this.increaseFontSizeToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.increaseFontSizeToolStripMenuItem.Text = "Increase Font Size ( Ctrl + )";
            this.increaseFontSizeToolStripMenuItem.Click += new System.EventHandler(this.increaseFontSizeToolStripMenuItem_Click);
            // 
            // decreaseFontSizeToolStripMenuItem
            // 
            this.decreaseFontSizeToolStripMenuItem.Name = "decreaseFontSizeToolStripMenuItem";
            this.decreaseFontSizeToolStripMenuItem.Size = new System.Drawing.Size(215, 22);
            this.decreaseFontSizeToolStripMenuItem.Text = "Decrease Font Size ( Ctrl - )";
            this.decreaseFontSizeToolStripMenuItem.Click += new System.EventHandler(this.decreaseFontSizeToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem1,
            this.enablementVideoToolStripMenuItem});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(169, 22);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // enablementVideoToolStripMenuItem
            // 
            this.enablementVideoToolStripMenuItem.Name = "enablementVideoToolStripMenuItem";
            this.enablementVideoToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.enablementVideoToolStripMenuItem.Text = "Enablement video";
            this.enablementVideoToolStripMenuItem.Click += new System.EventHandler(this.enablementVideoToolStripMenuItem_Click);
            // 
            // CtrlDockPanelMain
            // 
            this.CtrlDockPanelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CtrlDockPanelMain.BackgroundImage = global::FreyrViewer.Properties.Resources.DragNdropForCockpit3;
            this.CtrlDockPanelMain.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.CtrlDockPanelMain.Location = new System.Drawing.Point(1, 26);
            this.CtrlDockPanelMain.Name = "CtrlDockPanelMain";
            this.CtrlDockPanelMain.Size = new System.Drawing.Size(1704, 810);
            this.CtrlDockPanelMain.TabIndex = 4;
            // 
            // tmrReadingFile
            // 
            this.tmrReadingFile.Interval = 500;
            this.tmrReadingFile.Tick += new System.EventHandler(this.tmrReadingFile_Tick);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toggleParalellScrollingToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // toggleParalellScrollingToolStripMenuItem
            // 
            this.toggleParalellScrollingToolStripMenuItem.Name = "toggleParalellScrollingToolStripMenuItem";
            this.toggleParalellScrollingToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.toggleParalellScrollingToolStripMenuItem.Text = "Toggle Paralell Scrolling";
            this.toggleParalellScrollingToolStripMenuItem.Click += new System.EventHandler(this.toggleParalellScrollingToolStripMenuItem_Click);
            // 
            // FrmMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1705, 862);
            this.Controls.Add(this.CtrlBottomToolStrip);
            this.Controls.Add(this.CtrlTopMenu);
            this.Controls.Add(this.CtrlDockPanelMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.CtrlTopMenu;
            this.Name = "FrmMain";
            this.Text = "Qlik Cockpit";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.FrmMain_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.FrmMain_DragEnter);
            this.CtrlBottomToolStrip.ResumeLayout(false);
            this.CtrlBottomToolStrip.PerformLayout();
            this.CtrlTopMenu.ResumeLayout(false);
            this.CtrlTopMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip CtrlBottomToolStrip;
        private System.Windows.Forms.MenuStrip CtrlTopMenu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private WeifenLuo.WinFormsUI.Docking.DockPanel CtrlDockPanelMain;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFolderToolStripMenuItem;
        private System.Windows.Forms.ToolStripLabel lblStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem increaseFontSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decreaseFontSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchForFilesShiftCtrlFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem enablementVideoToolStripMenuItem;
        private System.Windows.Forms.Timer tmrReadingFile;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleParalellScrollingToolStripMenuItem;
    }
}

