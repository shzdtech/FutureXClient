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
using Xceed.Wpf.AvalonDock.Layout;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using Micro.Future.Message.Business;
using System.ComponentModel;

namespace Micro.Future.UI
{
    /// <summary>
    /// OTCTradingDeskContractParam.xaml 的交互逻辑
    /// </summary>
    public partial class OTCTradingDeskContractParam : UserControl, IReloadData
    {
        public OTCTradingDeskContractParam()
        {
            InitializeComponent();

            OTCTradingContractParamListView.ItemsSource =
               MessageHandlerContainer.DefaultInstance.Get<AbstractOTCMarketDataHandler>().
               ContractParamVMCollection;
        }

        public void ReloadData()
        {
            MessageHandlerContainer.DefaultInstance.Get<AbstractOTCMarketDataHandler>().ContractParamVMCollection.Clear();
            MessageHandlerContainer.DefaultInstance.Get<AbstractOTCMarketDataHandler>().QueryContractParam();
        }

        private void Button_Click_Cfm(object sender, RoutedEventArgs e)
        {
            Button cfm = sender as Button;
            var cpVM = (ContractParamVM)((ListViewItem)cfm.Tag).DataContext;
            MessageHandlerContainer.DefaultInstance.Get<AbstractOTCMarketDataHandler>().UpdateContractParam(cpVM);
        }

        private void OnParamChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            FrameworkElement ctrl = sender as FrameworkElement;
            ContractParamVM contractParamVM = ctrl.DataContext as ContractParamVM;
            if (contractParamVM != null)
                contractParamVM.UpdateContractParam();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                if (e.Key == Key.Escape || e.Key == Key.Enter)
                {

                    ContractParamVM contractVM = ctrl.DataContext as ContractParamVM;
                    if (contractVM != null)
                    {
                        if (e.Key == Key.Enter)
                            contractVM.UpdateContractParam();
                        else
                        {
                            ctrl.DataContext = null;
                            ctrl.DataContext = contractVM;
                        }
                    }
                    ctrl.Background = Brushes.White;
                }
                else
                {
                    ctrl.Background = Brushes.MistyRose;
                }
            }
        }
    }
}
