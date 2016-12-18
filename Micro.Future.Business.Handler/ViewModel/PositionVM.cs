using Micro.Future.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Micro.Future.ViewModel
{
    //报价
    public class PositionVM : ContractKeyVM
    {
        ///持仓多空方向
        private PositionDirectionType _direction;
        public PositionDirectionType Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                OnPropertyChanged("Direction");
            }
        }

        private int _position;
        public int Position
        {
            get { return _position; }
            set
            {
                _position = value;
                OnPropertyChanged("Position");
            }
        }

        ///上日持仓
        private int _ydPosition;
        public int YdPosition
        {
            get { return _ydPosition; }
            set
            {
                _ydPosition = value;
                OnPropertyChanged("YdPosition");
            }
        }

        private int _todayPosition;

        public int TodayPosition
        {
            get { return _todayPosition; }
            set
            {
                _todayPosition = value;
                OnPropertyChanged(nameof(TodayPosition));
            }
        }

        ///持仓日期
        private PositionDateFlagType _positionDateFlag;
        public PositionDateFlagType PositionDateFlag
        {
            get { return _positionDateFlag; }
            set
            {
                _positionDateFlag = value;
                OnPropertyChanged(nameof(PositionDateFlag));
            }
        }

        ///开仓量
        private int _openVolume;
        public int OpenVolume
        {
            get { return _openVolume; }
            set
            {
                _openVolume = value;
                OnPropertyChanged("OpenVolume");
            }
        }

        ///平仓量
        private int _closeVolume;
        public int CloseVolume
        {
            get { return _closeVolume; }
            set
            {
                _closeVolume = value;
                OnPropertyChanged("CloseVolume");
            }
        }

        ///开仓金额
        private double _openAmount;
        public double OpenAmount
        {
            get { return _openAmount; }
            set
            {
                _openAmount = value;
                OnPropertyChanged("OpenAmount");
            }
        }

        ///平仓金额
        private double _closeAmount;
        public double CloseAmount
        {
            get { return _closeAmount; }
            set
            {
                _closeAmount = value;
                OnPropertyChanged("CloseAmount");
            }
        }

        ///持仓成本
        private double _cost;
        public double Cost
        {
            get { return _cost; }
            set
            {
                _cost = value;
                OnPropertyChanged("Cost");
            }
        }

        ///开仓成本
        private double _openCost;
        public double OpenCost
        {
            get { return _openCost; }
            set
            {
                _openCost = value;
                OnPropertyChanged("OpenCost");
            }
        }

        ///持仓盈亏
        private double _profit;
        public double Profit
        {
            get { return _profit; }
            set
            {
                _profit = value;
                OnPropertyChanged("Profit");
            }
        }

        ///平仓盈亏
        private double _closeProfit;
        public double CloseProfit
        {
            get { return _closeProfit; }
            set
            {
                _closeProfit = value;
                OnPropertyChanged("CloseProfit");
            }
        }


        ///占用的保证金
        private double _useMargin;
        public double UseMargin
        {
            get { return _useMargin; }
            set
            {
                _useMargin = value;
                OnPropertyChanged("UseMargin");
            }
        }

        ///投机套保标志
        private HedgeType _hedgeFlag;
        public HedgeType HedgeFlag
        {
            get { return _hedgeFlag; }
            set
            {
                _hedgeFlag = value;
                OnPropertyChanged("HedgeFlag");
            }
        }

        private string _cancelTime;
        public string CancelTime
        {
            get { return _cancelTime; }
            set
            {
                _cancelTime = value;
                OnPropertyChanged("CancelTime");
            }
        }

        private double _expiryMonth;
        public double ExpiryMonth
        {
            get { return _expiryMonth; }
            set
            {
                _expiryMonth = value;
                OnPropertyChanged("ExpiryMonth");
            }
        }

        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged("Selected");
            }
        }

        private double _strikePrice;
        public double StrikePrice
        {
            get { return _strikePrice; }
            set
            {
                _strikePrice = value;
                OnPropertyChanged("StrikePrice");
            }
        }

        private ProductType _productType;
        public ProductType ProductType
        {
            get { return _productType; }
            set
            {
                _productType = value;
                OnPropertyChanged(nameof(ProductType));
            }
        }

        private Style _style;
        public Style Style
        {
            get { return _style; }
            set
            {
                _style = value;
                OnPropertyChanged("Style");
            }
        }

        private double _delta;
        public double Delta
        {
            get { return _delta; }
            set
            {
                _delta = value;
                OnPropertyChanged("Delta");
            }
        }

        private double _vega;
        public double Vega
        {
            get { return _vega; }
            set
            {
                _vega = value;
                OnPropertyChanged("Vega");
            }
        }

        private double _positiondelta;
        public double PositionDelta
        {
            get { return _positiondelta; }
            set
            {
                _positiondelta = value;
                OnPropertyChanged("PositionDelta");
            }
        }

        private double _positionvega;
        public double PositionVega
        {
            get { return _positionvega; }
            set
            {
                _positionvega = value;
                OnPropertyChanged("PositionVega");
            }
        }

        private string _portfolio;
        public string Portfolio
        {
            get
            {
                return _portfolio;
            }
            set
            {
                _portfolio = value;
                OnPropertyChanged(nameof(Portfolio));
            }
        }

        private double _meanCost;
        public double MeanCost
        {
            get
            {
                return _meanCost;
            }
            set
            {
                _meanCost = value;
                OnPropertyChanged(nameof(MeanCost));
            }
        }
    }
}
