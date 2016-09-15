using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    //报价
    public class MarketDataVM : PricingVM
    {

        private double preCloseValue;
        public double PreCloseValue
        {
            get { return preCloseValue; }
            set
            {
                preCloseValue = value;
                OnPropertyChanged(nameof(PreCloseValue));
            }
        }

        private double openValue;
        public double OpenValue
        {
            get { return openValue; }
            set
            {
                openValue = value;
                OnPropertyChanged(nameof(OpenValue));
            }
        }

        private long turnover;
        public long Turnover
        {
            get { return turnover; }
            set
            {
                turnover = value;
                OnPropertyChanged(nameof(Turnover));
            }
        }

        private long volume;
        public long Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                OnPropertyChanged(nameof(Volume));
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

        private DoubleChange _lastPrice = new DoubleChange();
        public DoubleChange LastPrice
        {
            get { return _lastPrice; }
            set
            {
                _lastPrice.Value = value.Value;
                OnPropertyChanged("LastPrice");
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
