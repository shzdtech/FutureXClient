using System.Collections.Generic;
using System.Linq;
using System.Text;
using Micro.Future.Message.Business;
using System.Collections.ObjectModel;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.LocalStorage;
using Micro.Future;
using System;

namespace Micro.Future.Message
{
    public class SimMarketDataHandler : MessageHandlerTemplate<SimMarketDataHandler>
    {
        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            MessageWrapper.RegisterAction<PBMarketDataList, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_SUB_MARKETDATA, SubMDSuccessAction, ErrorMsgAction);
            MessageWrapper.RegisterAction<SimpleStringTable, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_UNSUB_MARKETDATA, UnsubMDSuccessAction, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBMarketData, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_RET_MARKETDATA, RetMDSuccessAction, ErrorMsgAction);
        }

        private void UnsubMDSuccessAction(SimpleStringTable obj)
        {

        }

        private void SubMDSuccessAction(PBMarketDataList obj)
        {
            
        }

        private void RetMDSuccessAction(PBMarketData obj)
        {
            
        }

        public void SendSimMarketData(MarketDataDO mdo)
        {
            var mktData = new PBMarketData();
            mktData.AskPrice.Add(mdo.AskPrice);
            mktData.BidPrice.Add(mdo.BidPrice);
            mktData.AskVolume.Add(mdo.AskSize);
            mktData.BidVolume.Add(mdo.BidSize);

            MessageWrapper?.SendMessage((uint)BusinessMessageID.MSG_ID_RET_MARKETDATA, mktData);
        }

        public void SubMarketData(string instrID)
        {
            SubMarketData(new []{ instrID });
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
    }
}
