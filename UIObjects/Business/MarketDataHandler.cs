using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PBWrapMsgMDA;
using Micro.Future.Util;
using System.Windows.Threading;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using Micro.Future.Message.Business;

namespace Micro.Future.Message
{
    public class MarketDataHandler : MessageHandlerTemplate<MarketDataHandler>
    {
        private ISet<string> mWatchList = new HashSet<string>();
        public ISet<string> WatchList
        {
            get { return mWatchList; }
        }
        public DispatchObservableCollection<QuoteViewModel> QuoteVMCollection
        {
            get;
            set;
        }

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            MessageWrapper.RegisterAction<StringResponse, BizErrorMsg>
            ((uint)BusinessMessageID.MSG_ID_SUB_MARKETDATA, SubMDSuccessAction, ErrorMsgAction);
            MessageWrapper.RegisterAction<StringResponse, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_UNSUB_MARKETDATA, UnsubMDSuccessAction, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBMarketDataList, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_RET_MARKETDATA, RetMDSuccessAction, ErrorMsgAction);
        }

        public void SubMarketData(string instrID)
        {
            SubMarketData(new List<string>() { instrID });
        }

        public void SubMarketData(IEnumerable<string> instrIDList)
        {
            var instr = NamedStringVector.CreateBuilder();
            instr.SetName(FieldName.INSTRUMENT_ID);

            foreach (string instrID in instrIDList)
            {
                instr.AddEntry(instrID);
            }

            var sst = SimpleStringTable.CreateBuilder();
            sst.AddColumns(instr);

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_SUB_MARKETDATA, sst.Build());
        }

        public void SubMarketData()
        {
            SubMarketData(this.WatchList);
        }

        public void ResubMarketData()
        {
            int cnt = QuoteVMCollection.Count;
            for (int i = 0; i < cnt; i++)
            {
                SubMarketData(QuoteVMCollection[i].Symbol);
            }
        }

        public void UnsubMarketData(IEnumerable<QuoteViewModel> quoteCollection)
        {
            List<string> instrList = new List<string>();
            foreach (var quoteVM in quoteCollection)
            {
                QuoteVMCollection.Remove(quoteVM);
                instrList.Add(quoteVM.Symbol);
            }

            UnsubMarketData(instrList);
        }

        public void UnsubMarketData(IEnumerable<string> instrIDList)
        {
            var instr = NamedStringVector.CreateBuilder();
            instr.SetName(FieldName.INSTRUMENT_ID);

            foreach (string instrID in instrIDList)
            {
                instr.AddEntry(instrID);
            }

            var sst = SimpleStringTable.CreateBuilder();
            sst.AddColumns(instr);

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_UNSUB_MARKETDATA, sst.Build());
        }

        public void UnsubMarketData()
        {
            UnsubMarketData(this.WatchList);
        }

        protected void SubMDSuccessAction(StringResponse strrsp)
        {
            if (QuoteVMCollection != null)
            {
                lock (QuoteVMCollection)
                {
                    var exists = (from q in QuoteVMCollection
                                  where string.Compare(q.Symbol, strrsp.Value, true) == 0
                                  select 1).Any();
                    if (!exists)
                    {
                        QuoteVMCollection.Dispatcher.Invoke(
                            () =>
                            {
                                QuoteVMCollection.Add(new QuoteViewModel() { Symbol = strrsp.Value });
                            });
                    }

                }
            }
        }

        protected void UnsubMDSuccessAction(StringResponse strrsp)
        {
            if (QuoteVMCollection != null)
            {
                lock (QuoteVMCollection)
                {
                    var row = from q in QuoteVMCollection
                              where string.Compare(q.Symbol, strrsp.Value, true) == 0
                              select q;

                    QuoteVMCollection.Dispatcher.Invoke(
                        () =>
                        {
                            foreach (var r in row)
                                QuoteVMCollection.Remove(r);
                        });

                }
            }
        }

        protected void RetMDSuccessAction(PBMarketDataList PB)
        {
            if (QuoteVMCollection != null)
            {
                QuoteVMCollection.Dispatcher.Invoke(
                    () =>
                    {
                        foreach (var md in PB.MdListList)
                        {
                            foreach (QuoteViewModel quote in QuoteVMCollection)
                            {
                                if (string.Compare(quote.Symbol, md.Symbol, true) == 0)
                                {
                                    quote.TimeStamp = md.TimeStamp;
                                    quote.MatchPrice = md.MatchPrice;
                                    quote.BidPrice = md.BidPriceList[0];
                                    quote.AskPrice = md.AskPriceList[0];
                                    quote.BidSize = md.BidVolumeList[0];
                                    quote.AskSize = md.AskVolumeList[0];
                                    quote.Volume = md.Volume;
                                    quote.OpenValue = md.OpenValue;
                                    quote.PreCloseValue = md.PreCloseValue;
                                    quote.HighValue = md.HighValue;
                                    quote.LowValue = md.LowValue;
                                    quote.UpperLimitPrice = md.HighLimit;
                                    quote.LowerLimitPrice = md.LowLimit;

                                    break;
                                }
                            }
                        }
                    });
            }
        }

        private void ErrorMsgAction(BizErrorMsg bizErr)
        {
            RaiseOnError(
                new MessageException(bizErr.MessageId, bizErr.Errorcode,
                Encoding.UTF8.GetString(bizErr.Description.ToByteArray()),
                bizErr.Syserrcode));
        }
    }
}
