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
            MessageWrapper.RegisterAction<PBMarketDataList, BizErrorMsg>
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

        protected void SubMDSuccessAction(PBMarketDataList marketList)
        {
            if (QuoteVMCollection != null)
            {
                lock (QuoteVMCollection)
                {
                    foreach (var md in marketList.MdListList)
                    {
                        if (!QuoteVMCollection.Exist((quote) => string.Compare(quote.Contract, md.Contract, true) == 0))
                        {
                            QuoteVMCollection.Add(new QuoteViewModel() { Contract = md.Contract });
                        }
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
                              where string.Compare(q.Contract, strrsp.Value, true) == 0
                              select q;

                    foreach (var r in row)
                        QuoteVMCollection.Remove(r);

                }
            }
        }

        protected void RetMDSuccessAction(PBMarketDataList PB)
        {
            foreach (var md in PB.MdListList)
            {
                var quote = QuoteVMCollection.Find((pb) => string.Compare(pb.Contract, md.Contract, true) == 0);
                if (quote != null)
                {
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
                }
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
