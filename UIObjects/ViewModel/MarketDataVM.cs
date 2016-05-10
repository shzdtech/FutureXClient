using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    //报价
    public class QuoteViewModel : ContractNotifyPropertyChanged
    {
        private double preCloseValue;
        public double PreCloseValue
        {
            get { return preCloseValue; }
            set
            {
                preCloseValue = value;
                OnPropertyChanged("PreCloseValue");
            }
        }

        private double openValue;
        public double OpenValue
        {
            get { return openValue; }
            set
            {
                openValue = value;
                OnPropertyChanged("OpenValue");
            }
        }

        private long turnover;
        public long Turnover
        {
            get { return turnover; }
            set
            {
                turnover = value;
                OnPropertyChanged("Turnover");
            }
        }

        private long volume;
        public long Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                OnPropertyChanged("Volume");
            }
        }

        private double highValue;
        public double HighValue
        {
            get { return highValue; }
            set
            {
                highValue = value;
                OnPropertyChanged("HighValue");
            }
        }

        private double lowValue;
        public double LowValue
        {
            get { return lowValue; }
            set
            {
                lowValue = value;
                OnPropertyChanged("LowValue");
            }
        }

        private DoubleChange matchPrice = new DoubleChange();
        public DoubleChange MatchPrice
        {
            get { return matchPrice; }
            set
            {
                matchPrice.Value = value.Value;
                OnPropertyChanged("MatchPrice");
            }
        }

        private DoubleChange askPrice = new DoubleChange();
        public DoubleChange AskPrice
        {
            get { return askPrice; }
            set
            {
                askPrice.Value = value.Value;
                OnPropertyChanged("AskPrice");
            }
        }

        private long askSize;
        public long AskSize
        {
            get { return askSize; }
            set
            {
                askSize = value;
                OnPropertyChanged("AskSize");
            }
        }

        private DoubleChange bidPrice = new DoubleChange();
        public DoubleChange BidPrice
        {
            get { return bidPrice; }
            set
            {
                bidPrice.Value = value.Value;
                OnPropertyChanged("BidPrice");
            }
        }

        private long bidSize;
        public long BidSize
        {
            get { return bidSize; }
            set
            {
                bidSize = value;
                OnPropertyChanged("BidSize");
            }
        }

        private long upperlimitprice;

        public long UpperLimitPrice
        {
            get { return upperlimitprice; }
            set
            {
                upperlimitprice = value;
                OnPropertyChanged("UpperLimitPrice");
            }
        }

        private long lowerlimitprice;

        public long LowerLimitPrice
        {
            get { return lowerlimitprice; }
            set
            {
                lowerlimitprice = value;
                OnPropertyChanged("LowerLimitPrice");
            }
        }
    }
}
