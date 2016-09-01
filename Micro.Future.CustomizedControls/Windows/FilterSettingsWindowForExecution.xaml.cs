using System;
using System.ComponentModel;
using System.Windows;

namespace Micro.Future.Windows
{

    public partial class FilterSettingsWindowForExecution : Window
    {
        public event Action<string, string, string, string> OnFiltering;

        public string FilterTitle
        {
            get
            {
                return titleTxt.Text;
            }
            set { titleTxt.Text = value; }
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
            OnFiltering?.Invoke(FilterTitle, FilterExchange, FilterUnderlying, FilterContract);
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
            titleTxt.Text = "";
            exchangecombo.Text = "";
            exchangecombo.Text = "";
            underlyingTxt.Text = "";

        }
    }
}
