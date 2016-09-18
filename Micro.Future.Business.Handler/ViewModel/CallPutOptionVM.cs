using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public class CallPutOptionVM
    {
        public OptionPricingVM CallOptionVM { get; set; }
        public OptionPricingVM PutOptionVM { get; set; }
        public double StrikePrice { get; set; }
    }


    public class CallPutTDOptionVM
    {
        public TradingDeskOptionVM CallOptionVM { get; set; }
        public TradingDeskOptionVM PutOptionVM { get; set; }
        public double StrikePrice { get; set; }
        public StrategyVM CallStrategyVM { get; set; }
        public StrategyVM PutStrategyVM { get; set; }
        public PositionVM PutPositionVM { get; set; }
        public PositionVM CallPositionVM { get; set; }
    }
}
