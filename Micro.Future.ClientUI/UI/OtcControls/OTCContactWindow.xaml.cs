using System;
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
using Micro.Future.ViewModel;
using Micro.Future.Message;

namespace Micro.Future.UI
{
    /// <summary>
    /// QuoteGroupDoc.xaml 的交互逻辑
    /// </summary>
    public partial class OTCContactWindow : Window, IReloadData
    {
        public OTCContactWindow()
        {
            InitializeComponent();
        }

        public AbstractOTCHandler OTCHandler { get; set; }

        public void ReloadData()
        {
            tradingDeskLV.ItemsSource = OTCHandler.TradingDeskVMCollection;

            OTCHandler.TradingDeskVMCollection.Clear();
            OTCHandler.QueryTradingDesk();
        }

    }
}
