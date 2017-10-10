using Micro.Future.Message.Business;
using Micro.Future.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public class ETFOTCOptionDataHandler : BaseMarketDataHandler
    {
        public ETFOTCOptionDataHandler()
        {
            MSG_ID_SUB_MD = (uint)BusinessMessageID.MSG_ID_SUB_PRICING;
            MSG_ID_UNSUB_MD = (uint)BusinessMessageID.MSG_ID_UNSUB_PRICING;
            MSG_ID_RET_MD = (uint)BusinessMessageID.MSG_ID_RTN_PRICING;
        }

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            base.OnMessageWrapperRegistered(messageWrapper);
            MessageWrapper.RegisterAction<PBPricingData, ExceptionMessage>(MSG_ID_RET_MD, OnReturningPricing, ErrorMsgAction);
        }

        private void OnReturningPricing(PBPricingData md)
        {
            var mktVM = FindMarketData(md.Contract);
            if (mktVM != null)
            {
                if (mktVM.BidPrice != md.BidPrice ||
                    mktVM.AskPrice != md.AskPrice)
                {
                    mktVM.BidPrice = md.BidPrice;
                    mktVM.AskPrice = md.AskPrice;
                    mktVM.BidSize = md.BidSize;
                    mktVM.AskSize = md.AskSize;
                    mktVM.MidPrice = (mktVM.BidPrice + mktVM.AskPrice) / 2;
                    RaiseNewMD(mktVM);
                }
            }
            else
            {
                UnsubMarketData(new[] { new ContractKeyVM(md.Exchange, md.Contract) });
            }
        }

        public override Task<IList<MarketDataVM>> SubMarketDataAsync(IEnumerable<ContractKeyVM> instrIDList, int timeout = 10000)
        {
            var tcs = new TimeoutTaskCompletionSource<IList<MarketDataVM>>(timeout);

            var serialId = NextSerialId;

            #region callback
            MessageWrapper.RegisterAction<PBPricingDataList, ExceptionMessage>
            (MSG_ID_SUB_MD,
            (resp) =>
            {
                if (resp.Header?.SerialId == serialId)
                {
                    tcs.TrySetResult(SubMDSuccessAction(resp));
                }
            },
            (bizErr) =>
            {
                if (bizErr.SerialId == serialId)
                    tcs.TrySetException(new MessageException(bizErr.MessageId, ErrorType.BIZ_ERROR, bizErr.Errorcode, bizErr.Description.ToStringUtf8()));
            }
            );
            #endregion

            SendMessage(serialId, MSG_ID_SUB_MD, instrIDList);

            return tcs.Task;
        }

        public override void SendMessage(uint serialId, uint msgId, IEnumerable<ContractKeyVM> instrIDList)
        {
            var sst = new PBInstrumentList();
            foreach (var instrID in instrIDList)
            {
                var pb = new PBInstrument { Contract = instrID.Contract };
                if (!string.IsNullOrEmpty(instrID.Exchange))
                    pb.Exchange = instrID.Exchange;

                sst.Instrument.Add(pb);
            }

            sst.Header = new DataHeader { SerialId = serialId };
            MessageWrapper.SendMessage(msgId, sst);
        }

        protected virtual IList<MarketDataVM> SubMDSuccessAction(PBPricingDataList marketList)
        {
            var ret = new List<MarketDataVM>();
            foreach (var md in marketList.PricingData)
            {
                MarketDataVM mktVM = FindMarketData(md.Contract);
                if (mktVM == null)
                {
                    mktVM = new MarketDataVM
                    {
                        Exchange = md.Exchange,
                        Contract = md.Contract
                    };
                    MarketDataMap[md.Contract] = new WeakReference<MarketDataVM>(mktVM);
                }

                ret.Add(mktVM);
            }

            return ret;
        }
    }
}