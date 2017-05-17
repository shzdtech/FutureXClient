using Micro.Future.LocalStorage;
using Micro.Future.Utility;
using Micro.Future.ViewModel;
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
        }

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
                }
            }

        }


    }
}
