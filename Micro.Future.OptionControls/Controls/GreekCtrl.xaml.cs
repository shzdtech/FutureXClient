using Micro.Future.LocalStorage;
using Micro.Future.Utility;
using Micro.Future.ViewModel;
using Micro.Future.Windows;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.DataGrid;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class GreekCtrl : UserControl
    {
        public GreekCtrl()
        {
            //RiskVMCollection.Add(new RiskVM { Contract = "1801", Underlying = "222", Delta = 2, Gamma = 1 });
            //RiskVMCollection.Add(new RiskVM { Contract = "1802", Underlying = "222", Delta = 1, Gamma = 1 });
            //RiskVMCollection.Add(new RiskVM { Contract = "1803", Underlying = "111", Delta = 1, Gamma = 1 });
            InitializeComponent();
            FilterSettingsWin.OnFiltering += _filterSettingsWin_OnFiltering;
        }
        public FilterSettingsWindow FilterSettingsWin { get; }
              = new FilterSettingsWindow() { CancelClosing = true };
        public LayoutAnchorablePane AnchorablePane { get; set; }

        public ObservableCollection<RiskVM> RiskVMCollection
        {
            get;
        } = new ObservableCollection<RiskVM>();

        public void BindingToSource(ObservableCollection<RiskVM> source)
        {

            RiskVMCollection.Clear();
            foreach (var vm in source)
            {
                string basecontract = vm.Contract;
                var contractinfo = ClientDbContext.FindContract(vm.Contract);
                if (contractinfo != null)
                {
                    if (!string.IsNullOrEmpty(contractinfo.UnderlyingContract))
                        basecontract = contractinfo.UnderlyingContract;
                }
                var riskvm = RiskVMCollection.FirstOrDefault(r => r.Contract == basecontract);
                if (riskvm == null)
                {
                    vm.Contract = basecontract;
                    RiskVMCollection.Add(vm);
                }
                else
                {
                    riskvm.Delta += vm.Delta;
                    riskvm.Gamma += vm.Gamma;
                    riskvm.Theta += vm.Theta;
                    riskvm.Vega += vm.Vega;
                    riskvm.Rho += vm.Rho;
                }
            }

        }

        public void Filter(string tabTitle, string exchange, string underlying, string contract, string portfolio)
        {
            //GreekListView.View.FilterRowGlyph = 
            if (GreekListView == null)
            {
                return;
            }

            //this.AnchorablePane.SelectedContent.Title = tabTitle;
            FilterSettingsWin.FilterTabTitle = tabTitle;
            FilterSettingsWin.FilterExchange = exchange;
            FilterSettingsWin.FilterUnderlying = underlying;
            FilterSettingsWin.FilterContract = contract;
            FilterSettingsWin.FilterPortfolio = portfolio;


            ICollectionView view = (ICollectionView)GreekListView.View;
            view.Filter = delegate (object o)
            {
                if (contract == null)
                    return true;

                RiskVM pvm = o as RiskVM;

                if (pvm.Exchange.ContainsAny(exchange) &&
                    pvm.Contract.ContainsAny(underlying) &&
                    pvm.Contract.ContainsAny(contract) )
                {
                    return true;
                }

                return false;
            };
        }
        private void _filterSettingsWin_OnFiltering(string tabTitle, string exchange, string underlying, string contract, string portfolio)
        {
            //if (LayoutContent != null)
            //    LayoutContent.Title = _filterSettingsWin.FilterTabTitle;
            if (AnchorablePane != null)
                AnchorablePane.SelectedContent.Title = tabTitle;
            Filter(tabTitle, exchange, underlying, contract, portfolio);
        }

        private void MenuItem_Click_Settings(object sender, RoutedEventArgs e)
        {
            FilterSettingsWin.FilterTabTitle = AnchorablePane?.SelectedContent.Title;
            FilterSettingsWin.Show();
        }
    }
}
