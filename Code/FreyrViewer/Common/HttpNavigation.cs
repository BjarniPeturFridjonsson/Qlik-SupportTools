namespace FreyrViewer.Common
{
    public class HttpNavigation
    {
        public static void Navigate(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            System.Diagnostics.Process.Start(url);
        }
    }
}
