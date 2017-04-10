using Micro.Future.CustomizedControls.Controls;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
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
        private OTCOptionTradingDeskHandler _otcHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();

        public OptionModelCtrl()
        {
            InitializeComponent();
            OpMarketControl.expireDateCB.SelectionChanged += ExpireDateCB_SelectionChanged;
            OpMarketControl.expireDateCB1.SelectionChanged += ExpireDateCB1_SelectionChanged;
            //OpMarketControl.underlyingContractCB1.SelectionChanged += UnderlyingContractCB1_SelectionChanged;
            OpMarketControl.volModelCB1.SelectionChanged += VolModelCB1_SelectionChanged;
            WMSettingsLV.revertCurrentBtn.Click += RevertCurrentBtn_Click;
            WMSettingsLV.setReferenceBtn.Click += SetCurrentBtn_Click;
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

        private void ExpireDateCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var exchange = OpMarketControl.underlyingEX.SelectedValue;
            var uc = OpMarketControl.underlyingContractCB.SelectedValue;
            var ed = OpMarketControl.expireDateCB.SelectedValue;

            if (exchange != null && uc != null && ed != null)
            {
                VolCurvLV.SelectOptionImpl(exchange.ToString(), uc.ToString(), ed.ToString());
            }
        }
        private void ExpireDateCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var exchange = OpMarketControl.underlyingEX1.SelectedValue;
            var uc = OpMarketControl.underlyingContractCB1.SelectedValue;
            var ed = OpMarketControl.expireDateCB1.SelectedValue;

            if (exchange != null && uc != null && ed != null)
            {
                VolCurvLV.SelectOption(exchange.ToString(), uc.ToString(), ed.ToString());
                WMSettingsLV.SelectOption(exchange.ToString(), uc.ToString(), ed.ToString());
            }
        }

        private async void VolModelCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WMSettingsLV.RevertCurrent();
            VolCurvLV.TempCurveReset();
            var volModel = OpMarketControl.volModelCB1.SelectedItem as ModelParamsVM;            
            if (volModel != null)
            {
                await _otcHandler.QueryModelParamsAsync(volModel.ToString());
            }

            if (volModel != null)
            {
                WMSettingsLV.DataContext = null;
                WMSettingsLV.DataContext = volModel;
            }
            //var exchange = OpMarketControl.underlyingEX1.SelectedValue;
            //var uc = OpMarketControl.underlyingContractCB1.SelectedValue;
            //var ed = OpMarketControl.expireDateCB1.SelectedValue;

            //if (exchange != null && uc != null && ed != null)
            //{
            //    VolCurvLV.SelectOption(exchange.ToString(), uc.ToString(), ed.ToString());
            //    WMSettingsLV.SelectOption(exchange.ToString(), uc.ToString(), ed.ToString());
            //}
        }
        private async void RevertCurrentBtn_Click(object sender, RoutedEventArgs e)
        {
            VolCurvLV.TempCurveReset();
            var volModel = OpMarketControl.volModelCB1.SelectedItem as ModelParamsVM;            
            if (volModel != null)
            {
                WMSettingsLV.DataContext = null;
                var modelparamsVM = await _otcHandler.QueryModelParamsAsync(volModel.ToString());
                WMSettingsLV.DataContext = modelparamsVM;
            }
        }
        private void SetCurrentBtn_Click(object sender, RoutedEventArgs e)
        {
            VolCurvLV.TempCurveReset();
        }
    }
}
