using System;
using System.Windows.Forms;

namespace FreyrViewer.Common.Winforms
{
    public class DragReorderTabControl : IDisposable
    {
        private readonly TabControl _ctrlTab;
        private TabPage _predraggedTab;

        public DragReorderTabControl(TabControl ctrlTab)
        {
            _ctrlTab = ctrlTab;
            _ctrlTab.MouseDown += OnMouseDown;
            _ctrlTab.MouseUp += OnMouseUp;
            _ctrlTab.MouseMove += OnMouseMove;
            _ctrlTab.DragOver += OnDragOver;
            _ctrlTab.AllowDrop = true;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            _predraggedTab = GetTabUnderCursor();
        }
        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            _predraggedTab = null;
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            // mouse button down? tab was clicked?
            if (e.Button == MouseButtons.Left && _predraggedTab != null)
                _ctrlTab.DoDragDrop(_predraggedTab, DragDropEffects.Move);

        }
        private TabPage GetTabUnderCursor()
        {
            var pos = Cursor.Position;
            for (int i = 0; i < _ctrlTab.TabPages.Count; i++)
                if (_ctrlTab.GetTabRect(i).Contains(_ctrlTab.PointToClient(pos)))
                    return _ctrlTab.TabPages[i];

            return null;
        }

        protected void OnDragOver(object sender, DragEventArgs e)
        {
        
            if (e.Data.GetData(typeof(TabPage)) == null) return;
            TabPage draggedTab = (TabPage)e.Data.GetData(typeof(TabPage));
            
            TabPage pointedTab = GetTabUnderCursor();

            if (draggedTab == _predraggedTab && pointedTab != null)
            {
                e.Effect = DragDropEffects.Move;

                if (pointedTab != draggedTab)
                    SwapTabPages(draggedTab, pointedTab);
            }

        }

        private void SwapTabPages(TabPage src, TabPage dst)
        {
            int srci = _ctrlTab.TabPages.IndexOf(src);
            int dsti = _ctrlTab.TabPages.IndexOf(dst);

            _ctrlTab.TabPages[dsti] = src;
            _ctrlTab.TabPages[srci] = dst;

            if (_ctrlTab.SelectedIndex == srci)
                _ctrlTab.SelectedIndex = dsti;
            else if (_ctrlTab.SelectedIndex == dsti)
                _ctrlTab.SelectedIndex = srci;

            _ctrlTab.Refresh();
        }

        public void Dispose()
        {
            _ctrlTab.MouseMove -= OnMouseDown;
            _ctrlTab.MouseUp -= OnMouseUp;
            _ctrlTab.MouseMove -= OnMouseMove;
            _ctrlTab.DragOver -= OnDragOver;
        }
    }
}
