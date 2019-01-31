using System;

namespace FreyrViewer.Ui.Splashes
{
    public class Splash : IDisposable
    {
        public Splash(string text)
        {
            Text = text;
        }

        public event Action Disposing;

        public string Text { get; set; }

        public void Dispose()
        {
            Disposing?.Invoke();
        }
    }
}