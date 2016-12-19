using Micro.Future.LocalStorage;
using System;
using System.ComponentModel;
using System.Windows;
using System.Collections.ObjectModel;
using System.Linq;
using Micro.Future.LocalStorage.DataObject;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Micro.Future.Message;
using Micro.Future.ViewModel;

namespace Micro.Future.Windows
{

    public partial class FilterSettingsWindow : Window, IReloadData
    {
        public event Action<string, string, string, string> OnFiltering;

        public FilterSettingsWindow()
        {
            InitializeComponent();
        }


        public void ReloadData()
        {
        }

        public IList<FilterSettings> FilterSettingsList
        {
            get;
            set;
        }


        public string FilterTabTitle
        {
            get
            {
                return titleCombo.Text;
            }
            set { titleCombo.Text = value; }
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

        //public string FilterPortfolio
        //{
        //    get
        //    {
        //        return portfolioTxt.Text;
        //    }
        //    set { portfolioTxt.Text = value; }
        //}

        public string FilterId
        {
            get;
            set;
        } = Guid.NewGuid().ToString();

        public string UserID
        {
            get;
            set;
        }
        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            OnFiltering?.Invoke(FilterTabTitle, FilterExchange, FilterUnderlying, FilterContract);            
            ClientDbContext.SaveFilterSettings(MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().MessageWrapper.User.Id, PersistanceId, FilterId, FilterTabTitle, FilterExchange, FilterContract, FilterUnderlying);
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
        public string PersistanceId { get; set; }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            titleCombo.Text = "";
            exchangecombo.Text = "";
            underlyingTxt.Text = "";
            contractTxt.Text = "";
            //portfolioTxt.Text = "";

        }

        //private void titleCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    if(titleCombo.SelectedIndex >=0)
        //    {
        //        var filtersetting = FilterSettingsList[titleCombo.SelectedIndex];
        //        FilterId = filtersetting.Id;
        //        exchangecombo.Text = filtersetting.Exchange;
        //        underlyingTxt.Text = filtersetting.Underlying;
        //        contractTxt.Text = filtersetting.Contract;
        //    }

            //exchangecombo.Text = FilterSettingsList[titleCombo.];

        //}
    }
}
