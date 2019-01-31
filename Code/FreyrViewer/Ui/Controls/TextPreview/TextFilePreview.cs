using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace FreyrViewer.Ui.Controls.TextPreview
{
    /*
     * This control was built with whiskey and loud music. 
     * Dont complain (bfr 2018-03-24 04:30)
     * 
     * This takes a log file and creates an compressed overview 
     * of the file like you have in Visual studio scrollbar map.
     * 
     * Will show a text preview on scroll 
     * Will return a rowNr when clicking on the image
     * 
     */
    public partial class TextFilePreview : UserControl
    {
        
        /// <summary>
        /// Returns the approx row nr that was clicked on in the overview picture
        /// </summary>
        public Action<int> OnMouseClickRowAction { get; set; }

        private TextFilePreviewOptions _currentOptions;
        private string _currentlyLoading = "";
        private int _currentDisplayedRowNr;
        //private Image _bitmap;
        private Dictionary<string,Image> _displays = new Dictionary<string, Image>();

        public TextFilePreview()
        {
            InitializeComponent();
        }

        private void OnLoad()
        {
            Visible = false;
        }

        private void SetTextBoxHeight(TextFilePreviewOptions options)
        {
            var s = "";
            for (int i = 0; i < options.NrOfTextRowsForTextPreview; i++)
            {
                s += "Å" + Environment.NewLine;
            }
            var a = TextRenderer.MeasureText(s, options.TextPreviewer.Font, new Size(10, 1024));
            options.TextPreviewer.Height = a.Height + options.TextPreviewer.Padding.Top + options.TextPreviewer.Padding.Bottom + SystemInformation.VerticalScrollBarWidth+5;

        }
        
        public void ClearAllDisplays()
        {
            _displays = new Dictionary<string, Image>();
        }

        public void HideDisplay()
        {
            Visible = false;
            if(_currentOptions!=null)
                _currentOptions.TextPreviewer.Visible = false;
        }

        
        public void ShowDisplay(string key, TextFilePreviewOptions options)
        {
            if (_currentlyLoading.Equals(key) && Visible) return;
           
            _currentOptions = options;
            _currentlyLoading = key;
            Visible = true;
            Loading(true);
            SetTextBoxHeight(options);
            Task.Run(()=> ShowOrCreateDisplay(key,options));
        }

        private bool ShowOrCreateDisplay(string key, TextFilePreviewOptions options)
        {
            if (!_displays.ContainsKey(key))
            {
                Image img = null;
                if (options.DateaWrapperService.Lines.Count != 0)
                {
                    img = CreateDisplay(options);
                }
                
                _displays[key]= img;
            }
            Invoke(new Action(() =>
            {
                ShowDisplay(_displays[key]);
                Loading(false);
            }));
            return true;
        }

        private void Loading(bool show)
        {
            picLoader.Visible = show;
            panViewer.Visible = show != true;
            BorderStyle = show ? BorderStyle.FixedSingle : BorderStyle.None ;
            
        }


        private void ShowDisplay(Image bitmap)
        {
            picOverview.Width = bitmap?.Width ?? 1;
            picOverview.Height = bitmap?.Height ?? 1;
            picOverview.Image = bitmap;
            panViewer.Width = bitmap?.Width ?? 0;
            SetViewerHeight();
           
        }

        private Image CreateDisplay(TextFilePreviewOptions options)
        {
            _currentOptions = options;

            var renderer = new TextDataVisualizer(options);
            renderer.TextColor = options.TextColor;
            renderer.WhiteSpaceColor = options.WhiteSpaceColor;
            renderer.Render();
            
            var bitmap = GetBitmapFromSource(renderer.Bitmap);
            return bitmap;
        }

        private void SetViewerHeight()
        {
            if (picOverview.Image == null) return;
            panViewer.Height = picOverview.Image.Height + panViewer.Margin.Top;
            if (panViewer.Height - panViewer.Margin.Top - panViewer.Margin.Bottom > Height)
                panViewer.Height = Height - panViewer.Margin.Top - panViewer.Margin.Bottom;
        }

        private Image GetBitmapFromSource(BitmapSource rendererBitmap)
        {
            var helper = new BitmapHelper();
            return helper.GetBitmapFromSource(rendererBitmap);
        }


        private void picOverview_MouseClick(object sender, MouseEventArgs e)
        {
            OnMouseClickRowAction?.Invoke(GetRowNrFromRelativeTextLocation(e.Location.Y));
        }

        private int GetRowNrFromRelativeTextLocation(int relativeTextLocation)
        {
            if (_currentOptions?.DateaWrapperService == null) return 0;
            //translate point over image into row nr.
            double percent = relativeTextLocation / (double)picOverview.Height;
            var row = (int) Math.Floor(_currentOptions.DateaWrapperService.Lines.Count * percent);
            return row;
        }

        
        private void ShowPreviewPanel(Point loc, Point relativeTextLocation)
        {
            if (_currentOptions == null) return;
            var row = GetRowNrFromRelativeTextLocation(relativeTextLocation.Y) - (_currentOptions.NrOfTextRowsForTextPreview/2);
            if (row < 0) row = 0;
            if (_currentDisplayedRowNr == row) return; //flicker guard
            _currentDisplayedRowNr = row;
            //position preview
            var point = new Point();
            point.X = _currentOptions.TextPreviewer.Location.X;
            //point.Y = loc.Y - (_currentOptions.TextPreviewer.Height/2);
            point.Y = _currentOptions.TextPreviewer.Location.Y;
            //preview will never be below and above the viewer area.
            //if (point.Y < Location.Y)
            //    point.Y = Location.Y;
            //else if (point.Y + _currentOptions.TextPreviewer.Height > Location.Y + Height)
            //    point.Y = Location.Y + Height - _currentOptions.TextPreviewer.Height;
            _currentOptions.TextPreviewer.Location = point;

            
            
            if (_currentOptions.DateaWrapperService.Lines.Count < row + _currentOptions.NrOfTextRowsForTextPreview) row = _currentOptions.DateaWrapperService.Lines.Count - _currentOptions.NrOfTextRowsForTextPreview;
            var s = "";
            if (row < 0) row = 0;
            for (int i = 0; i < _currentOptions.NrOfTextRowsForTextPreview; i++)
            {
                s += $"({row + i}) {_currentOptions.DateaWrapperService.GetTextAtLine(row + i)}{Environment.NewLine}";
            }

            //flicker guard would be not using a text box but to paint manually on a picturebox. 
            //but I think its an overkill to do so.
            _currentOptions.TextPreviewer.SetText(s, relativeTextLocation.X / (double)picOverview.Width);

            if (!_currentOptions.TextPreviewer.Visible)
            {
                _currentOptions.TextPreviewer.Visible = true;
            }
            if (!tmrHide.Enabled) tmrHide.Enabled = true;
        }


        private void picOverview_MouseMove(object sender, MouseEventArgs e)
        {
            var coordinates = Parent.PointToClient(Cursor.Position);
            ShowPreviewPanel(coordinates, e.Location);
        }

        private void panViewer_Scroll(object sender, ScrollEventArgs e)
        {
            var coordinates = picOverview.PointToClient(Cursor.Position);
            coordinates.X = 0;
            ShowPreviewPanel(panViewer.PointToClient(Cursor.Position), coordinates);
        }

        //the mouse events are always unreliable. No matter what this will fix it
        private void tmrHide_Tick(object sender, EventArgs e)
        {
            //return;
            if (!_currentOptions.TextPreviewer.Visible)
            {
                tmrHide.Enabled = false;
                return;
            }

            Point pt = panViewer.PointToClient(MousePosition);

            if (!(pt.X >= 0 && pt.Y >= 0 && pt.X <= panViewer.Width + 10 && pt.Y <= panViewer.Height))
            {
                _currentOptions.TextPreviewer.HideIfPossible();
                return;
            }

            ShowPreviewPanel(panViewer.PointToClient(Cursor.Position), picOverview.PointToClient(Cursor.Position));
            //beginning of revert to default image when we are move away from a tab that is not the currently selected tab.
            //var pos = _currentOptions.CurrentActiveTab.PointToClient(Cursor.Position);
            //var rect = new Rectangle(pos.X, pos.Y, 1, 1);
            //if (!_currentOptions.CurrentActiveTab.ClientRectangle.IntersectsWith(rect))
            //    ShowDisplay(_currentOptions.CurrentActiveTab.Tag?.ToString() ?? "unknown", _currentOptions);
        }

        private void panViewer_Paint(object sender, PaintEventArgs e)
        {

            //the scrollbar is visible untill second paint event.. go figure.
            if (panViewer.VerticalScroll.Visible)
            {
                panViewer.Width = picOverview.Width + SystemInformation.VerticalScrollBarWidth + 3;
            }
            else
            {
                panViewer.Width = picOverview.Width+3;
            }
        }

        private void TextFilePreview_Resize(object sender, EventArgs e)
        {
            SetViewerHeight();
        }

        private void TextFilePreview_Load(object sender, EventArgs e)
        {
            OnLoad();
        }

        private void picClose_Click(object sender, EventArgs e)
        {
            HideDisplay();
        }
    }
}
