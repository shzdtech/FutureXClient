using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;


namespace Micro.Future.ViewModel
{
    using Message;
    using OxyPlot.Axes;
    using OxyPlot.Series;
    public class OptionOxyPlot : ViewModelBase
    {

        private PlotModel plotModel;
        public PlotModel PlotModel
        {
            get { return plotModel; }
            set { plotModel = value; OnPropertyChanged("PlotModel"); }
        }

        private void SetUpModel()
        {
            PlotModel.LegendTitle = "Legend";
            PlotModel.LegendOrientation = LegendOrientation.Horizontal;
            PlotModel.LegendPlacement = LegendPlacement.Outside;
            PlotModel.LegendPosition = LegendPosition.TopRight;
            PlotModel.LegendBackground = OxyColor.FromAColor(200, OxyColors.White);
            PlotModel.LegendBorder = OxyColors.Black;

            var volatilityAxis = new LinearAxis() { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "Value" };
            PlotModel.Axes.Add(volatilityAxis);
            var strikepriceAxis = new LinearAxis() { MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot, Title = "Value" };
            PlotModel.Axes.Add(strikepriceAxis);
        }

        private void LoadData(ModelParams modelParams)
        {
            var lineSerie = new LineSeries()
            {
                StrokeThickness = 2,
                MarkerSize = 3,
                CanTrackerInterpolatePoints = false,
                Smooth = false,
            };
            foreach (var data in modelParams.Values)
            {
                lineSerie.Points.Add(new DataPoint(1, data));
            }

            PlotModel.Series.Add(lineSerie);


        }

    }
}
