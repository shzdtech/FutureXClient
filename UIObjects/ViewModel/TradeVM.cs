using Micro.Future.Message;

namespace Micro.Future.ViewModel
{
    //报价
    public class TradeVM : ContractNotifyPropertyChanged
    {
        private ulong _orderID;
        public ulong OrderID
        {
            get { return _orderID; }
            set
            {
                _orderID = value;
                OnPropertyChanged("OrderID");
            }
        }

        private ulong _orderSysID;
        public ulong OrderSysID
        {
            get { return _orderSysID; }
            set
            {
                _orderSysID = value;
                OnPropertyChanged("OrderSysID");
            }
        }

        private DirectionType _direction;
        public DirectionType Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                OnPropertyChanged("Direction");
            }
        }

        private double _price;
        public double Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged("Price");
            }
        }

        private int _volume;
        public int Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                OnPropertyChanged("Volume");
            }
        }




        private TradingType _tradingType;
        public TradingType TradingType
        {
            get { return _tradingType; }
            set
            {
                _tradingType = value;
                OnPropertyChanged("TradingType");
            }
        }

        private string _tradeTime;
        public string TradeTime
        {
            get { return _tradeTime; }
            set
            {
                _tradeTime = value;
                OnPropertyChanged("TradeTime");
            }
        }

        private string _tradeDate;
        public string TradeDate
        {
            get { return _tradeDate; }
            set
            {
                _tradeDate = value;
                OnPropertyChanged("TradeDate");
            }
        }


    }
}
