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
    public class MarketDataHandler : MessageHandlerTemplate<MarketDataHandler>
    {
        public const int RETRY_TIMES = 5;
        //public ObservableCollection<MarketDataVM> QuoteVMCollection
        //{
        //    get;
        //} = new ObservableCollection<MarketDataVM>();
        public event Action<MarketDataVM> OnNewMarketData;

        public ConcurrentDictionary<string, WeakReference<MarketDataVM>> MarketDataMap
        {
            get;
        } = new ConcurrentDictionary<string, WeakReference<MarketDataVM>>();

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            MessageWrapper.RegisterAction<SimpleStringTable, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_UNSUB_MARKETDATA, UnsubMDSuccessAction, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBMarketData, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_RET_MARKETDATA, RetMDSuccessAction, ErrorMsgAction);
        }

        public MarketDataVM SubMarketData(string instrID)
        {
            var mktDataList = SubMarketData(new string[] { instrID });
            return mktDataList.FirstOrDefault();
        }

        public IList<MarketDataVM> SubMarketData(IEnumerable<string> instrIDList)
        {
            var task = SubMarketDataAsync(instrIDList);
            task.Wait();
            
            return task.IsCompleted ? task.Result : new List<MarketDataVM>();
        }


        public Task<IList<MarketDataVM>> SubMarketDataAsync(IEnumerable<string> instrIDList, int timeout = 10000)
        {
            var msgId = (uint)BusinessMessageID.MSG_ID_SUB_MARKETDATA;

            var tcs = new TaskCompletionSource<IList<MarketDataVM>>(new CancellationTokenSource(timeout));

            var serialId = NextSerialId;

            #region callback
            MessageWrapper.RegisterAction<PBMarketDataList, ExceptionMessage>
            (msgId,
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

            SendMessage(serialId, msgId, instrIDList);

            return tcs.Task;
        }


        public void SendMessage(uint serialId, uint msgId, IEnumerable<string> instrIDList)
        {
            var instr = new NamedStringVector();
            instr.Name = (FieldName.INSTRUMENT_ID);

            foreach (string instrID in instrIDList)
            {
                instr.Entry.Add(instrID);
            }

            var sst = new SimpleStringTable();
            sst.Header = new DataHeader { SerialId = serialId };
            sst.Columns.Add(instr);

            MessageWrapper.SendMessage(msgId, sst);
        }

        public IList<MarketDataVM> ResubMarketData()
        {
            return SubMarketData(MarketDataMap.Keys);
        }

        public void UnsubMarketData(IEnumerable<MarketDataVM> quoteCollection)
        {
            List<string> instrList = new List<string>();
            foreach (var quoteVM in quoteCollection)
            {
                WeakReference<MarketDataVM> mktData;
                MarketDataMap.TryRemove(quoteVM.Contract, out mktData);
                instrList.Add(quoteVM.Contract);
            }

            UnsubMarketData(instrList);
        }

        public void UnsubMarketData(IEnumerable<string> instrIDList)
        {
            SendMessage(NextSerialId, (uint)BusinessMessageID.MSG_ID_UNSUB_MARKETDATA, instrIDList);
        }

        public MarketDataVM FindMarketData(string contract)
        {
            WeakReference<MarketDataVM> mktVMRef;
            MarketDataVM mktVM = null;
            if (MarketDataMap.TryGetValue(contract, out mktVMRef))
                mktVMRef.TryGetTarget(out mktVM);

            return mktVM;
        }

        protected IList<MarketDataVM> SubMDSuccessAction(PBMarketDataList marketList)
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

        protected void UnsubMDSuccessAction(SimpleStringTable strTbl)
        {
            foreach (var contract in strTbl.Columns[0].Entry)
            {
                WeakReference<MarketDataVM> mktVMRef;
                MarketDataMap.TryRemove(contract, out mktVMRef);
            }
        }

        protected void RetMDSuccessAction(PBMarketData md)
        {
            WeakReference<MarketDataVM> mktVMRef;
            if (MarketDataMap.TryGetValue(md.Contract, out mktVMRef))
            {
                MarketDataVM mktVM = null;
                if (mktVMRef.TryGetTarget(out mktVM))
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

                    OnNewMarketData?.Invoke(mktVM);
                }
                else
                {
                    UnsubMarketData(new[] { md.Contract });
                }
            }
        }

        protected void ErrorMsgAction(ExceptionMessage bizErr)
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
