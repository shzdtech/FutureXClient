using System.Collections.Generic;
using System.Linq;
using System.Text;
using Micro.Future.ViewModel;
using Micro.Future.Message.Business;
using System.Threading;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public class BaseMarketDataHandler : AbstractMessageHandler
    {
        protected static uint MSG_ID_SUB_MD = (uint)BusinessMessageID.MSG_ID_SUB_MARKETDATA;
        protected static uint MSG_ID_UNSUB_MD = (uint)BusinessMessageID.MSG_ID_UNSUB_MARKETDATA;
        protected static uint MSG_ID_RET_MD = (uint)BusinessMessageID.MSG_ID_RET_MARKETDATA;

        public const int RETRY_TIMES = 5;
       
        public event Action<MarketDataVM> OnNewMarketData;


        protected void RaiseNewMD(MarketDataVM md)
        {
            OnNewMarketData?.Invoke(md);
        }

        public ConcurrentDictionary<string, WeakReference<MarketDataVM>> MarketDataMap
        {
            get;
        } = new ConcurrentDictionary<string, WeakReference<MarketDataVM>>();

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            MessageWrapper.RegisterAction<SimpleStringTable, ExceptionMessage>(MSG_ID_UNSUB_MD, UnsubMDSuccessAction, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBMarketData, ExceptionMessage>(MSG_ID_RET_MD, RetMDSuccessAction, ErrorMsgAction);
        }

        public virtual async Task<MarketDataVM> SubMarketDataAsync(string contract, string exchange = "")
        {
            var mktDataList = await SubMarketDataAsync(new [] { new ContractKeyVM(exchange, contract) });
            return mktDataList?.FirstOrDefault();
        }


        public virtual Task<IList<MarketDataVM>> SubMarketDataAsync(IEnumerable<ContractKeyVM> instrIDList, int timeout = 10000)
        {
            var tcs = new TimeoutTaskCompletionSource<IList<MarketDataVM>>(timeout);

            var serialId = NextSerialId;

            #region callback
            MessageWrapper.RegisterAction<PBMarketDataList, ExceptionMessage>
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


        public virtual void SendMessage(uint serialId, uint msgId, IEnumerable<ContractKeyVM> instrIDList)
        {
            var instr = new NamedStringVector();
            instr.Name = (FieldName.INSTRUMENT_ID);

            foreach (var instrID in instrIDList)
            {
                instr.Entry.Add(instrID.Contract);
            }

            var sst = new SimpleStringTable();
            sst.Header = new DataHeader { SerialId = serialId };
            sst.Columns.Add(instr);

            MessageWrapper.SendMessage(msgId, sst);
        }

        public virtual void UnsubMarketData(IEnumerable<ContractKeyVM> quoteCollection)
        {
            var instrList = new List<ContractKeyVM>();
            foreach (var quoteVM in quoteCollection)
            {
                WeakReference<MarketDataVM> mktData;
                MarketDataMap.TryRemove(quoteVM.Contract, out mktData);
                instrList.Add(quoteVM);
            }

            SendMessage(NextSerialId, MSG_ID_UNSUB_MD, instrList);
        }

        public MarketDataVM FindMarketData(string contract)
        {
            WeakReference<MarketDataVM> mktVMRef;
            MarketDataVM mktVM = null;
            if (MarketDataMap.TryGetValue(contract, out mktVMRef))
                mktVMRef.TryGetTarget(out mktVM);

            return mktVM;
        }

        protected virtual IList<MarketDataVM> SubMDSuccessAction(PBMarketDataList marketList)
        {
            var ret = new List<MarketDataVM>();
            foreach (var md in marketList.MarketData)
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

        protected virtual void UnsubMDSuccessAction(SimpleStringTable strTbl)
        {
            foreach (var contract in strTbl.Columns[0].Entry)
            {
                WeakReference<MarketDataVM> mktVMRef;
                MarketDataMap.TryRemove(contract, out mktVMRef);
            }
        }

        protected virtual void RetMDSuccessAction(PBMarketData md)
        {
            var mktVM = FindMarketData(md.Contract);
            if (mktVM != null)
            {
                if (mktVM.LastPrice != md.MatchPrice ||
                    mktVM.BidPrice != md.BidPrice[0] ||
                    mktVM.AskPrice != md.AskPrice[0] ||
                    mktVM.Volume != md.Volume)
                {
                    mktVM.LastPrice = md.MatchPrice;
                    mktVM.BidPrice = md.BidPrice[0];
                    mktVM.AskPrice = md.AskPrice[0];
                    mktVM.BidSize = md.BidVolume[0];
                    mktVM.AskSize = md.AskVolume[0];
                    mktVM.Volume = md.Volume;
                    mktVM.OpenValue = md.OpenValue;
                    mktVM.PreCloseValue = md.PreCloseValue;
                    mktVM.HighValue = md.HighValue;
                    mktVM.LowValue = md.LowValue;
                    mktVM.UpperLimitPrice = md.HighLimit;
                    mktVM.LowerLimitPrice = md.LowLimit;

                    RaiseNewMD(mktVM);
                }
            }
            else
            {
                UnsubMarketData(new[] { new ContractKeyVM(md.Exchange, md.Contract) });
            }
        }

        protected virtual void ErrorMsgAction(ExceptionMessage bizErr)
        {
            if (bizErr.Description != null)
            {
                var msg = bizErr.Description.ToByteArray();
                if (msg.Length > 0)
                    RaiseOnError(new MessageException(bizErr.MessageId, ErrorType.UNSPECIFIED_ERROR, bizErr.Errorcode,
                        Encoding.UTF8.GetString(msg)));
            }
        }
    }
}
