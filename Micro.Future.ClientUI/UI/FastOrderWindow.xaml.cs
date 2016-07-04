using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PBMsgTrader;
using Micro.Future.Utility;
using PBWrapMsgOG;
using Micro.Future.ViewModel;
using Xceed.Wpf.Toolkit;
using Micro.Future.Message;

namespace Micro.Future.UI
{
    /// <summary>
    /// FastOrder.xaml 的交互逻辑
    /// </summary>
    public partial class FastOrderWindow : UserControl, AddOrderView
    {
    	private bool submitEnabled;
        public bool SubmitEnabled
        {
            get
            {
                return submitEnabled;
            }
            set
            {
                submitEnabled = value;
            }
        }
        
        public AddOrderViewModel ViewModel { get; set; }

        public FastOrderWindow()
        {
            ViewModel = new AddOrderViewModel(this);
            this.DataContext = ViewModel;
            InitializeComponent();
        }

        public void SendOrder(PBMsgTrader.PBMsgOrderInsert pb)
        {
            try
            {
                if (ManFastOrder.IsChecked == true)
                {
                    if (Xceed.Wpf.Toolkit.MessageBox.Show("order detail:\n\n" + GetDumpString(pb), "发送订单", MessageBoxButton.YesNo).Equals(MessageBoxResult.Yes))
                    {
                        TradeHandler.Instance.CreateOrder(pb);
                        Xceed.Wpf.Toolkit.MessageBox.Show("订单已发送");
                    }
                }
                else
                {
                    TradeHandler.Instance.CreateOrder(pb);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }

        private string GetDumpString(PBMsgOrderInsert pb)
        {
            StringBuilder sb = new StringBuilder();
            //foreach (Google.Protobuf.Descriptors.FieldDescriptor fd in pb.AllFields.Keys)
            //{
            //    if (pb.AllFields[fd].GetType().ToString() == "Google.Protobuf.ByteString")
            //    {
            //        string decodedFieldValue = ((Google.Protobuf.ByteString)pb.AllFields[fd]).ToString(Encoding.GetEncoding("gb2312"));
            //        sb.Append(string.Format("{0}: {1} ,", fd.Name, decodedFieldValue));
            //    }
            //    else if (pb.AllFields[fd].GetType().ToString() == "Google.Protobuf.Descriptors.EnumValueDescriptor")
            //    {
            //        sb.Append(string.Format("{0}: {1} ,", fd.Name, ((Google.Protobuf.Descriptors.EnumValueDescriptor)pb.AllFields[fd]).Number));
            //    }
            //    else
            //    {
            //        sb.Append(string.Format("{0}: {1} ,", fd.Name, pb.AllFields[fd]));
            //    }
            //}
            return sb.ToString();
        }

        private void NumFastOrder_Click(object sender, RoutedEventArgs e)
        {
            if (NumFastOrder.IsChecked == true)
            {
                MainWindow.MyInstance.StartListening();
            }
            else
            {
                MainWindow.MyInstance.StopListening();
            }
        }
    }


    public class ExecuteTypeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int index = 0;
            switch ((ExecuteType)value)
            {
                case ExecuteType.Market:
                    index = 0;
                    break;
                case ExecuteType.Limit:
                    index = 1;
                    break;
                case ExecuteType.Stop:
                    index = 2;
                    break;
                case ExecuteType.Stoplimit:
                    index = 3;
                    break;
                default:
                    break;
            }

            return index;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }

            int index = (int)value;
            if (index == 0)
            {
                return ExecuteType.Market;
            }

            if (index == 1)
            {
                return ExecuteType.Limit;
            }

            if (index == 2)
            {
                return ExecuteType.Stop;
            }

            if (index == 3)
            {
                return ExecuteType.Stoplimit;
            }

            return null;
        }
    }
}
