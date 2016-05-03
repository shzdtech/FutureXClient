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
        private HashEncoder<MD5CryptoServiceProvider, HashEncoderOption>
            _hashEncoder = new HashEncoder<MD5CryptoServiceProvider, HashEncoderOption>();
        private HashEncoderOption _hashEncodeOption = new HashEncoderOption();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            uint round = (uint)(roundComboBox.SelectedIndex + 1);

            if (round > 0)
            {
                _hashEncodeOption.Iteration = round;

                md5TextBox.Text = _hashEncoder.Encode(md5TextBox.Text, _hashEncodeOption);
            }
        }


    }
}
