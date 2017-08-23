
using Micro.Future.Message;

namespace Micro.Future.ViewModel
{
    //报价
    public class TradeInfoVM : TradeVM
    {
        public TradeInfoVM(BaseTraderHandler trdHdl)
        {
            TradeHandler = trdHdl;
        }
        public BaseTraderHandler TradeHandler { get; set; }
    }
}
