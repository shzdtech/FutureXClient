using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public class CallPutTDOptionVM : ViewModelBase
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
            set
            {
                OnPropertyChanged(nameof(TotalPosition));
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
                    int absCallPosition = System.Math.Abs(CallOptionVM.Position);
                    int absPutPosition = System.Math.Abs(PutOptionVM.Position);
                    int[] absPostion = new int[] { absCallPosition, absPutPosition };
                    if (CallOptionVM.Position > 0)
                        return absPostion.Min();
                    else if (CallOptionVM.Position < 0)
                        return -absPostion.Min();
                    else
                        return 0;
                }
            }
            set
            {
                OnPropertyChanged(nameof(MixFuture));
            }
        }

    }
}
