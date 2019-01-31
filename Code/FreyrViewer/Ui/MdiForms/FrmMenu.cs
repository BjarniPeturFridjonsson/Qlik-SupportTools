using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using FreyrViewer.Common;


namespace FreyrViewer.Ui.MdiForms
{
   

    public partial class FrmMenu : FrmBaseForm
    {
        private TreeNode _currentRightClickNode;

        public FrmMenu(CommonResources commonResources, string title) : base(commonResources, title)
        {
            InitializeComponent();
            WireDragNdrop();
            treeViewMenu.HideSelection = false;
            treeViewMenu.DrawMode = TreeViewDrawMode.OwnerDrawText;
            treeViewMenu.DrawNode += (o, e) =>
            {
                if (!e.Node.TreeView.Focused && e.Node == e.Node.TreeView.SelectedNode)
                {
                    Font treeFont = e.Node.NodeFont ?? e.Node.TreeView.Font;
                    e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
                    ControlPaint.DrawFocusRectangle(e.Graphics, e.Bounds, SystemColors.HighlightText, SystemColors.Highlight);
                    TextRenderer.DrawText(e.Graphics, e.Node.Text, treeFont, e.Bounds, SystemColors.HighlightText, TextFormatFlags.GlyphOverhangPadding);
                }
                else
                    e.DrawDefault = true;
            };
            treeViewMenu.MouseDown += (o, e) =>
            {
                TreeNode node = treeViewMenu.GetNodeAt(e.X, e.Y);
                if (node != null && node.Bounds.Contains(e.X, e.Y))
                    treeViewMenu.SelectedNode = node;
            };
        }



        public async Task NavigateToLog(string logName)
        {
            var node = treeViewMenu.Nodes.Find(logName, true).FirstOrDefault();
            if (node != null)
            {
                treeViewMenu.SelectedNode= node;
                await NodeClick(node);
            }
        }

        public void ShowMenu(List<MenuItemWrapper> menuItems)
        {
            menuItems.ForEach(p =>
            {
                treeViewMenu.Nodes.Add(CreateNode(p.Text, p));
            });
            treeViewMenu.ExpandAll();

        }

        public void ClearDynamicMenues()
        {
            foreach (TreeNode node in treeViewMenu.Nodes)
            {
                if (node.Text.Equals("Log files"))
                {
                    treeViewMenu.Invoke(new Action(() =>
                    {
                        node.Nodes.Clear();
                    }));
                   
                    return;
                }
            }
        }

        public void CreateDynamicMenuItems(MenuItemWrapper[] items)
        {
            treeViewMenu.Invoke(new Action(() =>
            {
                treeViewMenu.BeginUpdate();
                foreach (TreeNode node in treeViewMenu.Nodes)
                {
                    if (node.Text.Equals("Log files"))
                    {
                        AddTreeNode(items, node);
                        break;
                    }
                }
                treeViewMenu.EndUpdate();
            }));
        }

        private void AddTreeNode(MenuItemWrapper[] items, TreeNode node)
        {
            foreach (MenuItemWrapper p in items)
            {   
                var newNode = CreateNode(p.Text, p);
                AddTreeNode(p.SubMenuItems, newNode);
                node.Nodes.Add(newNode);
            }
        }

        private async Task<bool> NodeClick(TreeNode eNode)
        {
            if (!(eNode.Tag is MenuItemWrapper task) || task.MenuAction == null)
                return false;

            await task.MenuAction();
            return true;
        }

        private TreeNode CreateNode(string text, MenuItemWrapper item)
        {
            var node =  new TreeNode
            {
                Name = item.Key,
                Tag = item,
                ImageIndex = (int)item.ApplicationMenuIcon,
                SelectedImageIndex = (int)item.ApplicationMenuIcon,
                Text = text,
                //ForeColor = foreColor
            };
            
            return node;
        }

        
        private async void TreeViewMenu_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                _currentRightClickNode = e.Node;
                treeViewMenu.SelectedNode = e.Node;
                return;
            }
            await NodeClick(e.Node);
        }

        private void CollapsChildNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeViewMenu.BeginUpdate();
            _currentRightClickNode?.Collapse(false);
            treeViewMenu.EndUpdate();
        }

        private void ExpandChildNodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeViewMenu.BeginUpdate();
            _currentRightClickNode?.ExpandAll();
            treeViewMenu.EndUpdate();
        }

       
    }
}
