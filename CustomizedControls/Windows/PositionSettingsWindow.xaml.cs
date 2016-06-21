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
    public partial class PositionSettingsWindow : Window
    {
        public PositionSettingsWindow()
        {
            InitializeComponent();
        }


        public string PositionTitle
        {
            get
            {
                return titleTxt.Text;
            }
        }

        public string PositionExchange
        {
            get
            {
                return exchangecombo.Text;
            }
        }

        public string PositionUnderlying
        {
            get
            {
                return underlyingTxt.Text;
            }
        }

        public string PositionContract
        {
            get
            {
                return contractTxt.Text;
            }
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
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
