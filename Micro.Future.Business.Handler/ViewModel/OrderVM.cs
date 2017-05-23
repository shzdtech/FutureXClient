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
    public class OrderVM : ContractKeyVM
    {
        public OrderVM(BaseTraderHandler trdHdl)
        {
            TradeHandler = trdHdl;
        }

        public BaseTraderHandler TradeHandler { get; set; }

        private ulong _orderID;
        public ulong OrderID
        {
            get { return _orderID; }
            set
            {
                _orderID = value;
                OnPropertyChanged(nameof(OrderID));
            }
        }

        private ulong _orderSysID;
        public ulong OrderSysID
        {
            get { return _orderSysID; }
            set
            {
                _orderSysID = value;
                OnPropertyChanged(nameof(OrderSysID));
            }
        }

        private int _sessionID;
        public int SessionID
        {
            get { return _sessionID; }
            set
            {
                _sessionID = value;
                OnPropertyChanged(nameof(SessionID));
            }
        }

        private DirectionType _direction = DirectionType.BUY;
        public DirectionType Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                OnPropertyChanged(nameof(Direction));
            }
        }

        private double _limitPrice;
        public double LimitPrice
        {
            get { return _limitPrice; }
            set
            {
                _limitPrice = value;
                OnPropertyChanged(nameof(LimitPrice));
            }
        }

        private int _volume;
        public int Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                OnPropertyChanged(nameof(Volume));
            }
        }

        private int _volumeTraded;
        public int VolumeTraded
        {
            get { return _volumeTraded; }
            set
            {
                _volumeTraded = value;
                OnPropertyChanged(nameof(VolumeTraded));
            }
        }

        private int _volumeRemain;
        public int VolumeRemain
        {
            get { return _volumeRemain; }
            set
            {
                _volumeRemain = value;
                OnPropertyChanged(nameof(VolumeRemain));
            }
        }

        private OrderExecType _execType;
        public OrderExecType ExecType
        {
            get { return _execType; }
            set
            {
                _execType = value;
                OnPropertyChanged(nameof(ExecType));
            }
        }

        private OrderTIFType _tif;
        public OrderTIFType TIF
        {
            get { return _tif; }
            set
            {
                _tif = value;
                OnPropertyChanged(nameof(TIF));
            }
        }

        private OrderVolType _volType;
        public OrderVolType VolCondition
        {
            get { return _volType; }
            set
            {
                _volType = value;
                
                OnPropertyChanged(nameof(VolCondition));
            }
        }

        private TradingType _tradingType;
        public TradingType TradingType
        {
            get { return _tradingType; }
            set
            {
                _tradingType = value;
                OnPropertyChanged(nameof(TradingType));
            }
        }

        private bool _active;
        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                OnPropertyChanged(nameof(Active));
            }
        }

        private OrderStatus _status;
        public OrderStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        private OrderOpenCloseType _openClose;
        public OrderOpenCloseType OpenClose
        {
            get { return _openClose; }
            set
            {
                _openClose = value;
                OnPropertyChanged(nameof(OpenClose));
            }
        }


        private string _insertTime;
        public string InsertTime
        {
            get { return _insertTime; }
            set
            {
                _insertTime = value;
                OnPropertyChanged(nameof(InsertTime));
            }
        }

        private string _updateTime;
        public string UpdateTime
        {
            get { return _updateTime; }
            set
            {
                _updateTime = value;
                OnPropertyChanged(nameof(UpdateTime));
            }
        }

        private string _cancelTime;
        public string CancelTime
        {
            get { return _cancelTime; }
            set
            {
                _cancelTime = value;
                OnPropertyChanged(nameof(CancelTime));
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        public void SendOrder(object param = null)
        {
            //this.Direction = (string)directStr == "1" ? DirectionType.BUY : DirectionType.SELL;
            TradeHandler?.CreateOrder(this);
        }

        RelayCommand _sendOrderCommand;
        public ICommand SendOrderCommand
        {
            get
            {
                if (_sendOrderCommand == null)
                {
                    _sendOrderCommand = new RelayCommand(SendOrder);
                }
                return _sendOrderCommand;
            }
        }

        OrderConditionType _orderType;
        public OrderConditionType ConditionType
        {
            get { return _orderType; }
            set
            {
                _orderType = value;
                UpdateCondition(_orderType);
                OnPropertyChanged(nameof(ConditionType));
            }
        }

        public void UpdateCondition(OrderConditionType type)
        {
            if (type == OrderConditionType.LIMIT)
            {
                TIF = OrderTIFType.GFD;
                VolCondition = OrderVolType.ANYVOLUME;
            }
           else if (type == OrderConditionType.FAK)
            {
                TIF = OrderTIFType.IOC;
                VolCondition = OrderVolType.ANYVOLUME;
            }
            else if (type == OrderConditionType.FOK)
            {
                TIF = OrderTIFType.IOC;
                VolCondition = OrderVolType.ALLVOLUME;
            }

        }

    }
}
