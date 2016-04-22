using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Micro.Future.ViewModel;

namespace Micro.Future.UI
{
    /// <summary>
    /// AdvancedMakeOrderWin.xaml 的交互逻辑
    /// </summary>
    public partial class AdvancedMakeOrderWin : Window
    {
        private PositionViewModel mLPositionViewModel;
        private PositionViewModel mSPositionViewModel;
        private QuoteViewModel mQuoteViewModel;
        private FundViewModel mFundViewModel;
        public AdvancedMakeOrderWin()
        {
            InitializeComponent();
        }

        public void SetBindingData(QuoteViewModel vmQuoteViewModel,
            PositionViewModel longPosition, 
            PositionViewModel shortPosition, 
            FundViewModel vmFundViewModel)
        {
            mQuoteViewModel = vmQuoteViewModel;
            mLPositionViewModel = longPosition;
            mSPositionViewModel = shortPosition;
            mFundViewModel = vmFundViewModel;
            FundGrid.DataContext = mFundViewModel;
            /// TBD
            /// 
            //for example by Lucas
            QuoteGrid.DataContext = mQuoteViewModel;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            volume.Value = int.Parse(((Button)sender).Content.ToString());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            volume.Value = int.Parse(((Button)sender).Content.ToString());
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            volume.Value = int.Parse(((Button)sender).Content.ToString());
        }
    }
}
