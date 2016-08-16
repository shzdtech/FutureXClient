using System;
using System.ComponentModel;
using System.Windows;
using Micro.Future.Windows;

namespace Micro.Future.CustomizedControls.Windows
{
    /// <summary>
    /// FilterSettingsWindowForTradeRecord.xaml 的交互逻辑
    /// </summary>
    public partial class FilterSettingsWindowForTradeRecord : Window
    {
        public event Action<string, string, string> OnFiltering;

        public FilterSettingsWindowForTradeRecord()
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

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
