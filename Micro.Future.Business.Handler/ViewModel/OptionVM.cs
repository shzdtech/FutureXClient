using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Micro.Future.ViewModel
{
    using System.Collections.Generic;
    using OxyPlot;
    using Message;

    public class OptionVM : ViewModelBase
    {
        public List<DataPoint> Points { get; set; }

        public string Title { get; private set; }


        private double _volatilityReference;

        public double VolatilityReference
        {
            get { return _volatilityReference; }
            set
            {
                _volatilityReference = value;
                OnPropertyChanged("VolatilityReference");
            }
        }

        private double _currentVolatility;

        public double CurrentVolatility
        {
            get { return _currentVolatility; }
            set
            {
                _currentVolatility = value;
                OnPropertyChanged("CurrentVolatility");
            }
        }

        private double _currentSlope;

        public double CurrentSlope
        {
            get { return _currentSlope; }
            set
            {
                _currentSlope = value;
                OnPropertyChanged("CurrentSlope");
            }
        }

        private double _volatilityChangeRate;

        public double VolatilityChangeRate
        {
            get { return _volatilityChangeRate; }
            set
            {
                _volatilityChangeRate = value;
                OnPropertyChanged("VolatilityChangeRate");
            }
        }

        private double _slopeReference;

        public double SlopeReference
        {
            get { return _slopeReference; }
            set
            {
                _slopeReference = value;
                OnPropertyChanged("SlopeReference");
            }
        }

        private double _slopeChangeRate;

        public double SlopeChangeRate
        {
            get { return _slopeChangeRate; }
            set
            {
                _slopeChangeRate = value;
                OnPropertyChanged("SlopeChangeRate");
            }
        }

        private double _putCurvature;

        public double PutCurvature
        {
            get { return _putCurvature; }
            set
            {
                _putCurvature = value;
                OnPropertyChanged("PutCurvature");
            }
        }

        private double _callCurvature;

        public double CallCurvature
        {
            get { return _callCurvature; }
            set
            {
                _callCurvature = value;
                OnPropertyChanged("CallCurvature");
            }
        }

        private double _downCutoff;

        public double DownCutoff
        {
            get { return _downCutoff; }
            set
            {
                _downCutoff = value;
                OnPropertyChanged("DownCutoff");
            }
        }

        private double _upCutoff;

        public double UpCutoff
        {
            get { return _upCutoff; }
            set
            {
                _upCutoff = value;
                OnPropertyChanged("UpCutoff");
            }
        }

        private int _downSmoothingRange;
        public int DownSmoothingRange
        {
            get { return _downSmoothingRange; }
            set
            {
                _downSmoothingRange = value;
                OnPropertyChanged("DownSmoothingRange");
            }
        }

        private double _upSmoothingRange;

        public double UpSmoothingRange
        {
            get { return _upSmoothingRange; }
            set
            {
                _upSmoothingRange = value;
                OnPropertyChanged("UpSmoothingRange");
            }
        }

        private double _downSlope;

        public double DownSlope
        {
            get { return _downSlope; }
            set
            {
                _downSlope = value;
                OnPropertyChanged("DownSlope");
            }
        }

        private double _upSlope;

        public double UpSlope
        {
            get { return _upSlope; }
            set
            {
                _upSlope = value;
                OnPropertyChanged("UpSlope");
            }
        }

        private double _volatilityReference1;

        public double VolatilityReference1
        {
            get { return _volatilityReference1; }
            set
            {
                _volatilityReference1 = value;
                OnPropertyChanged("VolatilityReference1");
            }
        }

        private double _currentVolatility1;

        public double CurrentVolatility1
        {
            get { return _currentVolatility1; }
            set
            {
                _currentVolatility1 = value;
                OnPropertyChanged("CurrentVolatility1");
            }
        }

        private double _currentSlope1;

        public double CurrentSlope1
        {
            get { return _currentSlope1; }
            set
            {
                _currentSlope1 = value;
                OnPropertyChanged("CurrentSlope1");
            }
        }

        private double _volatilityChangeRate1;

        public double VolatilityChangeRate1
        {
            get { return _volatilityChangeRate1; }
            set
            {
                _volatilityChangeRate1 = value;
                OnPropertyChanged("VolatilityChangeRate1");
            }
        }

        private double _slopeReference1;

        public double SlopeReference1
        {
            get { return _slopeReference1; }
            set
            {
                _slopeReference1 = value;
                OnPropertyChanged("SlopeReference1");
            }
        }

        private double _slopeChangeRate1;

        public double SlopeChangeRate1
        {
            get { return _slopeChangeRate1; }
            set
            {
                _slopeChangeRate1 = value;
                OnPropertyChanged("SlopeChangeRate1");
            }
        }

        private double _putCurvature1;

        public double PutCurvature1
        {
            get { return _putCurvature1; }
            set
            {
                _putCurvature1 = value;
                OnPropertyChanged("PutCurvature1");
            }
        }

        private double _callCurvature1;

        public double CallCurvature1
        {
            get { return _callCurvature1; }
            set
            {
                _callCurvature1 = value;
                OnPropertyChanged("CallCurvature1");
            }
        }

        private double _downCutoff1;

        public double DownCutoff1
        {
            get { return _downCutoff1; }
            set
            {
                _downCutoff1 = value;
                OnPropertyChanged("DownCutoff1");
            }
        }

        private double _upCutoff1;

        public double UpCutoff1
        {
            get { return _upCutoff1; }
            set
            {
                _upCutoff1 = value;
                OnPropertyChanged("UpCutoff1");
            }
        }

        private int _downSmoothingRange1;
        public int DownSmoothingRange1
        {
            get { return _downSmoothingRange1; }
            set
            {
                _downSmoothingRange1 = value;
                OnPropertyChanged("DownSmoothingRange1");
            }
        }

        private double _upSmoothingRange1;

        public double UpSmoothingRange1
        {
            get { return _upSmoothingRange1; }
            set
            {
                _upSmoothingRange1 = value;
                OnPropertyChanged("UpSmoothingRange1");
            }
        }

        private double _downSlope1;

        public double DownSlope1
        {
            get { return _downSlope1; }
            set
            {
                _downSlope1 = value;
                OnPropertyChanged("DownSlope1");
            }
        }

        private double _upSlope1;

        public double UpSlope1
        {
            get { return _upSlope1; }
            set
            {
                _upSlope1 = value;
                OnPropertyChanged("UpSlope1");
            }
        }


        private double _sSR;

        public double SSR
        {
            get { return _sSR; }
            set
            {
                _sSR = value;
                OnPropertyChanged("SSR");
            }
        }

        private double _volatility;

        public double Volatility
        {
            get { return _volatility; }
            set
            {
                _volatility = value;
                OnPropertyChanged("SSR");
            }
        }

        public void UpdateOptionParam()
        {
            MessageHandlerContainer.DefaultInstance.Get<OTCMDTradingDeskHandler>().
                UpdateOptionParam(this);
        }

        RelayCommand _updateOPCommand;
        public ICommand UpdateOptionVMCommand
        {
            get
            {
                if (_updateOPCommand == null)
                {
                    _updateOPCommand = new RelayCommand(
                            param => UpdateOptionParam()
                        );
                }
                return _updateOPCommand;
            }
        }




    }

    class test
    {
        private OptionVM _ootvm;

        void Update(double x, double y)
        {
            _ootvm.Points.Add(new DataPoint(x, y));
        }
    }
}
