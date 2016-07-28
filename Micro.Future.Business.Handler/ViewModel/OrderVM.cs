﻿using Micro.Future.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Micro.Future.ViewModel
{
    //报价
    public class OrderVM : ContractNotifyPropertyChanged
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

        private double _limitPrice;
        public double LimitPrice
        {
            get { return _limitPrice; }
            set
            {
                _limitPrice = value;
                OnPropertyChanged("LimitPrice");
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

        private int _volumeTraded;
        public int VolumeTraded
        {
            get { return _volumeTraded; }
            set
            {
                _volumeTraded = value;
                OnPropertyChanged("VolumeTraded");
            }
        }

        private int _volumeRemain;
        public int VolumeRemain
        {
            get { return _volumeRemain; }
            set
            {
                _volumeRemain = value;
                OnPropertyChanged("VolumeRemain");
            }
        }

        private OrderExecType _execType;
        public OrderExecType ExecType
        {
            get { return _execType; }
            set
            {
                _execType = value;
                OnPropertyChanged("ExecType");
            }
        }

        private OrderTIFType _tif;
        public OrderTIFType TIF
        {
            get { return _tif; }
            set
            {
                _tif = value;
                OnPropertyChanged("TIF");
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

        private bool _active;
        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                OnPropertyChanged("Active");
            }
        }

        private OrderStatus _status;
        public OrderStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        private OrderOffsetType _offsetFlag;
        public OrderOffsetType OffsetFlag
        {
            get { return _offsetFlag; }
            set
            {
                _offsetFlag = value;
                OnPropertyChanged("OffsetFlag");
            }
        }        


        private string _insertTime;
        public string InsertTime
        {
            get { return _insertTime; }
            set
            {
                _insertTime = value;
                OnPropertyChanged("InsertTime");
            }
        }

        private string _updateTime;
        public string UpdateTime
        {
            get { return _updateTime; }
            set
            {
                _updateTime = value;
                OnPropertyChanged("UpdateTime");
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

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }



        private void sendOrder(object param)
        {
            //this.Direction = (string)directStr == "1" ? DirectionType.BUY : DirectionType.SELL;
            MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>().
                CreateOrder(this);
        }

        RelayCommand _sendOrderCommand;
        public ICommand SendOrderCommand
        {
            get
            {
                if (_sendOrderCommand == null)
                {
                    _sendOrderCommand= new RelayCommand(sendOrder);
                }
                return _sendOrderCommand;
            }
        }

    }
}