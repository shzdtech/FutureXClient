using System;
using System.ComponentModel;
using System.Windows;

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
        

        public bool CancelClosing
        {
            get; set;
        }
    }
}
