using System.Collections.Generic;
using System.Linq;
using System.Text;
using Micro.Future.ViewModel;
using Micro.Future.Message.Business;
using System.Collections.ObjectModel;

namespace Micro.Future.Message
{
    public class MarketDataHandler : MessageHandlerTemplate<MarketDataHandler>
    {
        private ISet<string> mWatchList = new HashSet<string>();
        public ISet<string> WatchList
        {
            get { return mWatchList; }
        }
        public ObservableCollection<QuoteViewModel> QuoteVMCollection
        {
            get;
        } = new ObservableCollection<QuoteViewModel>();

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            MessageWrapper.RegisterAction<PBMarketDataList, ExceptionMessage>
            ((uint)BusinessMessageID.MSG_ID_SUB_MARKETDATA, SubMDSuccessAction, ErrorMsgAction);
            MessageWrapper.RegisterAction<SimpleStringTable, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_UNSUB_MARKETDATA, UnsubMDSuccessAction, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBMarketDataList, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_RET_MARKETDATA, RetMDSuccessAction, ErrorMsgAction);
        }

        public void SubMarketData(string instrID)
        {
            SubMarketData(new List<string>() { instrID });
        }

        public void SubMarketData(IEnumerable<string> instrIDList)
        {
            var instr = new NamedStringVector();
            instr.Name = (FieldName.INSTRUMENT_ID);

            foreach (string instrID in instrIDList)
            {
                instr.Entry.Add(instrID);
            }

            var sst = new SimpleStringTable();
            sst.Columns.Add(instr);

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_SUB_MARKETDATA, sst);
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
                SubMarketData(QuoteVMCollection[i].Contract);
            }
        }

        public void UnsubMarketData(IEnumerable<QuoteViewModel> quoteCollection)
        {
            List<string> instrList = new List<string>();
            foreach (var quoteVM in quoteCollection)
            {
                QuoteVMCollection.Remove(quoteVM);
                instrList.Add(quoteVM.Contract);
            }

            UnsubMarketData(instrList);
        }

        public void UnsubMarketData(IEnumerable<string> instrIDList)
        {
            var instr = new NamedStringVector();
            instr.Name = (FieldName.INSTRUMENT_ID);

            foreach (string instrID in instrIDList)
            {
                instr.Entry.Add(instrID);
            }

            var sst = new SimpleStringTable();
            sst.Columns.Add(instr);

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_UNSUB_MARKETDATA, sst);
        }

        public void UnsubMarketData()
        {
            UnsubMarketData(this.WatchList);
        }

        protected void SubMDSuccessAction(PBMarketDataList marketList)
        {
            if (QuoteVMCollection != null)
            {
                lock (QuoteVMCollection)
                {
                    foreach (var md in marketList.MdList)
                    {
                        if (!QuoteVMCollection.Exist((quote) => string.Compare(quote.Contract, md.Contract, true) == 0))
                        {
                            QuoteVMCollection.Add(new QuoteViewModel()
                            {
                                Exchange = md.Exhange,
                                Contract = md.Contract
                            });
                        }
                    }
                }
            }
        }

        protected void UnsubMDSuccessAction(SimpleStringTable strTbl)
        {
            if (QuoteVMCollection != null)
            {
                lock (QuoteVMCollection)
                {
                    foreach (var contract in strTbl.Columns[0].Entry)
                    {
                        var quote = QuoteVMCollection.Find((pb) => string.Compare(pb.Contract, contract, true) == 0);
                        if (quote != null)
                            QuoteVMCollection.Remove(quote);
                    }
                }
            }
        }

        protected void RetMDSuccessAction(PBMarketDataList PB)
        {
            foreach (var md in PB.MdList)
            {
                var quote = QuoteVMCollection.Find((pb) => string.Compare(pb.Contract, md.Contract, true) == 0);
                if (quote != null)
                {
                    quote.MatchPrice = md.MatchPrice;
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

        private void ErrorMsgAction(ExceptionMessage bizErr)
        {
            if (bizErr.Description != null)
            {
                var msg = bizErr.Description.ToByteArray();
                if (msg.Length > 0)
                    RaiseOnError(
                        new MessageException(bizErr.MessageId, ErrorType.UNSPECIFIED_ERROR, bizErr.Errorcode,
                        Encoding.UTF8.GetString(msg)));
            }
        }
    }
}
