using Micro.Future.Message;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Micro.Future.ViewModel
{
    public class TradeDifferVM : ContractKeyVM
    {
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

        private OrderOpenCloseType _openClose;
        public OrderOpenCloseType OpenClose
        {
            get { return _openClose; }
            set
            {
                _openClose = value;
                OnPropertyChanged("OpenClose");
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
        private int _dbTrade;
        public int DBTrade
        {
            get { return _dbTrade; }
            set
            {
                _dbTrade = value;
                OnPropertyChanged("DBTrade");
            }
        }

        private int _sysTrade;
        public int SysTrade
        {
            get { return _sysTrade; }
            set
            {
                _sysTrade = value;
                OnPropertyChanged(nameof(SysTrade));
            }
        }
    }
}
