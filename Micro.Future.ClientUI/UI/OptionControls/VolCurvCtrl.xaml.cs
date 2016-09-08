using Micro.Future.Message;
using Micro.Future.UI.OptionControls;
using Micro.Future.ViewModel;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Wpf;
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
            volPlot.DataContext = traderExHandler.VolatilityLinesVM;
            VegaPosition.Model = traderExHandler.OptionOxyVM.PlotModelBar;
            theoBidPutSC.MarkerOutline = CustomOxyMarkers.LDTriangle;
            theoBidCallSC.MarkerOutline = CustomOxyMarkers.RDTriangle;

            var internalSS = theoBidCallSC.CreateModel() as OxyPlot.Series.ScatterSeries;
            internalSS.MouseDown += InternalSS_MouseDown;

            traderExHandler.OnUpdateOption();
            traderExHandler.OnUpdateTest();

        }

        private void InternalSS_MouseDown(object sender, OxyMouseDownEventArgs e)
        {
            var bpSC = sender as OxyPlot.Series.ScatterSeries;
            if (bpSC != null)
            {
                TrackerHitResult nearestPoint = bpSC.GetNearestPoint(e.Position, false);
                if (nearestPoint != null)
                {
                    var point = nearestPoint.Item as ScatterPoint;
                    if (point != null)
                    {
                        // int index = (int)nearestPoint.Index;
                        point.Size = new Random().Next(10, 30);
                        bpSC.PlotModel.InvalidatePlot(false);
                    }
                }
            }
        }


        private void theoBidPutSC_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void theoBidCallSC_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }



        //public static PlotModel CustomMarkers()
        //{ }
    }



}
