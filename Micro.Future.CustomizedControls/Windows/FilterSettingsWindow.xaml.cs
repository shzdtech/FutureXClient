using System;
using System.ComponentModel;
using System.Windows;

namespace Micro.Future.Windows
{

    public partial class FilterSettingsWindow : Window
    {
        public event Action<string, int, string, string, string, string> OnFiltering;

        public FilterSettingsWindow()
        {
            InitializeComponent();
        }

        //To be done
        public string FilterTabType
        {
            get
            {
                return titleTxt.Text;
            }
            set { titleTxt.Text = value; }
        }

        //to be done
        public int FilterTabIndex
        {
            get
            {
                return 0;
            }
            //set { titleTxt.Text = value; }
        }


        public string FilterTabTitle
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
            OnFiltering?.Invoke(FilterTabType, FilterTabIndex, FilterTabTitle, FilterExchange, FilterUnderlying, FilterContract);
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
