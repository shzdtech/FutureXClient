using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Micro.Future.Windows
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class TradeSettingsWindow : Window
    {
        public event Action<string, string, string> OnFiltering;

        public TradeSettingsWindow()
        {
            InitializeComponent();
        }


        public string TradeTitle
        {
            get
            {
                return titleTxt.Text;
            }
        }

        public string TradeExchange
        {
            get
            {
                return exchangecombo.Text;
            }
        }

        public string TradeUnderlying
        {
            get
            {
                return underlyingTxt.Text;
            }
        }

        public string TradeContract
        {
            get
            {
                return contractTxt.Text;
            }
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            OnFiltering?.Invoke(TradeExchange, TradeUnderlying, TradeContract);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnClosing(e);
        }

        public IEnumerable ExchangeCollection
        {
            set
            {
                exchangecombo.ItemsSource = value;
            }
        }
    }
}
