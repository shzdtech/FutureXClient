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

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class WMSettingsCtrl : UserControl
    {
        private VolModelSettingsWindow _volModelSettingsWin = new VolModelSettingsWindow();
        public LayoutContent LayoutContent { get; set; }
        public WMSettingsCtrl()
        {
            InitializeComponent();
            _volModelSettingsWin.OnNaming += _volModelSettingsWin_OnNaming;
        }
        public ObservableCollection<StrategyVM> StrategyVMCollection
        {
            get;
        } = new ObservableCollection<StrategyVM>();

        
        private AbstractOTCHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>();


        private void _volModelSettingsWin_OnNaming(string volModelName)
        {
            if (LayoutContent != null)
                LayoutContent.Title = _volModelSettingsWin.VolModelTabTitle;
            var handler = _otcOptionHandler;
            foreach (var volmodelVM in StrategyVMCollection)
            {
                volmodelVM.VolModel = volModelName;
                handler.UpdateStrategy(volmodelVM);
            }
        }
        public ModelParamsVM ModelParams
        {
            get;
        } = new ModelParamsVM();

        private void OptionWin_KeyDown(object sender, KeyEventArgs e)
        {
            Control ctrl = sender as Control;
            if (ctrl != null)
            {
                if (e.Key == Key.Escape || e.Key == Key.Enter)
                {

                    OptionVM optionVM = ctrl.DataContext as OptionVM;
                    if (optionVM != null)
                    {
                        if (e.Key == Key.Enter)
                            optionVM.UpdateOptionParam();
                        else
                        {
                            ctrl.DataContext = null;
                            ctrl.DataContext = optionVM;
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

        public static explicit operator TabItem(WMSettingsCtrl v)
        {
            throw new NotImplementedException();
        }

        private void VolModel_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            _volModelSettingsWin.Show();
        }

    }
}
