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
        //成交买卖方向
        private DirectionType _orderDirection;
        public DirectionType OrderDirection
        {
            get { return _orderDirection; }
            set
            {
                _orderDirection = value;
                OnPropertyChanged("OrderDirection");
            }
        }

        private int _position;
        public int Position
        {
            get { return _position; }
            set
            {
                _position = value;
                OnPropertyChanged(nameof(Position));
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
                OnPropertyChanged(nameof(YdPosition));
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
        public double Cost
        {
            get { return TdCost + YdCost; }
        }

        private double _tdCost;
        public double TdCost
        {
            get { return _tdCost; }
            set
            {
                _tdCost = value;
                OnPropertyChanged(nameof(Cost));
            }
        }

        private double _ydCost;
        public double YdCost
        {
            get { return _ydCost; }
            set
            {
                _ydCost = value;
                OnPropertyChanged(nameof(Cost));
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

        private double _avgPrice;
        public double AvgPrice
        {
            get { return _avgPrice; }
            set
            {
                _avgPrice = value;
                OnPropertyChanged("AvgPrice");
            }
        }
        private double _lastPrice;
        public double LastPrice
        {
            get { return _lastPrice; }
            set
            {
                _lastPrice = value;
                OnPropertyChanged("LastPrice");
            }
        }

        public int Multiplier
        {
            get;
            set;
        }
        private int _tdBuyPosition;
        public int TdBuyPosition
        {
            get { return _tdBuyPosition; }
            set
            {
                _tdBuyPosition = value;
                OnPropertyChanged("TdBuyPosition");
            }
        }
        private int _tdSellPosition;
        public int TdSellPosition
        {
            get { return _tdSellPosition; }
            set
            {
                _tdSellPosition = value;
                OnPropertyChanged("TdSellPosition");
            }
        }
        private int _ydBuyPosition;
        public int YdBuyPosition
        {
            get { return _ydBuyPosition; }
            set
            {
                _ydBuyPosition = value;
                OnPropertyChanged("YdBuyPosition");
            }
        }
        private int _ydSellPosition;
        public int YdSellPosition
        {
            get { return _ydSellPosition; }
            set
            {
                _ydSellPosition = value;
                OnPropertyChanged("YdSellPosition");
            }
        }
        private double _tdBuyAmount;
        public double TdBuyAmount
        {
            get { return _tdBuyAmount; }
            set
            {
                _tdBuyAmount = value;
                OnPropertyChanged("TdBuyAmount");
            }
        }
        private double _tdSellAmount;
        public double TdSellAmount
        {
            get { return _tdSellAmount; }
            set
            {
                _tdSellAmount = value;
                OnPropertyChanged("TdSellAmount");
            }
        }
        private double _ydBuyAmount;
        public double YdBuyAmount
        {
            get { return _ydBuyAmount; }
            set
            {
                _ydBuyAmount = value;
                OnPropertyChanged("YdBuyAmount");
            }
        }
        private double _ydSellAmount;
        public double YdSellAmount
        {
            get { return _ydSellAmount; }
            set
            {
                _ydSellAmount = value;
                OnPropertyChanged("YdSellAmount");
            }
        }
        private double _buyProfit;
        public double BuyProfit
        {
            get { return _buyProfit; }
            set
            {
                _buyProfit = value;
                OnPropertyChanged("BuyProfit");
            }
        }
        private double _sellProfit;
        public double SellProfit
        {
            get { return _sellProfit; }
            set
            {
                _sellProfit = value;
                OnPropertyChanged("SellProfit");
            }
        }
        private double _avgBuyPrice;
        public double AvgBuyPrice
        {
            get { return _avgBuyPrice; }
            set
            {
                _avgBuyPrice = value;
                OnPropertyChanged("AvgBuyPrice");
            }
        }
        private double _avgSellPrice;
        public double AvgSellPrice
        {
            get { return _avgSellPrice; }
            set
            {
                _avgSellPrice = value;
                OnPropertyChanged("AvgSellPrice");
            }
        }
        private double _ydAmount;
        public double YdAmount
        {
            get { return _ydAmount; }
            set
            {
                _ydAmount = value;
                OnPropertyChanged("YdAmount");
            }
        }
        private double _tdAmount;
        public double  TdAmount
        {
            get { return _tdAmount; }
            set
            {
                _tdAmount = value;
                OnPropertyChanged("TdAmount");
            }
        }

    }
}
