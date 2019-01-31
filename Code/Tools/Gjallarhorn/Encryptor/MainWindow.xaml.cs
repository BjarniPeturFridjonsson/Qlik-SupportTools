using System.Windows;
using Encryptor.Common;

namespace Encryptor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CmdEncrypt_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtIn.Text))
            {
                MessageBox.Show("Please supply text", "Missing values");
                return;
            }

            var secret = TxtSecret.Text.Trim();
            if(string.IsNullOrWhiteSpace(secret))
                secret = Helper.Value;

            var sec = new Eir.Common.Security.Encryption();
            var s = sec.Encrypt(TxtIn.Text.Trim(), secret);
            TxtOut.Text = s;
            if (!TxtIn.Text.Trim().Equals(sec.Decrypt(s, secret)))
            {
                MessageBox.Show("The encryption failed.");
            }
        }
    }
}
