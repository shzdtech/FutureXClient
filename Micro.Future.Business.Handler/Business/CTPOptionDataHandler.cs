using Micro.Future.Business.Handler.ViewModel;
using Micro.Future.Message.Business;
using Micro.Future.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public class CTPOptionDataHandler : MarketDataHandler
    {


        public ObservableCollection<CallPutOptionVM> CallPutOptionVMCollection
        {
            get;
        } = new ObservableCollection<CallPutOptionVM>();

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            base.OnMessageWrapperRegistered(messageWrapper);
            MessageWrapper.RegisterAction<PBMarketData, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_RET_MARKETDATA, RetOptionSuccessAction, ErrorMsgAction);
        }

        public void SubCallPutOptionData(IList<double> strikeList, IList<string> callList, IList<string> putList)
        {
            for(int i=0;i< callList.Count;i++)
            {
                var callOption = new OptionMarketVM { Contract = callList[i] };
                var putOption = new OptionMarketVM { Contract = putList[i] };
                CallPutOptionVMCollection.Add(new CallPutOptionVM()
                {
                    StrikePrice = strikeList[i],
                    CallOptionVM = callOption,
                    PutOptionVM = putOption
                });
            }

            SubMarketData(callList);
            SubMarketData(putList);
        }

        protected void RetOptionSuccessAction(PBMarketData md)
        {
            OptionMarketVM quote = null;
            var cp = CallPutOptionVMCollection.FirstOrDefault((pb) => string.Compare(pb.PutOptionVM.Contract, md.Contract, true) == 0);
            if(cp != null)
            {
                quote = cp.PutOptionVM;
            }
            else
            {
                cp = CallPutOptionVMCollection.FirstOrDefault((pb) => string.Compare(pb.CallOptionVM.Contract, md.Contract, true) == 0);
                if (cp != null)
                {
                    quote = cp.CallOptionVM;
                }
            }

            if (quote != null)
            {
                quote.LastPrice = md.MatchPrice;
                quote.BidPrice = md.BidPrice[0];
                quote.AskPrice = md.AskPrice[0];
                quote.BidSize = md.BidVolume[0];
                quote.AskSize = md.AskVolume[0];
                quote.Volume = md.Volume;
                quote.OpenValue = md.OpenValue;
                quote.PreCloseValue = md.PreCloseValue;
                quote.HighValue = md.HighValue;
                quote.LowValue = md.LowValue;
                quote.UpperLimitPrice = md.HighLimit;
                quote.LowerLimitPrice = md.LowLimit;
            }

        }
    }
}