namespace FreyrViewer.Ui.MdiForms
{
    partial class FrmSenseNodes
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
            this.ctrlSplitContainer = new System.Windows.Forms.SplitContainer();
            this.treeInfo = new System.Windows.Forms.TreeView();
            this.ctrlFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.lblData1 = new System.Windows.Forms.Label();
            this.txtData1 = new System.Windows.Forms.TextBox();
            this.CtrlGrid = new BrightIdeasSoftware.FastDataListView();
            this.ctrlContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuItemOpenLogFolder = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.ctrlSplitContainer)).BeginInit();
            this.ctrlSplitContainer.Panel1.SuspendLayout();
            this.ctrlSplitContainer.Panel2.SuspendLayout();
            this.ctrlSplitContainer.SuspendLayout();
            this.ctrlFlow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CtrlGrid)).BeginInit();
            this.ctrlContext.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctrlSplitContainer
            // 
            this.ctrlSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ctrlSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.ctrlSplitContainer.Name = "ctrlSplitContainer";
            // 
            // ctrlSplitContainer.Panel1
            // 
            this.ctrlSplitContainer.Panel1.Controls.Add(this.treeInfo);
            // 
            // ctrlSplitContainer.Panel2
            // 
            this.ctrlSplitContainer.Panel2.Controls.Add(this.ctrlFlow);
            this.ctrlSplitContainer.Panel2.Controls.Add(this.CtrlGrid);
            this.ctrlSplitContainer.Size = new System.Drawing.Size(1093, 696);
            this.ctrlSplitContainer.SplitterDistance = 364;
            this.ctrlSplitContainer.TabIndex = 0;
            // 
            // treeInfo
            // 
            this.treeInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeInfo.Location = new System.Drawing.Point(0, 0);
            this.treeInfo.Name = "treeInfo";
            this.treeInfo.Size = new System.Drawing.Size(364, 696);
            this.treeInfo.TabIndex = 0;
            this.treeInfo.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeInfo_BeforeSelect);
            this.treeInfo.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeInfo_NodeMouseClick);
            // 
            // ctrlFlow
            // 
            this.ctrlFlow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ctrlFlow.AutoScroll = true;
            this.ctrlFlow.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ctrlFlow.Controls.Add(this.lblData1);
            this.ctrlFlow.Controls.Add(this.txtData1);
            this.ctrlFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.ctrlFlow.Location = new System.Drawing.Point(0, 0);
            this.ctrlFlow.Name = "ctrlFlow";
            this.ctrlFlow.Size = new System.Drawing.Size(722, 514);
            this.ctrlFlow.TabIndex = 0;
            this.ctrlFlow.TabStop = true;
            this.ctrlFlow.Visible = false;
            this.ctrlFlow.WrapContents = false;
            this.ctrlFlow.Resize += new System.EventHandler(this.ctrlFlow_Resize);
            // 
            // lblData1
            // 
            this.lblData1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblData1.Location = new System.Drawing.Point(3, 0);
            this.lblData1.Name = "lblData1";
            this.lblData1.Size = new System.Drawing.Size(710, 24);
            this.lblData1.TabIndex = 0;
            this.lblData1.Text = "label1";
            // 
            // txtData1
            // 
            this.txtData1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtData1.BackColor = System.Drawing.Color.Black;
            this.txtData1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtData1.ForeColor = System.Drawing.Color.White;
            this.txtData1.Location = new System.Drawing.Point(3, 27);
            this.txtData1.Multiline = true;
            this.txtData1.Name = "txtData1";
            this.txtData1.Size = new System.Drawing.Size(710, 61);
            this.txtData1.TabIndex = 1;
            // 
            // CtrlGrid
            // 
            this.CtrlGrid.CellEditUseWholeCell = false;
            this.CtrlGrid.DataSource = null;
            this.CtrlGrid.Location = new System.Drawing.Point(0, 667);
            this.CtrlGrid.Name = "CtrlGrid";
            this.CtrlGrid.ShowGroups = false;
            this.CtrlGrid.Size = new System.Drawing.Size(153, 29);
            this.CtrlGrid.TabIndex = 23;
            this.CtrlGrid.UseCompatibleStateImageBehavior = false;
            this.CtrlGrid.View = System.Windows.Forms.View.Details;
            this.CtrlGrid.VirtualMode = true;
            this.CtrlGrid.Visible = false;
            // 
            // ctrlContext
            // 
            this.ctrlContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuItemOpenLogFolder});
            this.ctrlContext.Name = "ctrlContext";
            this.ctrlContext.Size = new System.Drawing.Size(163, 26);
            // 
            // mnuItemOpenLogFolder
            // 
            this.mnuItemOpenLogFolder.Name = "mnuItemOpenLogFolder";
            this.mnuItemOpenLogFolder.Size = new System.Drawing.Size(162, 22);
            this.mnuItemOpenLogFolder.Text = "Open Log Folder";
            this.mnuItemOpenLogFolder.Click += new System.EventHandler(this.mnuItemOpenLogFolder_Click);
            // 
            // FrmSenseNodes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1093, 696);
            this.Controls.Add(this.ctrlSplitContainer);
            this.KeyPreview = true;
            this.Name = "FrmSenseNodes";
            this.Load += new System.EventHandler(this.FrmSenseNodes_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FrmSenseNodes_KeyPress);
            this.ctrlSplitContainer.Panel1.ResumeLayout(false);
            this.ctrlSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ctrlSplitContainer)).EndInit();
            this.ctrlSplitContainer.ResumeLayout(false);
            this.ctrlFlow.ResumeLayout(false);
            this.ctrlFlow.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CtrlGrid)).EndInit();
            this.ctrlContext.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer ctrlSplitContainer;
        private System.Windows.Forms.TreeView treeInfo;
        private BrightIdeasSoftware.FastDataListView CtrlGrid;
        private System.Windows.Forms.FlowLayoutPanel ctrlFlow;
        private System.Windows.Forms.Label lblData1;
        private System.Windows.Forms.TextBox txtData1;
        private System.Windows.Forms.ContextMenuStrip ctrlContext;
        private System.Windows.Forms.ToolStripMenuItem mnuItemOpenLogFolder;
    }
}