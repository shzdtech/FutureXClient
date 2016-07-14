using Micro.Future.Message;
using Micro.Future.Constant;
using Micro.Future.Message.Business;
using Micro.Future.Message.PBMessageHandler;
using Micro.Future.UI;
using System;
using System.Text;
using System.Windows;

namespace Micro.Future.Test
{

    /// <summary>
    /// TestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : Window
    {
        PBOrderInfo _orderInfo;
        PBSignInManager _connectHelper = new PBSignInManager();
        public TestWindow()
        {
            InitializeComponent();
        }

        void _connectHelper_OnConnected(Exception obj)
        {
            if (obj != null)
                Title = obj.Message;
        }

        void _connectHelper_OnError(MessageException obj)
        {
            Title = obj.Message;
        }

        void _connectHelper_OnLogged(IUserInfo obj)
        {
            Title = "登陆成功";
        }

        private void testbtn_Click(object sender, RoutedEventArgs e)
        {
            var pb = new PBOrderInfo();
            pb.Exchange = textBox_Exchange.Text;
            pb.Contract = textBox_Contract.Text;
            pb.LimitPrice = double.Parse(textBox_Price.Text);
            pb.Tif = (int)OrderTIFType.GFD;
            pb.Volume = 1;
            pb.ExecType = (int)OrderExecType.LIMIT;
            pb.Direction = (int)DirectionType.BUY;
            _connectHelper.MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_ORDER_NEW, pb);

        }

        private bool Login()
        {
            LoginWindow loginWindow = new LoginWindow(_connectHelper);
            loginWindow.ShowDialog();
            return true;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            MessageHandlerContainer.Register<TradeHandler, TradeHandler>();
            MessageHandlerContainer.DefaultInstance.Get<TradeHandler>().
                RegisterMessageWrapper(_connectHelper.MessageWrapper);

            _connectHelper.OnConnected += _connectHelper_OnConnected;
            _connectHelper.OnLoginError += _connectHelper_OnError;
            _connectHelper.OnLogged += _connectHelper_OnLogged;

            _connectHelper.MessageWrapper.RegisterAction<PBOrderInfo, ExceptionMessage>
                            ((uint)BusinessMessageID.MSG_ID_ORDER_NEW, OnReturningOrderInfo, OnErrorAction);
            _connectHelper.MessageWrapper.RegisterAction<PBOrderInfo, ExceptionMessage>
                            ((uint)BusinessMessageID.MSG_ID_ORDER_UPDATE, OnReturningOrderInfo, OnErrorAction);
            _connectHelper.MessageWrapper.RegisterAction<PBOrderInfo, ExceptionMessage>
                            ((uint)BusinessMessageID.MSG_ID_ORDER_CANCEL, OnRspCancelOrder, OnErrorAction);


            Login();
        }

        private void OnRspCancelOrder(PBOrderInfo obj)
        {
            _orderInfo = obj;
            MessageBox.Show(obj.Contract + ":" + (OrderStatus)obj.OrderStatus);
        }

        private void OnErrorAction(ExceptionMessage obj)
        {
            MessageBox.Show(Encoding.UTF8.GetString(obj.Description.ToByteArray()));
        }

        private void OnReturningOrderInfo(PBOrderInfo obj)
        {
            _orderInfo = obj;
            MessageBox.Show(obj.Contract + ":" + (OrderStatus)obj.OrderStatus);
        }


        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private void CancelOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var sendobj = _orderInfo;
            if (tbOrderSysID.Text.Length > 0)
            {
                var sendobjBld = new PBOrderInfo();
                sendobjBld.Exchange = "SHFE";
                sendobjBld.Contract = textBox_Contract.Text;
                sendobjBld.OrderSysID = ulong.Parse(tbOrderSysID.Text);
                sendobj = sendobjBld;
            }
            _connectHelper.MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_ORDER_CANCEL, sendobj);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var pb = new PBStrategy();
            pb.Exchange = "shfe";
            pb.Contract = textBox_Contract.Text;
            pb.Enabled = true;
            pb.AllowTrading = true;

            _connectHelper.MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_MODIFY_STRATEGY, pb);
        }
    }
}
