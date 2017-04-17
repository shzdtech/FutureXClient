using Micro.Future.CustomizedControls.Controls;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.ViewModel;
using Micro.Future.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Xceed.Wpf.Toolkit;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class WMSettingsCtrl : UserControl
    {
        public LayoutContent LayoutContent { get; set; }
        private IDictionary<string, double> TempSettings { get; set; } = new Dictionary<string, double>();
        private OTCOptionTradingDeskHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
        private IList<ContractInfo> _contractList;
        private IDictionary<ContractKeyVM, ContractInfo> _strategySet;
        private WingsReturnVM _wingsReturnVM = new WingsReturnVM();

        public WMSettingsCtrl()
        {
            InitializeComponent();

            current_Slope.DataContext = _wingsReturnVM;
            current_Slope1.DataContext = _wingsReturnVM;
            current_Volatility.DataContext = _wingsReturnVM;
            current_Volatility1.DataContext = _wingsReturnVM;
            var options = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OPTIONS);
            var otcOptions = ClientDbContext.GetContractFromCache((int)ProductType.PRODUCT_OTC_OPTION);
            _contractList = options.Union(otcOptions).ToList();

            _otcOptionHandler.OnTradingDeskOptionParamsReceived += OnTradingDeskOptionParamsReceived;
        }

        private void OnTradingDeskOptionParamsReceived(TradingDeskOptionVM tdOptionVM)
        {
            if (_strategySet != null && tdOptionVM.WingsReturnVM != null && _strategySet.ContainsKey(tdOptionVM))
            {
                _wingsReturnVM.SlopeCurr = tdOptionVM.WingsReturnVM.SlopeCurr;
                _wingsReturnVM.SlopeCurrOffset = tdOptionVM.WingsReturnVM.SlopeCurrOffset;
                _wingsReturnVM.VolCurr = tdOptionVM.WingsReturnVM.VolCurr;
                _wingsReturnVM.VolCurrOffset = tdOptionVM.WingsReturnVM.VolCurrOffset;
            }
        }

        //public ModelParamsVM VolatilityModelVM { get; } = new ModelParamsVM();


        private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null && e.OldValue != null && e.NewValue != null)
            {
                var modelParamsVM = updownctrl.DataContext as ModelParamsVM;
                if (modelParamsVM != null)
                {
                    var key = updownctrl.Tag.ToString();
                    double value = (double)e.NewValue;
                    TempSettings[key] = value;
                    _otcOptionHandler.UpdateTempModelParams(modelParamsVM.InstanceName, key, value);
                }
            }
        }
        private void DeleteTempSettings()
        {
            TempSettings.Clear();
        }

        private void SetReference_Click(object sender, RoutedEventArgs e)
        {
            if (TempSettings.Any())
            {
                var modelParamsVM = DataContext as ModelParamsVM;
                if (modelParamsVM != null)
                {
                    _otcOptionHandler.UpdateModelParams(modelParamsVM.InstanceName, TempSettings);
                    RevertCurrent();
                }
            }

        }
        private void RevertCurrent_Click(object sender, RoutedEventArgs e)
        {
            if (TempSettings.Any())
            {
                RevertCurrent();
            }
        }

        public void RevertCurrent()
        {
            var modelParamsVM = DataContext as ModelParamsVM;
            if (modelParamsVM != null)
            {
                DeleteTempSettings();
                _otcOptionHandler.RemoveTempModel(modelParamsVM.InstanceName);
            }
        }

        private void Spinned(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        {
            var updownctrl = sender as DoubleUpDown;
            if (updownctrl != null)
            {
                Task.Run(() => { Task.Delay(100); Dispatcher.Invoke(() => updownctrl.CommitInput()); });
            }
        }


        public void SelectOption(string exchange, string contract, string expiredate)
        {
            _strategySet = _contractList.Where(c =>
                c.UnderlyingContract == contract && c.ExpireDate == expiredate && c.Exchange == exchange)
                .ToDictionary(c => new ContractKeyVM(c.Exchange, c.Contract), c => c);
        }

    }
}
