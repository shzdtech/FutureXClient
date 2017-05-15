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
        public void InitProperties()
        {
            MarketDataVM = new PricingVM();
            ImpliedVolVM = new VolatilityVM();
            TheoDataVM = new OptionPricingVM();
        }
    }
}
