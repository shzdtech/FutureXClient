using Micro.Future.Message;
using System.Security.Cryptography;
using System.Windows;

namespace WpfUI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MD5 _md5 = MD5.Create();

        private HashEncoder<HashEncoderOption> _hashEncoder =
            new HashEncoder<HashEncoderOption>(
               (byteArray) =>
               {
                   return _md5.ComputeHash(byteArray);
               }
               );
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            uint round = (uint)(roundComboBox.SelectedIndex + 1);

            if (round > 0)
            {
                _hashEncoder.Option.Iteration = round;

                md5TextBox.Text = _hashEncoder.Encode(md5TextBox.Text);
            }
        }
    }
}
