using Micro.Future.CustomizedControls.Controls;
using Micro.Future.LocalStorage;
using Micro.Future.Message;
using Micro.Future.UI;
using Micro.Future.ViewModel;
using Micro.Future.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Security.Cryptography;


namespace Micro.Future.CustomizedControls.Windows
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class ResetPasswordWindow : Window
    {
        public AbstractSignInManager SignInManager
        {
            get; protected set;
        }
        private PBSignInManager _accountSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<AccountHandler>());
        public string Password
        {
            get;
            set;
        }

        private HashEncoder<HashEncoderOption> _hashEncoder =
    new HashEncoder<HashEncoderOption>(MD5.Create(),
       (md5, byteArray) =>
       {
           return ((MD5)md5).ComputeHash(byteArray);
       }
       );

        public uint MD5Round
        {
            get;
            set;
        }
        public ResetPasswordWindow(string password)
        {
            InitializeComponent();
            Password = password;

        }

        //private IEnumerable<PortfolioVM> SeletedQuoteVM
        //{
        //    get
        //    {
        //        var selectedItems = portofolioTextBox.SelectedText;

        //    }
        //}


        private async void Button_Click_Reset(object sender, RoutedEventArgs e)
        {
            string password = originalPasswordTextBox.Password;
            if (password!=null)
            {

                _hashEncoder.Option.Iteration = MD5Round;
                password = _hashEncoder.Encode(password);
            }

            if (Password == password)
            {

                if (resetPasswordTextBox.Password == "")
                {
                    this.resetPasswordTextBox.Background = new SolidColorBrush(Colors.Red);
                    MessageBox.Show(this, "输入不能为空");
                    this.resetPasswordTextBox.Background = new SolidColorBrush(Colors.White);
                    return;
                }
                else if (resetPasswordTextBox.Password != affirmPasswordTextBox.Password)
                {
                    MessageBox.Show(this, "两次密码输入不一致,请重新输入!", "系统提示");
                    return;
                }
                else if (resetPasswordTextBox.Password == affirmPasswordTextBox.Password)
                {
                    bool bSuc = await MessageHandlerContainer.DefaultInstance.Get<AccountHandler>().ResetPassword(resetPasswordTextBox.Password);
                    if (!bSuc)
                        MessageBox.Show(this, "修改密码失败!", "系统提示");
                    else
                        MessageBox.Show(this, "密码修改成功!请重启程序", "系统提示");
                    this.Close();
                }
            }
            else
                MessageBox.Show(this, "原密码错误!", "系统提示");
        }

    }
}
