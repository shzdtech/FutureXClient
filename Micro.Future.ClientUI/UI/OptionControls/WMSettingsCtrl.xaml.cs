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
        private OTCOptionHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionHandler>();
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
            if (_strategySet.ContainsKey(tdOptionVM))
            {
                _wingsReturnVM.SlopeCurr = tdOptionVM.WingsReturnVM.SlopeCurr;
                _wingsReturnVM.SlopeCurrOffset = tdOptionVM.WingsReturnVM.SlopeCurrOffset;
                _wingsReturnVM.VolCurr = tdOptionVM.WingsReturnVM.VolCurr;
                _wingsReturnVM.VolCurrOffset = tdOptionVM.WingsReturnVM.VolCurrOffset;
            }
        }

        //public ModelParamsVM VolatilityModelVM { get; } = new ModelParamsVM();

        private void OptionWin_KeyDown(object sender, KeyEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                if (e.Key == Key.Escape || e.Key == Key.Enter)
                {
                    if (_otcOptionHandler != null)
                    {
                        if (e.Key == Key.Enter)
                        {
                            var modelParamsVM = DataContext as ModelParamsVM;
                            TempSettings[ctrl.Tag.ToString()] = modelParamsVM[ctrl.Tag.ToString()].Value;
                            _otcOptionHandler.UpdateTempModelParams(modelParamsVM.InstanceName, ctrl.Tag.ToString(), modelParamsVM[ctrl.Tag.ToString()].Value);
                        }
                    }
                }
            }
        }

        private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                if (_otcOptionHandler != null)
                {
                    var modelParamsVM = DataContext as ModelParamsVM;
                    TempSettings[ctrl.Tag.ToString()] = modelParamsVM[ctrl.Tag.ToString()].Value;
                    _otcOptionHandler.UpdateTempModelParams(modelParamsVM.InstanceName, ctrl.Tag.ToString(), modelParamsVM[ctrl.Tag.ToString()].Value);
                }
            }
        }
        private void DeleteTempSettings()
        {
            TempSettings.Clear();
        }
        private void SetReference_Click(object sender, RoutedEventArgs e)
        {
            if (_otcOptionHandler != null)
            {
                var modelParamsVM = DataContext as ModelParamsVM;
                _otcOptionHandler.UpdateModelParams(modelParamsVM.InstanceName, TempSettings);
                _otcOptionHandler.RemoveTempModel(modelParamsVM.InstanceName);
                DeleteTempSettings();
            }

        }
        private void RevertCurrent_Click(object sender, RoutedEventArgs e)
        {
            if (_otcOptionHandler != null)
            {
                var modelParamsVM = DataContext as ModelParamsVM;
                _otcOptionHandler.RemoveTempModel(modelParamsVM.InstanceName);
                DeleteTempSettings();
            }
        }

        private void DataContext_Changed(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void Spinned(object sender, Xceed.Wpf.Toolkit.SpinEventArgs e)
        {
            var ctrl = sender as DoubleUpDown;
            if (ctrl != null)
            {
                if (_otcOptionHandler != null)
                {
                    var modelParamsVM = DataContext as ModelParamsVM;
                    TempSettings[ctrl.Tag.ToString()] = ctrl.Value.Value; // modelParamsVM[ctrl.Tag.ToString()].Value;
                    _otcOptionHandler.UpdateTempModelParams(modelParamsVM.InstanceName, ctrl.Tag.ToString(), ctrl.Value.Value);
                }
            }
        }

        private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                if (_otcOptionHandler != null)
                {
                    var modelParamsVM = DataContext as ModelParamsVM;
                    if (modelParamsVM != null)
                    {
                        TempSettings[ctrl.Tag.ToString()] = modelParamsVM[ctrl.Tag.ToString()].Value;
                        _otcOptionHandler.UpdateTempModelParams(modelParamsVM.InstanceName, ctrl.Tag.ToString(), modelParamsVM[ctrl.Tag.ToString()].Value);
                    }
                }
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
