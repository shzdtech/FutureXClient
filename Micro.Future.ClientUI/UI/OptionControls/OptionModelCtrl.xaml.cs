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
        private VolCurvCtrl _volCurvCtrl = new VolCurvCtrl();
        private OpMarketData _opMarketData = new OpMarketData();
        private OTCOptionHandler _otcHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionHandler>();
        public OptionModelCtrl()
        {
            InitializeComponent();
            OpMarketControl.underlyingContractCB1.SelectionChanged += UnderlyingContractCB1_SelectionChanged;
        }

        private async void UnderlyingContractCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var exchange = OpMarketControl.underlyingEX.SelectedValue;
            var uc = OpMarketControl.underlyingContractCB1.SelectedValue;

            if (exchange != null && uc != null)
            {
                _volCurvCtrl.SelectOption(uc.ToString());
                var strategyVM = _otcHandler.StrategyVMCollection.FirstOrDefault(s => s.Contract == uc.ToString() && s.Exchange == exchange.ToString());
                var modelparamsVM = await _otcHandler.QueryModelParamsAsync(strategyVM.VolModel);
                //WMSettingsLV.DataContext = modelparamsVM;
            }
        }
    }
}
