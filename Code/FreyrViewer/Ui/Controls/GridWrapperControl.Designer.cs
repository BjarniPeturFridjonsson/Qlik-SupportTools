namespace FreyrViewer.Ui.Controls
{
    partial class GridWrapperControl<T>
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
            this.CtrlGrid = new BrightIdeasSoftware.FastDataListView();
            this.ctrlContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.CtrlGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // CtrlGrid
            // 
            this.CtrlGrid.CellEditUseWholeCell = false;
            this.CtrlGrid.DataSource = null;
            this.CtrlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CtrlGrid.Location = new System.Drawing.Point(0, 0);
            this.CtrlGrid.Name = "CtrlGrid";
            this.CtrlGrid.ShowGroups = false;
            this.CtrlGrid.Size = new System.Drawing.Size(1063, 725);
            this.CtrlGrid.TabIndex = 25;
            this.CtrlGrid.UseCompatibleStateImageBehavior = false;
            this.CtrlGrid.View = System.Windows.Forms.View.Details;
            this.CtrlGrid.VirtualMode = true;
            this.CtrlGrid.CellRightClick += new System.EventHandler<BrightIdeasSoftware.CellRightClickEventArgs>(this.CtrlGrid_CellRightClick);
            this.CtrlGrid.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.CtrlGrid_FormatRow);
            // 
            // ctrlContext
            // 
            this.ctrlContext.Name = "ctrlContext";
            this.ctrlContext.Size = new System.Drawing.Size(153, 26);
            this.ctrlContext.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ctrlContext_ItemClicked);
            // 
            // GridWrapperControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.CtrlGrid);
            this.Name = "GridWrapperControl";
            this.Size = new System.Drawing.Size(1063, 725);
            ((System.ComponentModel.ISupportInitialize)(this.CtrlGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.FastDataListView CtrlGrid;
        private System.Windows.Forms.ContextMenuStrip ctrlContext;
    }
}
