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
    }
}
