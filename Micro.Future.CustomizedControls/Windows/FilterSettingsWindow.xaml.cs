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
    public partial class FilterSettingsWindow : Window
    {
        public event Action<string, string, string> OnFiltering;
        public FilterSettingsWindow()
        {
            InitializeComponent();
        }


        public string FilterTitle
        {
            get
            {
                return titleTxt.Text;
            }
        }

        public string FilterExchange
        {
            get
            {
                return exchangecombo.Text;
            }
            set { exchangecombo.Text = value; }
        }

        public string FilterUnderlying
        {
            get
            {
                return underlyingTxt.Text;
            }
            set { underlyingTxt.Text = value; }
        }

        public string FilterContract
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
            OnFiltering?.Invoke(FilterExchange, FilterUnderlying, FilterContract);
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            Hide();
            e.Cancel = CancelClosing;
            base.OnClosing(e);
        }
        
        //public IEnumerable ExchangeCollection
        //{
        //    set
        //    {
        //        exchangecombo.ItemsSource = value;
        //    }
        //}

        public bool CancelClosing
        {
            get; set;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
