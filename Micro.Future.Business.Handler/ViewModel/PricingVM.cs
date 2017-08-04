using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public class PricingVM : ContractKeyVM
    {
        private DoubleChange _askPrice = new DoubleChange();
        public DoubleChange AskPrice
        {
            get { return _askPrice; }
            set
            {
                _askPrice.Value = value.Value;
                OnPropertyChanged(nameof(AskPrice));
            }
        }

        private DoubleChange _bidPrice = new DoubleChange();
        public DoubleChange BidPrice
        {
            get { return _bidPrice; }
            set
            {
                _bidPrice.Value = value.Value;
                OnPropertyChanged(nameof(BidPrice));
            }
        }

        private DoubleChange _midPrice = new DoubleChange();
        public DoubleChange MidPrice
        {
            get { return _midPrice; }
            set
            {
                _midPrice.Value = value.Value;
                OnPropertyChanged(nameof(MidPrice));
            }
        }

        private long bidSize;
        public long BidSize
        {
            get { return bidSize; }
            set
            {
                bidSize = value;
                OnPropertyChanged(nameof(BidSize));
            }
        }

        private long askSize;
        public long AskSize
        {
            get { return askSize; }
            set
            {
                askSize = value;
                OnPropertyChanged(nameof(AskSize));
            }
        }

        private double averagePrice;
        public double AveragePrice
        {
            get { return averagePrice; }
            set
            {
                averagePrice = value;
                OnPropertyChanged(nameof(AveragePrice));
            }
        }
        private double averagePriceMultiplier;
        public double AveragePriceMultiplier
        {
            get { return averagePriceMultiplier; }
            set
            {
                averagePriceMultiplier = value;
                OnPropertyChanged(nameof(AveragePriceMultiplier));
            }
        }
        private double highLimint;
        public double HighLimint
        {
            get { return highLimint; }
            set
            {
                highLimint = value;
                OnPropertyChanged(nameof(HighLimint));
            }
        }

        private double lowLimint;
        public double LowLimint
        {
            get { return lowLimint; }
            set
            {
                lowLimint = value;
                OnPropertyChanged(nameof(LowLimint));
            }
        }
        private double openInterest;
        public double OpenInterest
        {
            get { return openInterest; }
            set
            {
                openInterest = value;
                OnPropertyChanged(nameof(OpenInterest));
            }
        }
        private double preOpenInterest;
        public double PreOpenInterest
        {
            get { return preOpenInterest; }
            set
            {
                preOpenInterest = value;
                OnPropertyChanged(nameof(PreOpenInterest));
            }
        }
        private double priceChange;
        public double PriceChange
        {
            get { return priceChange; }
            set
            {
                priceChange = value;
                OnPropertyChanged(nameof(PriceChange));
            }
        }
        private int updateTime;
        public int UpdateTime
        {
            get { return updateTime; }
            set
            {
                updateTime = value;
                OnPropertyChanged(nameof(UpdateTime));
            }
        }

    }
}
