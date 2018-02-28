using System.Windows.Controls;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using System.Windows;
using System.Threading;
using System;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class RiskParamsControl : UserControl, IReloadData
    {
        private IList<ColumnObject> mColumns;
        private Timer _timer;
        private const int UpdateInterval = 2000;

        public string PersistanceId
        {
            get;
            set;
        }
        private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }
        public RiskParamsControl()
        {
            InitializeComponent();
            //Xceed.Wpf.Toolkit.DoubleUpDown a = new Xceed.Wpf.Toolkit.DoubleUpDown() {Text="test" };
            //RiskParamNameSP.Children.Add(new GroupBox() {Content = a, Header = "a" });
        }

        private void UpdateAccountInfoCallback(object state)
        {

        }

        public void ReloadData()
        {
            _timer = new Timer(UpdateAccountInfoCallback, null, UpdateInterval, UpdateInterval); 
        }

        private void MenuItemColumns_Click(object sender, RoutedEventArgs e)
        {
            ColumnSettingsWindow win = new ColumnSettingsWindow(mColumns);
            win.ShowDialog();
        }

        public void Initialize()
        {
        }

        public event Action<ModelParamsVM> OnModelSelected;

        private void RiskParamNameListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ModelParamsVM modelparamsVM = RiskParamNameListView.SelectedItem as ModelParamsVM;
            OnModelSelected?.Invoke(modelparamsVM);
        }
    }
}
