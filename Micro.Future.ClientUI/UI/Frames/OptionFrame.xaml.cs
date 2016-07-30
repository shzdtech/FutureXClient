using Micro.Future.Controls;
using Micro.Future.Message;
using Micro.Future.ViewModel;
using System;
using System.Collections;
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
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;

namespace Micro.Future.UI
{
    /// <summary>
    /// ClientOptionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OptionFrame : UserControl
    {
        private static OptionFrame clientOptionPage = null;

        public static OptionFrame getClientOptionPage() { if (clientOptionPage == null) clientOptionPage = new OptionFrame();
                                                                return clientOptionPage; }


        private CollectionViewSource _viewSource = new CollectionViewSource();
        private ColumnObject[] mColumns;



        public OptionVM OptionVM
        {
            get;
            private set;
        } = new OptionVM();

        public OptionFrame()
        {
            InitializeComponent();

            StrikePricePanel.DataContext = new NumericalSimVM();
            VolatilityPanel.DataContext = new OptionVM();
            VolatilityPanel1.DataContext = new OptionVM();
            price_greekLV.DataContext = new PriceGreekVM();
            volatilityLV.DataContext = new VolatilityVM();
            positionLV.DataContext = new PositionVM();
            riskLV.DataContext = new RiskVM();
           
            mColumns = ColumnObject.GetColumns(listView_numSim);
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tabControl_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        public IEnumerable ExpirationMonthCollection
        {
            set
            {
                contractExpirationMonth.ItemsSource = value;
            }
        }

        

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void tabControl_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {

        }

        private void miniSteps_KeyUp(object sender, KeyEventArgs e)
        {

        }

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
    }
}

