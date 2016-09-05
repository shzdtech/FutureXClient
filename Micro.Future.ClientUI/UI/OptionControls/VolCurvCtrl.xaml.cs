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
    public partial class VolCurvCtrl : UserControl
    {
        public VolCurvCtrl()
        {
            InitializeComponent();
            Initialize();
        }

        public void Initialize()
        {
            var traderExHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();

            //PlotVolatility.Model = traderExHandler.OptionOxyVM.PlotModel;
            callBidLS.ItemsSource = traderExHandler.VolatilityLinesVM.CallBidVolLine;
            callBidLS.MarkerType = OxyPlot.MarkerType.Triangle;
            callAskLS.ItemsSource = traderExHandler.VolatilityLinesVM.CallAskVolLine;
            callMidLS.ItemsSource = traderExHandler.VolatilityLinesVM.CallMidVolLine;
            putBidLS.ItemsSource = traderExHandler.VolatilityLinesVM.PutBidVolLine;
            putAskLS.ItemsSource = traderExHandler.VolatilityLinesVM.PutAskVolLine;
            putMidLS.ItemsSource = traderExHandler.VolatilityLinesVM.PutMidVolLine;
            theoBidLS.ItemsSource = traderExHandler.VolatilityLinesVM.TheoBidVolLine;
            theoAskLS.ItemsSource = traderExHandler.VolatilityLinesVM.TheoAskVolLine;
            theoMidLS.ItemsSource = traderExHandler.VolatilityLinesVM.TheoMidVolLine;
            VegaPosition.Model = traderExHandler.OptionOxyVM.PlotModelBar;
            traderExHandler.OnUpdateOption();
            traderExHandler.OnUpdateTest();
        }

        private void theoBidLS_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }



}
