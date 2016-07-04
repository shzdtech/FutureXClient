using System;
using System.Collections;
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

namespace Micro.Future.Windows
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class QuoteSettingsWindow : Window
    {

        public event Action<string, string, string> OnFiltering;

        public QuoteSettingsWindow()
        {
            InitializeComponent();
        }


        public string QuoteTitle
        {
            get
            {
                return titleTxt.Text;
            }
        }

        public string QuoteExchange
        {
            get
            {
                return exchangeCombo.Text;
            }
        }

        public string QuoteUnderlying
        {
            get
            {
                return underlyingTxt.Text;
            }
        }

        public string QuoteContract
        {
            get
            {
                return contractTxt.Text;
            }
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            OnFiltering?.Invoke(QuoteExchange, QuoteUnderlying, QuoteContract);
        }

        //public IEnumerable ExchangeCollection
        //{
        //    set
        //    {
        //        exchangeCombo.ItemsSource = value;
        //    }
        //}

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
