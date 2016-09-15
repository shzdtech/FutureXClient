﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public class TradingDeskOptionVM : ContractNotifyPropertyChanged
    {
        public OptionPricingVM MarketDataVM { get; } = new OptionPricingVM();
        public OptionPricingVM TheoDataVM { get; } = new OptionPricingVM();
    }
}
