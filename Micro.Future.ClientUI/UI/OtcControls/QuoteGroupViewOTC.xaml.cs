﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Xceed.Wpf.AvalonDock.Layout;
using Micro.Future.ViewModel;
using Micro.Future.Message;

namespace Micro.Future.UI
{
    /// <summary>
    /// QuoteGroupDoc.xaml 的交互逻辑
    /// </summary>
    public partial class QuoteGroupViewOTC : UserControl, IReloadData
    {
        public QuoteGroupViewOTC()
        {
            InitializeComponent();
        }

        public event Action<OTCPricingVM> OnQuoteSelected;
        public string PersistanceId
        {
            get;
            set;
        }

        public void ReloadData()
        {
            //MessageHandlerContainer.DefaultInstance.Get<AbstractOTCMarketDataHandler>().SubMarketData();
        }

        private void QuoteListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OnQuoteSelected != null)
            {
                OTCPricingVM quote = QuoteListView.SelectedItem as OTCPricingVM;
                OnQuoteSelected(quote);
            }
        }

        public void Initialize()
        {
        }
    }
}
