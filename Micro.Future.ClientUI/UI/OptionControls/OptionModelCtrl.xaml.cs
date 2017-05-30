using Micro.Future.CustomizedControls.Controls;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class OptionModelCtrl : UserControl, ILayoutAnchorableControl, IReloadData

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
            OpMarketControl.adjustment2.ValueChanged += Adjustment2_ValueChanged;
            OpMarketControl.contract2.SelectionChanged += Contract2CB1_SelectionChanged;

        }

        private void Adjustment2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.OldValue != null && e.NewValue != null)
            {
                VolCurvLV.TempCurveReset();
            }
        }
        private void Contract2CB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (OpMarketControl.contract2.SelectedValue!=null)
                VolCurvLV.TempCurveReset();
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

        public string PersistanceId
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
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
            if (WMSettingsLV.DataContext != null)
            {
                WMSettingsLV.RevertCurrent();
                VolCurvLV.TempCurveReset();
            }
            var volModel = OpMarketControl.volModelCB1.SelectedItem as ModelParamsVM;
            if (volModel != null)
            {
                await _otcHandler.QueryModelParamsAsync(volModel.ToString());
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
            //Task.Run(() => { Task.Delay(5000); Dispatcher.Invoke(() => VolCurvLV.TempCurveReset()); });        
            VolCurvLV.TempCurveReset();
        }

        public void Initialize()
        {
        }

        public void ReloadData()
        {
            Initialize();
            var layoutInfo = ClientDbContext.GetLayout(_otcHandler.MessageWrapper.User.Id, optionmodelDM.Uid);
            if (layoutInfo != null)
            {
                XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(optionmodelDM);

                using (var reader = new StringReader(layoutInfo.LayoutCFG))
                {
                    layoutSerializer.Deserialize(reader);
                }
            }
        }
        public void SaveLayout()
        {

            var layoutInfo = ClientDbContext.GetLayout(_otcHandler.MessageWrapper.User?.Id, optionmodelDM.Uid);

            XmlLayoutSerializer layoutSerializer = new XmlLayoutSerializer(optionmodelDM);
            var strBuilder = new StringBuilder();
            using (var writer = new StringWriter(strBuilder))
            {
                layoutSerializer.Serialize(writer);
            }
            ClientDbContext.SaveLayoutInfo(_otcHandler.MessageWrapper.User.Id, optionmodelDM.Uid, strBuilder.ToString());
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveLayout();
        }

    }
}
