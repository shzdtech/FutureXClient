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
    }
}
