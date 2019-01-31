namespace FreyrViewer.Ui.MdiForms
{
    partial class FrmMenu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMenu));
            this.treeViewMenu = new System.Windows.Forms.TreeView();
            this.ctrlRighMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.collapsChildNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandChildNodesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.ctrlRighMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewMenu
            // 
            this.treeViewMenu.BackColor = System.Drawing.SystemColors.Control;
            this.treeViewMenu.ContextMenuStrip = this.ctrlRighMenu;
            this.treeViewMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewMenu.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewMenu.HideSelection = false;
            this.treeViewMenu.ImageIndex = 0;
            this.treeViewMenu.ImageList = this.imageList;
            this.treeViewMenu.Location = new System.Drawing.Point(0, 0);
            this.treeViewMenu.Name = "treeViewMenu";
            this.treeViewMenu.SelectedImageIndex = 0;
            this.treeViewMenu.Size = new System.Drawing.Size(217, 532);
            this.treeViewMenu.TabIndex = 1;
            this.treeViewMenu.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeViewMenu_NodeMouseClick);
            // 
            // ctrlRighMenu
            // 
            this.ctrlRighMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.collapsChildNodesToolStripMenuItem,
            this.expandChildNodesToolStripMenuItem});
            this.ctrlRighMenu.Name = "ctrlRighMenu";
            this.ctrlRighMenu.Size = new System.Drawing.Size(182, 48);
            // 
            // collapsChildNodesToolStripMenuItem
            // 
            this.collapsChildNodesToolStripMenuItem.Name = "collapsChildNodesToolStripMenuItem";
            this.collapsChildNodesToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.collapsChildNodesToolStripMenuItem.Text = "Collaps Child Nodes";
            this.collapsChildNodesToolStripMenuItem.Click += new System.EventHandler(this.CollapsChildNodesToolStripMenuItem_Click);
            // 
            // expandChildNodesToolStripMenuItem
            // 
            this.expandChildNodesToolStripMenuItem.Name = "expandChildNodesToolStripMenuItem";
            this.expandChildNodesToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.expandChildNodesToolStripMenuItem.Text = "Expand Child Nodes";
            this.expandChildNodesToolStripMenuItem.Click += new System.EventHandler(this.ExpandChildNodesToolStripMenuItem_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "Hamburger.ico");
            this.imageList.Images.SetKeyName(1, "Hamburger_Empty.png");
            this.imageList.Images.SetKeyName(2, "Hamburger_Warning.png");
            this.imageList.Images.SetKeyName(3, "Hamburger_Error.png");
            this.imageList.Images.SetKeyName(4, "FileFlatGreen.png");
            this.imageList.Images.SetKeyName(5, "NetworkFlatGreen2.png");
            this.imageList.Images.SetKeyName(6, "FolderFlatGreen.png");
            this.imageList.Images.SetKeyName(7, "SererFlatGreen.png");
            this.imageList.Images.SetKeyName(8, "");
            // 
            // FrmMenu
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(217, 532);
            this.CloseButton = false;
            this.CloseButtonVisible = false;
            this.Controls.Add(this.treeViewMenu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmMenu";
            this.Controls.SetChildIndex(this.treeViewMenu, 0);
            this.ctrlRighMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeViewMenu;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ContextMenuStrip ctrlRighMenu;
        private System.Windows.Forms.ToolStripMenuItem collapsChildNodesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandChildNodesToolStripMenuItem;
    }
}