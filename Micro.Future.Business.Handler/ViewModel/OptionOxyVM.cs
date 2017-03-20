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
        public ObservableCollection<DataPoint> TheoBidVolLine { get; }
            = new ObservableCollection<DataPoint>();

        public ObservableCollection<DataPoint> TheoAskVolLine { get; }
            = new ObservableCollection<DataPoint>();

        public ObservableCollection<DataPoint> TheoMidVolLine { get; }
            = new ObservableCollection<DataPoint>();

        public ObservableCollection<DataPoint> TheoBidVolLine1 { get; }
    = new ObservableCollection<DataPoint>();

        public ObservableCollection<DataPoint> TheoAskVolLine1 { get; }
            = new ObservableCollection<DataPoint>();

        public ObservableCollection<DataPoint> TheoMidVolLine1 { get; }
            = new ObservableCollection<DataPoint>();

        public ObservableCollection<ScatterPoint> TheoPutAskVolScatter { get; }
            = new ObservableCollection<ScatterPoint>();
        public ObservableCollection<ScatterPoint> TheoPutBidVolScatter { get; }
            = new ObservableCollection<ScatterPoint>();

        public ObservableCollection<ScatterPoint> TheoCallAskVolScatter { get; }
           = new ObservableCollection<ScatterPoint>();
        public ObservableCollection<ScatterPoint> TheoCallBidVolScatter { get; }
            = new ObservableCollection<ScatterPoint>();
        private double _referencePriceLine;
        public double ReferencePriceLine
        {
            get
            {
                return _referencePriceLine;
            }
            set
            {
                _referencePriceLine = value;
                OnPropertyChanged(nameof(ReferencePriceLine));
            }
        }
        private double _ATMLine;
        public double ATMLine
        {
            get
            {
                return _ATMLine;
            }
            set
            {
                _ATMLine = value;
                OnPropertyChanged(nameof(ATMLine));
            }
        }
        private double _synFLine;
        public double SynFLine
        {
            get
            {
                return _synFLine;
            }
            set
            {
                _synFLine = value;
                OnPropertyChanged(nameof(SynFLine));
            }
        }
        private double _x0Line;
        public double X0Line
        {
            get
            {
                return _x0Line;
            }
            set
            {
                _x0Line = value;
                OnPropertyChanged(nameof(X0Line));
            }
        }
        private double _x1Line;
        public double X1Line
        {
            get
            {
                return _x1Line;
            }
            set
            {
                _x1Line = value;
                OnPropertyChanged(nameof(X1Line));
            }
        }
        private double _x2Line;
        public double X2Line
        {
            get
            {
                return _x2Line;
            }
            set
            {
                _x2Line = value;
                OnPropertyChanged(nameof(X2Line));
            }
        }
        private double _x3Line;
        public double X3Line
        {
            get
            {
                return _x3Line;
            }
            set
            {
                _x3Line = value;
                OnPropertyChanged(nameof(X3Line));
            }
        }
        public void ClearAll()
        {
            CallBidVolLine.Clear();
            CallAskVolLine.Clear();
            CallMidVolLine.Clear();
            PutBidVolLine.Clear();
            PutAskVolLine.Clear();
            PutMidVolLine.Clear();
            TheoBidVolLine.Clear();
            TheoAskVolLine.Clear();
            TheoMidVolLine.Clear();
            TheoBidVolLine1.Clear();
            TheoAskVolLine1.Clear();
            TheoMidVolLine1.Clear();
            TheoPutAskVolScatter.Clear();
            TheoPutBidVolScatter.Clear();
            TheoCallAskVolScatter.Clear();
            TheoCallBidVolScatter.Clear();
        }

    }
}
