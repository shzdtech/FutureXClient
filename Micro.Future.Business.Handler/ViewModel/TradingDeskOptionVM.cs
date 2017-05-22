using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public class TradingDeskOptionVM : ContractKeyVM
    {
        public PricingVM MarketDataVM { get; set; }
        public VolatilityVM ImpliedVolVM { get; set; }
        public OptionPricingVM TheoDataVM { get; set; }
        public OptionPricingVM TempTheoDataVM { get; set; }
        public WingsReturnVM WingsReturnVM { get; set; }
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

        private int _longposition;
        public int LongPosition
        {
            get { return _longposition; }
            set
            {
                _longposition = value;
                OnPropertyChanged(nameof(LongPosition));
            }
        }
        private int _shortposition;
        public int ShortPosition
        {
            get { return _shortposition; }
            set
            {
                _shortposition = value;
                OnPropertyChanged(nameof(ShortPosition));
            }
        }
        private int _netposition;
        public int NetPosition
        {
            get { return _netposition; }
            set
            {
                _netposition = value;
                OnPropertyChanged(nameof(NetPosition));
            }
        }
        public void InitProperties()
        {
            MarketDataVM = new PricingVM();
            ImpliedVolVM = new VolatilityVM();
            TheoDataVM = new OptionPricingVM();
        }
    }
}
