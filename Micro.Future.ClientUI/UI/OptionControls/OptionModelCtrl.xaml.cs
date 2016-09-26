using Micro.Future.Message;
using Micro.Future.ViewModel;
using System;
using System.Collections.Generic;
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

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class OptionModelCtrl : UserControl
    {
        private OTCOptionHandler _otcHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionHandler>();
        public OptionModelCtrl()
        {
            InitializeComponent();
            OpMarketControl.underlyingContractCB.SelectionChanged += UnderlyingContractCB_SelectionChanged;
        }

        private async void UnderlyingContractCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var exchange = OpMarketControl.underlyingEX.SelectedValue;
            var uc = OpMarketControl.underlyingContractCB.SelectedValue;

            if (exchange != null && uc != null)
            {
                VolCurvLV.SelectOption(uc.ToString());
                var strategyVM = _otcHandler.StrategyVMCollection.FirstOrDefault(s => s.Contract == uc.ToString() && s.Exchange == exchange.ToString());
                //var modelparamsVM = await _otcHandler.QueryModelParamsAsync(strategyVM.VolModel);
                //WMSettingsLV.DataContext = modelparamsVM;
            }
        }
    }
}
