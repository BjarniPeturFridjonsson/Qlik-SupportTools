using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FreyrViewer.Services;

namespace FreyrViewer.Common.Winforms
{
    /// <summary>
    /// Uses the image list on your tab control for displaying the close image
    /// you have to have 2 images, but those can be the same.
    /// 
    /// This also support right clicking on the heder and mouse over on header.
    /// </summary>
    public class AnimatedCloseTab : IDisposable
    {
        public Action<TabPage> OnTabMouseOver { get; set; }
        public Action<TabPage> OnHeaderRightClick { get; set; }

        private readonly TabControl _tabCtrl;
        private readonly Action<TabPage> _onTabClose;
           

        private Dictionary<string, int> _tabImageIndex = new Dictionary<string, int>();
        

        public AnimatedCloseTab(TabControl tabCtrl, Action<TabPage> onTabClose)
        {
            _tabCtrl = tabCtrl;
            _onTabClose = onTabClose;
            _tabCtrl.MouseClick += ctrlTab_MouseClick;
            _tabCtrl.MouseMove += ctrlTab_MouseMove;
            _tabCtrl.ControlAdded += _tabCtrl_ControlAdded;
            //_tabCtrl.DrawMode = TabDrawMode.OwnerDrawFixed;
            //_tabCtrl.DrawItem += tabControl1_DrawItem;
        }

        private void _tabCtrl_ControlAdded(object sender, ControlEventArgs e)
        {

        }
       
        public void SetTabHeader(TabPage tab, SimplifiedFailureLevels level)
        {
            _tabImageIndex[tab.Tag as string +""] = (int) level;
            tab.ImageIndex = ((int) level) *2;
        }
        //private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        //{
        //    //e.DrawBackground();
        //    using (Brush br = new SolidBrush(TabColors[_tabCtrl.TabPages[e.Index]]))
        //    {
        //        e.Graphics.FillRectangle(br, e.Bounds);
        //        SizeF sz = e.Graphics.MeasureString(_tabCtrl.TabPages[e.Index].Text, e.Font);
        //        e.Graphics.DrawString(_tabCtrl.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + (e.Bounds.Width - sz.Width) / 2, e.Bounds.Top + (e.Bounds.Height - sz.Height) / 2 + 1);

        //        Rectangle rect = e.Bounds;
        //        rect.Offset(0, 1);
        //        rect.Inflate(0, -1);
        //        e.Graphics.DrawRectangle(Pens.DarkGray, rect);
        //        e.DrawFocusRectangle();
        //    }
        //}
        private void ctrlTab_MouseMove(object sender, MouseEventArgs e)
        {
            AnimatedCloseTabAction(e, false);
        }

        private void ctrlTab_MouseClick(object sender, MouseEventArgs e)
        {
            AnimatedCloseTabAction(e, true);
        }

        public void AnimatedCloseTabAction(MouseEventArgs e, bool close)
        {
            Rectangle mouseRect = new Rectangle(e.X, e.Y, 1, 1);
            for (int i = 0; i < _tabCtrl.TabCount; i++)
            {
                
                var rect = _tabCtrl.GetTabRect(i);
                var tab = _tabCtrl.TabPages[i];
                var baseImgIndex = _tabImageIndex[tab.Tag as string + ""] * 2;
                rect.Width = 25;
                if (rect.IntersectsWith(mouseRect))
                {
                   
                    if (close)
                    {
                        
                        _onTabClose?.Invoke(tab);
                        _tabCtrl.TabPages.RemoveAt(i);
                        tab.Dispose();

                        return;
                    }
                    
                    if (tab.ImageIndex != baseImgIndex+1)//flicker defence
                        tab.ImageIndex = baseImgIndex+1;
                }
                else
                {
                    rect = _tabCtrl.GetTabRect(i);
                    if (rect.IntersectsWith(mouseRect))
                    {
                        OnTabMouseOver?.Invoke(tab);

                        if (OnHeaderRightClick != null && e.Button == MouseButtons.Right)
                        {
                            OnHeaderRightClick.Invoke(_tabCtrl.TabPages[i]);
                        }
                    }
                    
                    if (_tabCtrl.TabPages[i].ImageIndex != baseImgIndex) //reset image with flicker defence
                        _tabCtrl.TabPages[i].ImageIndex = baseImgIndex;
                }
            }
        }

        public void Dispose()
        {
            _tabCtrl.MouseClick -= ctrlTab_MouseClick;
            _tabCtrl.MouseMove -= ctrlTab_MouseMove;
        }
    }
}
