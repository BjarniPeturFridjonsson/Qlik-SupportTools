using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using FreyrViewer.Models;
using FreyrViewer.Services;

namespace FreyrViewer.Ui.Controls.TextPreview
{
    class TextDataVisualizer
    {

        public Color WhiteSpaceColor { private get; set; }
        public Color TextColor { private get; set; } 

        private readonly GenericDataWrapperService _textView;
        private DrawingVisual TextVisual { get;}
        public BitmapSource Bitmap;

        // not using hard limit because large file would be a smudge.
        private const int MaxPreferredBitmapHeight = 4000;

        private readonly Object _locker = new object();

        private readonly PixelFormat _pixelFormat = PixelFormats.Bgra32;
        private byte[] _bmpPixels;
        private int _bmpStride;
        private int _bmpWidth;
        private int _bmpHeight;

        private int _stride;
        private int _width;
        private int _height;
        private double _lineRatio;
        private byte[] _pixels;

        private List<LogFileAnalyzerResult> _postColoring;

        public int NumLines => _textView.Lines.Count;

        public TextDataVisualizer(TextFilePreviewOptions options)
        {
            TextVisual = new DrawingVisual();
            _postColoring = options.DateaWrapperService?.LogFileAnalyzer?.AnalyzerResults;

             _textView = options.DateaWrapperService;
            _bmpStride = (_bmpWidth * _pixelFormat.BitsPerPixel + 7) / 8;
            _bmpHeight = 0;
            _bmpWidth = options.PreviewImageWidth;
            _bmpPixels = null;

            _stride = _bmpStride;
            _width = _bmpWidth;
            _height = _bmpHeight;
            _pixels = null;
            _lineRatio = 1.0;
        }

        public void Render()
        {
            DrawLines();
            PostProcess();
            try
            {
                lock (_locker)
                {
                    _bmpWidth = _width;
                    _bmpHeight = _height;
                    _bmpStride = _stride;
                    _bmpPixels = _pixels;
                }

                TextVisual.Dispatcher.Invoke(RenderBitmap, DispatcherPriority.Render);
            }
            catch (Exception)
            {
                // disposing?
            }
        }
        private void RenderBitmap()
        {
            if (_bmpHeight == 0) return;
            lock (_locker)
            {
                Bitmap = BitmapSource.Create(
                    _bmpWidth,
                    _bmpHeight,
                    96,
                    96,
                    _pixelFormat,
                    null,
                    _bmpPixels,
                    _bmpStride);
            }

            var drawingGroup = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(drawingGroup, BitmapScalingMode.HighQuality);
            var img = new ImageDrawing
            {
                Rect = new Rect(0.0, 0.0, _bmpWidth, _bmpHeight),
                ImageSource = Bitmap
            };
            drawingGroup.Children.Add(img);
            using (DrawingContext drawingContext = TextVisual.RenderOpen())
            {
                drawingContext.DrawDrawing(drawingGroup);
            }
        }
     

        private void DrawLines()
        {
            // Create the image buffer
            var height = NumLines;
            var scaledouwnCounter = 0;
            while (height > MaxPreferredBitmapHeight && scaledouwnCounter < 6)
            { // we want to compress the hight but not so much it becomes unintelligible smudge.
                height = height / 2;
                scaledouwnCounter++;
                if (height < MaxPreferredBitmapHeight)
                    height = MaxPreferredBitmapHeight;
            }
            _height = height;
            _stride = (_width * _pixelFormat.BitsPerPixel + 7) / 8;
            _pixels = new byte[_stride * _height];
            _lineRatio = (_height) / (double)(NumLines);

            int tabSize = 4;
            int virtualColumn = 0;
            
            var lineCounter = 0;
            foreach (var genericDataWrapper in _textView.Lines)
            {
               
                for (var ii = 0; ii < _textView.Headers.Count; ii++)
                {
                    if (_textView.Headers[ii].ColumnType == typeof(DateTime)) continue;
                    if (ii != 0)
                    {
                        var numChars = tabSize - (virtualColumn % tabSize);
                        SetPixels(virtualColumn, lineCounter, TextColor, numChars);
                        virtualColumn += numChars;
                    }
                    var text = _textView.Headers[ii].AspectGetter.Invoke(genericDataWrapper);
                    for (int i = 0; i < text?.Length; ++i)
                    {
                        
                        if (!Char.IsWhiteSpace(text[i]))
                        {
                            SetPixel(virtualColumn, lineCounter, WhiteSpaceColor);
                        }
                        else
                        {
                            SetPixel(virtualColumn, lineCounter, TextColor);
                        }
                        virtualColumn += 1;
                    }
                    
                }
                virtualColumn = 0;
                lineCounter++;
            }
        }

        private void PostProcess()
        {
            _postColoring?.ForEach(p =>
            {
                if (p.ColorLeft != null)
                {
                    
                   SetHardPixels(0, p.RowNumber, p.ColorLeft.Value,3,10);
                    
                }
                if (p.ColorRight != null)
                {
                    SetHardPixels(_bmpWidth-p.PixelWith, p.RowNumber, p.ColorRight.Value, 3, 10);
                }
            });
        }

        private void SetHardPixels(int startX, int startY, Color c, int height, int width)
        {

            for (int yCalc = startY; yCalc < height+ startY; yCalc++)
            {
                for (int x = startX; x < width+ startX; x++)
                {
                    var y = (int)(yCalc * _lineRatio); // when lines compress beond 1px per line.
                    if (x < _width && y < _height)
                    {
                        int offset = y * _stride + x * 4;
                        _pixels[offset + 0] = (byte)(_lineRatio * c.B); // blue
                        _pixels[offset + 1] = (byte)(_lineRatio * c.G); // green
                        _pixels[offset + 2] = (byte)(_lineRatio * c.R); // red
                        _pixels[offset + 3] = (byte)(_lineRatio * 255); // alpha
                    }
                }
            }

            
        }

        private void SetPixel(int x, int y, Color c)
        {
            y = (int)(y * _lineRatio); // when lines compress beond 1px per line.
            if (x < _width && y < _height)
            {
                int offset = y * _stride + x * 4;
                _pixels[offset + 0] += (byte)(_lineRatio * c.B); // blue
                _pixels[offset + 1] += (byte)(_lineRatio * c.G); // green
                _pixels[offset + 2] += (byte)(_lineRatio * c.R); // red
                _pixels[offset + 3] += (byte)(_lineRatio * 255); // alpha
            }
        }

        private void SetPixels(int x, int y, Color c, int nrOfPixels)
        {
            for (int i = 0; i < nrOfPixels; ++i)
            {
                SetPixel(x + i, y, c);
            }
        }

    }
}
