using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public class TradingDeskOptionVM : ContractKeyVM
    {
        public OptionPricingVM MarketDataVM { get; } = new OptionPricingVM();
        public OptionPricingVM TheoDataVM { get; } = new OptionPricingVM();
        public OptionPricingVM TempTheoDataVM { get; set; }
        public WingsReturnVM WingsReturnVM { get; } = new WingsReturnVM();
    }
}
