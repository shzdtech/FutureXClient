using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Business.Handler.ViewModel
{
    public class CallPutOptionVM
    {
        public OptionMarketVM CallOptionVM { get; set; }
        public OptionMarketVM PutOptionVM { get; set; }
        public double StrikePrice { get; set; }
    }
}
