using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Micro.Future.ViewModel;
using Micro.Future.Message;

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
        }

        public AbstractOTCMarketDataHandler OTCHandler { get; set; }

        public void ReloadData()
        {
            OTCTradingContractParamListView.ItemsSource = OTCHandler?.ContractParamVMCollection;
            OTCHandler?.ContractParamVMCollection.Clear();
            OTCHandler?.QueryContractParam();
        }

        private void Button_Click_Cfm(object sender, RoutedEventArgs e)
        {
            Button cfm = sender as Button;
            var cpVM = (ContractParamVM)((ListViewItem)cfm.Tag).DataContext;
            OTCHandler?.UpdateContractParam(cpVM);
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
