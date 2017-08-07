using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public class CallPutTDOptionVM
    {
        public TradingDeskOptionVM CallOptionVM { get; set; }
        public TradingDeskOptionVM PutOptionVM { get; set; }
        public double StrikePrice { get; set; }
        public StrategyVM CallStrategyVM { get; set; }
        public StrategyVM PutStrategyVM { get; set; }
        public double TotalPosition
        {
            get
            {
                return CallOptionVM.Position + PutOptionVM.Position;
            }
        }
        public double MixFuture
        {
            get
            {
                if (PutOptionVM.Position * CallOptionVM.Position >= 0)
                    return 0;
                else
                {
                    double absCallPosition = System.Math.Abs(CallOptionVM.Position);
                    double absPutPosition = System.Math.Abs(PutOptionVM.Position);
                    double[] absPostion = new double[] { absCallPosition, absPutPosition };
                    if (CallOptionVM.Position > 0)
                        return absPostion.Min();
                    else if (CallOptionVM.Position < 0)
                        return -absPostion.Min();
                    else
                        return 0;
                }
            }
        }

    }
}
