using System.Collections.Generic;
using System.Linq;
using System.Text;
using Micro.Future.ViewModel;
using Micro.Future.Message.Business;
using System.Collections.ObjectModel;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.UI;
using Micro.Future.LocalStorage;
using Micro.Future;


namespace Micro.Future.Message
{
    public class MarketDataHandler : MessageHandlerTemplate<MarketDataHandler>
    {
        public ObservableCollection<MarketDataVM> QuoteVMCollection
        {
            get;
        } = new ObservableCollection<MarketDataVM>();

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            MessageWrapper.RegisterAction<PBMarketDataList, ExceptionMessage>
            ((uint)BusinessMessageID.MSG_ID_SUB_MARKETDATA, SubMDSuccessAction, ErrorMsgAction);
            MessageWrapper.RegisterAction<SimpleStringTable, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_UNSUB_MARKETDATA, UnsubMDSuccessAction, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBMarketData, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_RET_MARKETDATA, RetMDSuccessAction, ErrorMsgAction);
        }

        public MarketDataVM SubMarketData(string instrID)
        {
            MarketDataVM mktVM = QuoteVMCollection.FirstOrDefault(((quote) => string.Compare(quote.Contract, instrID, true) == 0));
            if (mktVM == null)
            {
                mktVM = new MarketDataVM() { Contract = instrID };
                QuoteVMCollection.Add(mktVM);
                SubMarketData(new[] { instrID });
            }
            return mktVM;
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

            MessageWrapper?.SendMessage((uint)BusinessMessageID.MSG_ID_SUB_MARKETDATA, sst);
        }

        public void ResubMarketData()
        {
            int cnt = QuoteVMCollection.Count;
            for (int i = 0; i < cnt; i++)
            {
                SubMarketData(QuoteVMCollection[i].Contract);
            }
        }

        public void UnsubMarketData(IEnumerable<MarketDataVM> quoteCollection)
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

            MessageWrapper?.SendMessage((uint)BusinessMessageID.MSG_ID_UNSUB_MARKETDATA, sst);
        }

        protected void SubMDSuccessAction(PBMarketDataList marketList)
        {
            if (QuoteVMCollection != null)
            {
                lock (QuoteVMCollection)
                {
                    foreach (var md in marketList.MarketData)
                    {
                        MarketDataVM mktVM = QuoteVMCollection.FirstOrDefault(((quote) => string.Compare(quote.Contract, md.Contract, true) == 0));
                        if(mktVM == null)
                        {
                            mktVM = new MarketDataVM();
                            QuoteVMCollection.Add(mktVM);
                        }
                        mktVM.Exchange = md.Exchange;
                        mktVM.Contract = md.Contract;
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
                        var quote = QuoteVMCollection.FirstOrDefault((pb) => string.Compare(pb.Contract, contract, true) == 0);
                        if (quote != null)
                            QuoteVMCollection.Remove(quote);
                    }
                }
            }
        }

        protected void RetMDSuccessAction(PBMarketData md)
        {
            var quote = QuoteVMCollection.FirstOrDefault((pb) => string.Compare(pb.Contract, md.Contract, true) == 0);
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

        protected void ErrorMsgAction(ExceptionMessage bizErr)
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


        // 保存个人合约信息
        private void saveContract()
        {
            using (var clientCtx = new ClientDbContext())
            {
                var queryPersonalContract = from query in clientCtx.PersonalContract select query;

                if (queryPersonalContract.Any() == true)
                {
                    clientCtx.PersonalContract.RemoveRange(queryPersonalContract);
                    clientCtx.SaveChanges();
                }

                foreach (var data in MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().QuoteVMCollection)
                {
                    clientCtx.PersonalContract.Add(new PersonalContract() { UserID = int.Parse(MessageWrapper.User.Id), Contract = data.Contract });
                }

                clientCtx.SaveChanges();
            }
        }
    }
}
