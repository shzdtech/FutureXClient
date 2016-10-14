using Micro.Future.CustomizedControls.Controls;
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
using Xceed.Wpf.AvalonDock.Layout;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class OptionModelCtrl : UserControl, ILayoutAnchorableControl

    {
        private OTCOptionHandler _otcHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionHandler>();
        public OptionModelCtrl()
        {
            InitializeComponent();
            OpMarketControl.underlyingContractCB.SelectionChanged += UnderlyingContractCB_SelectionChanged;
            OpMarketControl.underlyingContractCB1.SelectionChanged += UnderlyingContractCB1_SelectionChanged;
        }

        private LayoutAnchorablePane _pane;
        public LayoutAnchorablePane AnchorablePane
        {
            get
            {
                return _pane;
            }
            set
            {
                _pane = value;
                WMSettingsLV.LayoutContent = _pane.SelectedContent;
            }
        }

        private async void UnderlyingContractCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var exchange = OpMarketControl.underlyingEX.SelectedValue;
            var uc = OpMarketControl.underlyingContractCB.SelectedValue;

            if (exchange != null && uc != null)
            {
                VolCurvLV.SelectOption(uc.ToString());
                var callputOpt = VolCurvLV.CallPutTDOptionVMCollection.FirstOrDefault();
                if(callputOpt != null && callputOpt.CallStrategyVM != null && callputOpt.CallStrategyVM.VolModel != null)
                {
                    var modelparamsVM = await _otcHandler.QueryModelParamsAsync(callputOpt.CallStrategyVM.VolModel);
                    WMSettingsLV.DataContext = modelparamsVM;
                }
            }
        }
        private void UnderlyingContractCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var exchange = OpMarketControl.underlyingEX1.SelectedValue;
            var uc = OpMarketControl.underlyingContractCB1.SelectedValue;

            if (exchange != null && uc != null)
            {
                VolCurvLV.SelectOption1(uc.ToString());
            }
        }

    }
}
