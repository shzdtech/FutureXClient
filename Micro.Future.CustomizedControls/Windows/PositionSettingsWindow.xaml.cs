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
    public partial class PositionSettingsWindow : Window
    {
        public event Action<string, string, string> OnFiltering;
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
            set { exchangecombo.Text = value; }
        }

        public string PositionUnderlying
        {
            get
            {
                return underlyingTxt.Text;
            }
            set { underlyingTxt.Text = value; }
        }

        public string PositionContract
        {
            get
            {
                return contractTxt.Text;
            }
            set { contractTxt.Text = value; }
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            OnFiltering?.Invoke(PositionExchange, PositionUnderlying, PositionContract);
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            Hide();
            e.Cancel = CancelClosing;
            base.OnClosing(e);
        }
        
        public IEnumerable ExchangeCollection
        {
            set
            {
                exchangecombo.ItemsSource = value;
            }
        }

        public bool CancelClosing
        {
            get; set;
        }
    }
}
