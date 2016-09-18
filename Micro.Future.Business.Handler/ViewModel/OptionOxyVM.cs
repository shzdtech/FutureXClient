using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using System.Collections.ObjectModel;
using OxyPlot.Axes;
using System.Windows;
using OxyPlot.Series;

namespace Micro.Future.ViewModel
{
    public class OptionOxyVM : ViewModelBase
    {

        public OptionOxyVM()
        {
            SetUpModel();
            SetUpModelBar();
        }
        public PlotModel PlotModel
        {
            get;
        } = new PlotModel();

        public PlotModel PlotModelBar
        {
            get;
        } = new PlotModel();


        private void SetUpModel()
        {
            //PlotModel.LegendTitle = "Legend";
            //PlotModel.LegendOrientation = LegendOrientation.Horizontal;
            //PlotModel.LegendPlacement = LegendPlacement.Outside;
            //PlotModel.LegendPosition = LegendPosition.TopRight;
            //PlotModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            //PlotModel.LegendBorder = OxyColors.Black;

            var volatilityAxis = new LinearAxis()
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = 10,
                MinimumPadding = 0,
                MaximumPadding = 0,
                Title = "StrikePrice"
            };
            PlotModel.Axes.Add(volatilityAxis);
            var strikepriceAxis = new LinearAxis()
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Position = AxisPosition.Left,
                Title = "Volatility"
            };
            PlotModel.Axes.Add(strikepriceAxis);

        }

        private void SetUpModelBar()
        {
            var strikepriceAxis1 = new CategoryAxis()
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Title = "StrikePrice",
                Position = AxisPosition.Bottom,
                IsTickCentered = false,
                //GapWidth =             
            };
            PlotModelBar.Axes.Add(strikepriceAxis1);

            var positionAxis = new LinearAxis()
            {
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Dot,
                Title = "Position",
                Position = AxisPosition.Left,
            };
            PlotModelBar.Axes.Add(positionAxis);
        }

        //private void LoadData(ModelParams modelParams)
        //{
        //    var lineSerie = new LineSeries()
        //    {
        //        StrokeThickness = 2,
        //        MarkerSize = 3,
        //        CanTrackerInterpolatePoints = false,
        //        Smooth = false,
        //    };
        //    foreach (var data in modelParams.Values)
        //    {
        //        lineSerie.Points.Add(new DataPoint(1, data));
        //    }

        //    PlotModel.Series.Add(lineSerie);


        //}

    }

    public class VolatilityLinesVM : ViewModelBase
    {
        //public static readonly DependencyProperty CallBidVolLineDataProperty =
        //    DependencyProperty.Register(nameof(CallBidVolLine), typeof(ObservableCollection<DataPoint>), typeof(LineSeries));
        public ObservableCollection<DataPoint> CallBidVolLine { get; }
            = new ObservableCollection<DataPoint>();

        //public static readonly DependencyProperty CallAskVolLineDataProperty =
        //   DependencyProperty.Register(nameof(CallAskVolLine), typeof(ObservableCollection<DataPoint>), typeof(LineSeries));
        public ObservableCollection<DataPoint> CallAskVolLine { get; }
            = new ObservableCollection<DataPoint>();

        //public static readonly DependencyProperty CallMidVolLineDataProperty =
        //   DependencyProperty.Register(nameof(CallMidVolLine), typeof(ObservableCollection<DataPoint>), typeof(LineSeries));
        public ObservableCollection<DataPoint> CallMidVolLine { get; }
            = new ObservableCollection<DataPoint>();

        //public static readonly DependencyProperty PutBidVolLineDataProperty =
        //   DependencyProperty.Register(nameof(PutBidVolLine), typeof(ObservableCollection<DataPoint>), typeof(LineSeries));
        public ObservableCollection<DataPoint> PutBidVolLine { get; }
             = new ObservableCollection<DataPoint>();

        //public static readonly DependencyProperty PutAskVolLineDataProperty =
        //   DependencyProperty.Register(nameof(PutAskVolLine), typeof(ObservableCollection<DataPoint>), typeof(LineSeries));
        public ObservableCollection<DataPoint> PutAskVolLine { get; }
            = new ObservableCollection<DataPoint>();

        //public static readonly DependencyProperty PutMidVolLineDataProperty =
        //   DependencyProperty.Register(nameof(PutMidVolLine), typeof(ObservableCollection<DataPoint>), typeof(LineSeries));
        public ObservableCollection<DataPoint> PutMidVolLine { get; }
            = new ObservableCollection<DataPoint>();

        //public static readonly DependencyProperty TheoBidVolLineDataProperty =
        //   DependencyProperty.Register(nameof(TheoBidVolLine), typeof(ObservableCollection<DataPoint>), typeof(LineSeries));
        public ObservableCollection<DataPoint> TheoBidVolLine { get; }
            = new ObservableCollection<DataPoint>();

        //public static readonly DependencyProperty TheoAskVolLineDataProperty =
        //   DependencyProperty.Register(nameof(TheoAskVolLine), typeof(ObservableCollection<DataPoint>), typeof(LineSeries));
        public ObservableCollection<DataPoint> TheoAskVolLine { get; }
            = new ObservableCollection<DataPoint>();

        //public static readonly DependencyProperty TheoMidVolLineDataProperty =
        //   DependencyProperty.Register(nameof(TheoMidVolLine), typeof(ObservableCollection<DataPoint>), typeof(LineSeries));
        public ObservableCollection<DataPoint> TheoMidVolLine { get; }
            = new ObservableCollection<DataPoint>();

        public ObservableCollection<ScatterPoint> CallAskVolScatter { get; }
            = new ObservableCollection<ScatterPoint>();
        public ObservableCollection<ScatterPoint> TheoBidPutVolScatter { get; }
            = new ObservableCollection<ScatterPoint>();

        public ObservableCollection<ScatterPoint> TheoBidCallVolScatter { get; }
            = new ObservableCollection<ScatterPoint>();

        public void ClearAll()
        {
            CallBidVolLine.Clear();
        }

    }
}
