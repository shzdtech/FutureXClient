using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Micro.Future.Util;
using System.Windows.Threading;
using PBMsgTrader;
using Google.ProtocolBuffers;
using System.Threading;
using Micro.Future.ViewModel;
using System.Windows;
using Micro.Future.UI;
using Micro.Future.Message;
using Micro.Future.Message.Business;

namespace Micro.Future.Message
{
    public class TradeHandler : MessageHandlerTemplate<TradeHandler>
    {
        //private int mOrderRefBase;
        private int mFrequency;
        private int mOrderActionRef = 1;
        public bool HasLogin { get; set; }

        //private int ogMsgId;
        private string ogMsgService;
        // gzs add 20140109 start
        private Dictionary<string, PBMsgQueryRspInstrumentInfo> mInstrumentInfo = new Dictionary<string, PBMsgQueryRspInstrumentInfo>();
        private Dictionary<string, PBMsgQueryRspPosition.Builder> mSPosition = new Dictionary<string, PBMsgQueryRspPosition.Builder>();

        public Dictionary<string, PBMsgQueryRspPosition.Builder> SPosition
        {
            get { return mSPosition; }
            set { mSPosition = value; }
        }
        private Dictionary<string, PBMsgQueryRspPosition.Builder> mLPosition = new Dictionary<string, PBMsgQueryRspPosition.Builder>();

        public Dictionary<string, PBMsgQueryRspPosition.Builder> LPosition
        {
            get { return mLPosition; }
            set { mLPosition = value; }
        }
        // gzs add 20140109 end

        public SignInOptions UserInfo { get; set; }

        public ExecutionCollection ExecutionViewModel { get; set; }

        public ObservableCollection<PositionViewModel> Positions { get; set; }
        public FundViewModel Fund { get; set; }

        //private delegate void DoProcess(PBMsgTrader.PBMsgTrader trade);
        //private Dictionary<int, Action<PBMsgTrader.PBMsgTrader>> mProcessMap;

        public void Initialize(SignInOptions userinfo, Dictionary<string, string> manConfig)
        {
            this.ogMsgService = manConfig["OutgoingService"];

            HasLogin = false;
            mFrequency = int.Parse(manConfig["QueryFrequency"]);

            Positions = new ObservableCollection<PositionViewModel>();

            ExecutionViewModel = new ExecutionCollection();
            Fund = new FundViewModel();

            UserInfo = userinfo;

            //init process map
            //mProcessMap = new Dictionary<int, Action<PBMsgTrader.PBMsgTrader>>();
            //mProcessMap.Add((int)MsgIdLogin.ID_LOGIN_RSP, new Action<PBMsgTrader.PBMsgTrader>(OnLogin));
            //mProcessMap.Add((int)MsgIdQueryRsp.ID_QUERY_RSP_MARKET_INFO, new Action<PBMsgTrader.PBMsgTrader>(OnMarketInfo));
            //mProcessMap.Add((int)MsgIdQueryRsp.ID_QUERY_RSP_INSTRUMENT_INFO, new Action<PBMsgTrader.PBMsgTrader>(OnInstrumentInfo));
            //mProcessMap.Add((int)MsgIdQueryRsp.ID_QUERY_RSP_POSITION, new Action<PBMsgTrader.PBMsgTrader>(OnPosition));
            //mProcessMap.Add((int)MsgIdQueryRsp.ID_QUERY_RSP_FUND, new Action<PBMsgTrader.PBMsgTrader>(OnFund));
            //mProcessMap.Add((int)MsgIdQueryRsp.ID_QUERY_RSP_ORDER, new Action<PBMsgTrader.PBMsgTrader>(OnQueryOrder));
            //mProcessMap.Add((int)MsgIdOrder.ID_ORDER_RTN_ORDER, new Action<PBMsgTrader.PBMsgTrader>(OnReturnOrder));
            //mProcessMap.Add((int)MsgIdQueryRsp.ID_QUERY_RSP_TRADE, new Action<PBMsgTrader.PBMsgTrader>(OnQueryTrade));
            //mProcessMap.Add((int)MsgIdOrder.ID_ORDER_RTN_TRADE, new Action<PBMsgTrader.PBMsgTrader>(OnReturnTrade));
            //mProcessMap.Add((int)MsgIdOrder.ID_ORDER_RSP_CAN, new Action<PBMsgTrader.PBMsgTrader>(OnCancel));
        }

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            MessageWrapper.RegisterAction<PBMsgTrader.PBMsgQueryRspMarketInfo, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_QUERY_EXCHANGE, OnMarketInfo, null);
            MessageWrapper.RegisterAction<PBMsgTrader.PBMsgQueryRspInstrumentInfo, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_QUERY_INSTRUMENT, OnInstrumentInfo, null);
            MessageWrapper.RegisterAction<PBMsgTrader.PBMsgQueryRspPosition, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_QUERY_POSITION, OnPosition, null);
            MessageWrapper.RegisterAction<PBMsgTrader.PBMsgQueryRspFund, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_QUERY_ACCOUNT_INFO, OnFund, null);
            MessageWrapper.RegisterAction<PBMsgTrader.PBMsgOrderRtn, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_QUERY_ORDER, OnQueryOrder, null);
            MessageWrapper.RegisterAction<PBMsgTrader.PBMsgOrderRtn, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_ORDER_UPDATE, OnUpdateOrder, null);
            MessageWrapper.RegisterAction<PBMsgTrader.PBMsgTradeRtn, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_QUERY_TRADE, OnQueryTrade, null);
            MessageWrapper.RegisterAction<PBMsgTrader.PBMsgTradeRtn, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_TRADE_RTN, OnReturnTrade, null);
            MessageWrapper.RegisterAction<PBMsgTrader.PBMsgOrderAction, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_ORDER_CANCEL, OnCancel, null);
        }


        public void SettlementInfoConfirm()
        {
            MainWindow.MyInstance.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action<string>(MainWindow.MyInstance.PringToStatus),
                "确认结算结果进行中...");
            //PBMsgTrader.PBMsgTrader.Builder pbTrade = PBMsgTrader.PBMsgTrader.CreateBuilder();
            //pbTrade.SetMsgId((int)MsgIdOther.ID_REQ_SETTLEMENTINFOCONFIRM);

            //PBMsgSettlementInfoConfirm.Builder pb = PBMsgSettlementInfoConfirm.CreateBuilder();
            //pb.SetMsgId((int)MsgIdOther.ID_REQ_SETTLEMENTINFOCONFIRM);
            //pb.SetBrokerID(UserInfo.BrokerID);
            //pb.SetInvestorID(UserInfo.UserID);
            //pb.SetConfirmDate("");
            //pb.SetConfirmTime("");

            var pb = SimpleStringTable.CreateBuilder();

            pb.AddColumns(
                NamedStringVector.CreateBuilder().
                SetName(FieldName.BROKER_ID).AddEntry(UserInfo.BrokerID));
            pb.AddColumns(
                NamedStringVector.CreateBuilder().
                SetName(FieldName.USER_ID).AddEntry(UserInfo.UserID));

            //if (producerBeReady)
            //{
            //sendMsgToBroker(pbTrade.Build().ToByteArray());
            //}
            this.MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_SETTLEMENT_INFO_CONFIRM,
                pb.Build());


            Logger.Debug("SettlementInfoConfirm");
        }

        public void RequestMarketInfo()
        {
            MainWindow.MyInstance.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action<string>(MainWindow.MyInstance.PringToStatus),
                "查询市场信息进行中...");
            //PBMsgTrader.PBMsgTrader.Builder pbTrade = PBMsgTrader.PBMsgTrader.CreateBuilder();
            //pbTrade.SetMsgId((int)MsgIdQueryReq.ID_QUERY_REQ_MARKET_INFO);
            //PBMsgQueryReqMarketInfo.Builder pb = PBMsgQueryReqMarketInfo.CreateBuilder();
            //pb.SetExchangeID("");
            //pb.SetMsgId((int)MsgIdQueryReq.ID_QUERY_REQ_MARKET_INFO);
            //pbTrade.SetMsgQueryReqMarketInfo(pb);
            //if (producerBeReady)
            //{
            //    sendMsgToBroker(pbTrade.Build().ToByteArray());
            //}
            var pb = SimpleStringTable.CreateBuilder();

            pb.AddColumns(
                NamedStringVector.CreateBuilder().
                SetName(FieldName.EXCHANGE_ID).AddEntry(string.Empty));

            this.MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_EXCHANGE,
                pb.Build());

            Logger.Debug("RequestMarketInfo");
        }

        public void RequestInstrumentInfo()
        {
            MainWindow.MyInstance.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action<string>(MainWindow.MyInstance.PringToStatus),
                "查询合约进行中...");
            //PBMsgTrader.PBMsgTrader.Builder pbTrade = PBMsgTrader.PBMsgTrader.CreateBuilder();
            //pbTrade.SetMsgId((int)MsgIdQueryReq.ID_QUERY_REQ_INSTRUMENT_INFO);

            //PBMsgQueryReqInstrumentInfo.Builder pb = PBMsgQueryReqInstrumentInfo.CreateBuilder();

            var pb = SimpleStringTable.CreateBuilder();

            pb.AddColumns(
                NamedStringVector.CreateBuilder().
                SetName(FieldName.EXCHANGE_ID).AddEntry(string.Empty));
            pb.AddColumns(
                NamedStringVector.CreateBuilder().
                SetName(FieldName.INSTRUMENT_ID).AddEntry(string.Empty));
            pb.AddColumns(
                NamedStringVector.CreateBuilder().
                SetName(FieldName.EXCHANGE_INSTRUMENT_ID).AddEntry(string.Empty));
            pb.AddColumns(
                NamedStringVector.CreateBuilder().
                SetName(FieldName.PRODUCT_ID).AddEntry(string.Empty));


            //pb.SetMsgId((int)MsgIdQueryReq.ID_QUERY_REQ_INSTRUMENT_INFO);

            //if (producerBeReady)
            //{
            this.MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_INSTRUMENT,
                    pb.Build());
            //}

            Logger.Debug("RequestInstrumentInfo");
        }

        public void RequestPositionInfo()
        {
            MainWindow.MyInstance.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action<string>(MainWindow.MyInstance.PringToStatus),
                "查询持仓进行中...");
            //PBMsgTrader.PBMsgTrader.Builder pbTrade = PBMsgTrader.PBMsgTrader.CreateBuilder();
            //pbTrade.SetMsgId((int)MsgIdQueryReq.ID_QUERY_REQ_POSITION);

            //PBMsgQueryReqPosition.Builder pb = PBMsgQueryReqPosition.CreateBuilder();
            //pb.SetBrokerID(UserInfo.BrokerID);
            //pb.SetInvestorID(UserInfo.UserID);
            //pb.SetInstrumentID("");
            //pb.SetMsgId((int)MsgIdQueryReq.ID_QUERY_REQ_POSITION);
            var pb = SimpleStringTable.CreateBuilder();

            pb.AddColumns(
                NamedStringVector.CreateBuilder().
                SetName(FieldName.BROKER_ID).AddEntry(UserInfo.BrokerID));
            pb.AddColumns(
                NamedStringVector.CreateBuilder().
                SetName(FieldName.USER_ID).AddEntry(UserInfo.UserID));
            pb.AddColumns(
                NamedStringVector.CreateBuilder().
                SetName(FieldName.EXCHANGE_INSTRUMENT_ID).AddEntry(string.Empty));

            //if (producerBeReady)
            //{
            //    sendMsgToBroker(pbTrade.Build().ToByteArray());
            //}
            this.MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_POSITION,
                pb.Build());


            Logger.Debug("RequestPositionInfo");
        }

        public void RequestFundInfo()
        {
            MainWindow.MyInstance.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action<string>(MainWindow.MyInstance.PringToStatus),
                "查询资金进行中...");
            //PBMsgTrader.PBMsgTrader.Builder pbTrade = PBMsgTrader.PBMsgTrader.CreateBuilder();
            //pbTrade.SetMsgId((int)MsgIdQueryReq.ID_QUERY_REQ_FUND);

            //PBMsgQueryReqFund.Builder pb = PBMsgQueryReqFund.CreateBuilder();
            //pb.SetBrokerID(UserInfo.BrokerID);
            //pb.SetInvestorID(UserInfo.UserID);
            //pb.SetMsgId((int)MsgIdQueryReq.ID_QUERY_REQ_FUND);

            var pb = SimpleStringTable.CreateBuilder();

            pb.AddColumns(
                NamedStringVector.CreateBuilder().
                SetName(FieldName.BROKER_ID).AddEntry(UserInfo.BrokerID));
            pb.AddColumns(
                NamedStringVector.CreateBuilder().
                SetName(FieldName.USER_ID).AddEntry(UserInfo.UserID));

            //if (producerBeReady)
            //{
            //sendMsgToBroker(pbTrade.Build().ToByteArray());
            //}
            this.MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_ACCOUNT_INFO,
                pb.Build());


            Logger.Debug("RequestFundInfo");
        }

        public void SyncFromServer()
        {
            lock (this)
            {
                RequestFundInfo();
                RequestPositionInfo();
            }
        }

        public void RequestOrders()
        {
            MainWindow.MyInstance.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action<string>(MainWindow.MyInstance.PringToStatus),
                "查询订单进行中...");
            //PBMsgTrader.PBMsgTrader.Builder pbTrade = PBMsgTrader.PBMsgTrader.CreateBuilder();
            //pbTrade.SetMsgId((int)MsgIdQueryReq.ID_QUERY_REQ_ORDER);

            //PBMsgQueryReqOrder.Builder pb = PBMsgQueryReqOrder.CreateBuilder();
            //pb.SetBrokerID(UserInfo.BrokerID);
            //pb.SetInvestorID(UserInfo.UserID);
            //pb.SetInstrumentID("");
            //pb.SetExchangeID("");
            //pb.SetOrderSysID("");
            //pb.SetInsertTimeStart("");
            //pb.SetInsertTimeEnd("");
            //pb.SetTradingDay("");
            //pb.SetSettlementID(0);
            //pb.SetMsgId((int)MsgIdQueryReq.ID_QUERY_REQ_ORDER);

            var pb = SimpleStringTable.CreateBuilder();

            pb.AddColumns(
                NamedStringVector.CreateBuilder().
                SetName(FieldName.BROKER_ID).AddEntry(UserInfo.BrokerID));
            pb.AddColumns(
                NamedStringVector.CreateBuilder().
                SetName(FieldName.USER_ID).AddEntry(UserInfo.UserID));


            //if (producerBeReady)
            //{
            //sendMsgToBroker(pbTrade.Build().ToByteArray());
            //}
            this.MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_ORDER,
                pb.Build());


            Logger.Debug("RequestOrders");
        }

        public void RequestTrades()
        {
            MainWindow.MyInstance.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action<string>(MainWindow.MyInstance.PringToStatus),
                "查询成交进行中...");
            //PBMsgTrader.PBMsgTrader.Builder pbTrade = PBMsgTrader.PBMsgTrader.CreateBuilder();
            //pbTrade.SetMsgId((int)MsgIdQueryReq.ID_QUERY_REQ_TRADE);

            //PBMsgQueryReqTrade.Builder pb = PBMsgQueryReqTrade.CreateBuilder();
            //pb.SetBrokerID(UserInfo.BrokerID);
            //pb.SetInvestorID(UserInfo.UserID);
            //pb.SetInstrumentID("");
            //pb.SetExchangeID("");
            //pb.SetTradeID("");
            //pb.SetTradeTimeStart("");
            //pb.SetTradeTimeEnd("");

            var pb = SimpleStringTable.CreateBuilder();

            pb.AddColumns(
                NamedStringVector.CreateBuilder().
                SetName(FieldName.BROKER_ID).AddEntry(UserInfo.BrokerID));
            pb.AddColumns(
                NamedStringVector.CreateBuilder().
                SetName(FieldName.USER_ID).AddEntry(UserInfo.UserID));

            //if (producerBeReady)
            //{
            //sendMsgToBroker(pbTrade.Build().ToByteArray());
            //}
            this.MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_TRADE,
                pb.Build());


            Logger.Debug("RequestTrades");
        }

        public void InitAllInfo()
        {

            HasLogin = true;

            //mOrderRefBase = int.Parse(rsp.MaxOrderRef);
            //UserInfo.SessionID = rsp.SessionID;
            //UserInfo.FrontIDNum = rsp.FrontID;

            SettlementInfoConfirm();
            Thread.Sleep(mFrequency);

            RequestInstrumentInfo();
            Thread.Sleep(5 * mFrequency);

            RequestMarketInfo();
            Thread.Sleep(mFrequency);

            RequestFundInfo();
            Thread.Sleep(mFrequency);

            RequestPositionInfo();
            Thread.Sleep(mFrequency);

            RequestOrders();
            Thread.Sleep(mFrequency);

            RequestTrades();
            Thread.Sleep(mFrequency);
        }

        private void OnMarketInfo(PBMsgTrader.PBMsgQueryRspMarketInfo rsp)
        {
            //foreach (PBMsgQueryRspMarketInfo rsp in trade.MsgQueryRspMarketInfoList)
            //{
            //PBMsgQueryRspMarketInfo rsp = trade.MsgQueryRspMarketInfo;
            //Logger.Dump(rsp.AllFields);
            //}
            MainWindow.MyInstance.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action<string>(MainWindow.MyInstance.PringToStatus),
                "查询市场信息结束");

            Logger.Debug("OnMarketInfo");
        }

        private void OnInstrumentInfo(PBMsgTrader.PBMsgQueryRspInstrumentInfo rsp)
        {
            lock (mInstrumentInfo)
            {
                var instrID = rsp.InstrumentID;
                mInstrumentInfo[instrID] = rsp;

                if (rsp.EOF > 0)
                    UpdatePreDefinedGroup();
            }
        }

        private void UpdatePreDefinedGroup()
        {
            var query =
                from n in mInstrumentInfo.Keys
                let insID = from p in InstrumentVMList.Instance select p.InstrumentID
                where !insID.Contains(n)
                select n;
            //var query1 = from row in InstrumentVMList.Instance where rsp.InstrumentID == row.InstrumentID select row;
            var insIDs = query.ToList();
            foreach (var insID in insIDs)
            {
                //ignore spread
                if (!insID.Contains("&"))
                {
                    InstrumentViewModel qi = new InstrumentViewModel();
                    qi.RawData = mInstrumentInfo[insID];

                    qi.Adjust();

                    InstrumentVMList.Instance.Add(qi);

                    if (MainWindow.MyInstance.GroupMap.ContainsKey(qi.ProductClass))
                    {
                        MainWindow.MyInstance.GroupMap[qi.ProductClass].UpdatePreDefinedGroup(qi);
                    }
                }
            }
            MainWindow.MyInstance.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action<string>(MainWindow.MyInstance.PringToStatus),
                "查询合约信息结束");
        }

        private void OnPosition(PBMsgTrader.PBMsgQueryRspPosition rsp)
        {
            if (rsp.PosiDirection == "2")
            {
                mLPosition[rsp.InstrumentID] = rsp.ToBuilder();
            }
            else
            {
                mSPosition[rsp.InstrumentID] = rsp.ToBuilder();
            }

            if (rsp.EOF > 0)
            {
                UpdatePosition();
            }

            MainWindow.MyInstance.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action<string>(MainWindow.MyInstance.PringToStatus),
                "查询持仓结束");
        }

        private void UpdatePosition()
        {
            MainWindow.MyInstance.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action(Positions.Clear));

            var PBList = (from pos in mLPosition select pos.Value)
                .Union(from pos in mSPosition select pos.Value);

            foreach (var pb in PBList)
            {
                var pos = pb.Build();
                PositionViewModel pi = new PositionViewModel(pos, mInstrumentInfo[pos.InstrumentID].ExchangeID);
                MainWindow.MyInstance.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                    new Action<PositionViewModel>(Positions.Add),
                    pi);
            }
        }

        private void OnFund(PBMsgTrader.PBMsgQueryRspFund rsp)
        {
            //PBMsgQueryRspFund rsp = trade.MsgQueryRspFund;
            //Logger.Dump(rsp.AllFields);

            MainWindow.MyInstance.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action<PBMsgQueryRspFund>(Fund.Update),
                rsp);
            MainWindow.MyInstance.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action<string>(MainWindow.MyInstance.PringToStatus),
                "查询资金结束");

            Logger.Debug("OnFund");
        }

        private void OnQueryOrder(PBMsgTrader.PBMsgOrderRtn rsp)
        {
            //foreach (PBMsgOrderRtn rsp in trade.MsgOrderRtnList)
            //{
            //PBMsgOrderRtn rsp = trade.MsgOrderRtn;
            if (rsp.EOF > 0)
            {
                MainWindow.MyInstance.Dispatcher.Invoke(
                 DispatcherPriority.Normal,
                 new Action<PBMsgOrderRtn, string>(ExecutionViewModel.Update),
                 rsp,
                 mInstrumentInfo[rsp.InstrumentID].ExchangeID);
            }

            // }
            MainWindow.MyInstance.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action<string>(MainWindow.MyInstance.PringToStatus),
                "查询订单结束");

            Logger.Debug("OnQueryOrder");
        }

        private void OnUpdateOrder(PBMsgTrader.PBMsgOrderRtn rsp)
        {
            //foreach (PBMsgOrderRtn rsp in trade.MsgOrderRtnList)
            //{
            if (rsp.EOF > 0)
            {
                MainWindow.MyInstance.Dispatcher.Invoke(
                  DispatcherPriority.Normal,
                  new Action<PBMsgOrderRtn, string>(ExecutionViewModel.Update),
                  rsp,
                  mInstrumentInfo[rsp.InstrumentID].ExchangeID);
            }

            //}

            Logger.Debug("OnReturnOrder");
        }

        private void OnQueryTrade(PBMsgTrader.PBMsgTradeRtn rsp)
        {

            //foreach (PBMsgTradeRtn rsp in trade.MsgTradeRtnList)
            //{
            if (rsp.EOF > 0)
            {
                MainWindow.MyInstance.Dispatcher.Invoke(
                  DispatcherPriority.Normal,
                  new Action<PBMsgTradeRtn, string>(ExecutionViewModel.Update),
                  rsp,
                  mInstrumentInfo[rsp.InstrumentID].ExchangeID);
            }

            //}

            MainWindow.MyInstance.Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action<string>(MainWindow.MyInstance.PringToStatus),
                "查询成交结束");

            Logger.Debug("OnQueryTrade");
        }

        private void OnReturnTrade(PBMsgTrader.PBMsgTradeRtn rsp)
        {
            //foreach (PBMsgTradeRtn rsp in trade.MsgTradeRtnList)
            //{
            if (rsp.EOF > 0)
            {
                MainWindow.MyInstance.Dispatcher.Invoke(
                  DispatcherPriority.Normal,
                  new Action<PBMsgTradeRtn, string>(ExecutionViewModel.Update),
                  rsp,
                  mInstrumentInfo[rsp.InstrumentID].ExchangeID);
            }

            //}

            //to update positions
            //RequestPositionInfo();
            SyncFromServer();

            //Thread.Sleep(mFrequency);

            //to get status update
            RequestOrders();

            Logger.Debug("OnReturnTrade");
        }

        private void OnCancel(PBMsgTrader.PBMsgOrderAction rsp)
        {
            //PBMsgOrderAction rsp = trade.MsgOrderAction;
            //Logger.Dump(rsp.AllFields);

            Logger.Debug("OnCancel");
        }

        // gzs add 20140109 start
        public List<PBMsgOrderInsert.Builder> PreCreateOrder(PBMsgOrderInsert.Builder pb)
        {
            // gzs add 20140109 start
            List<PBMsgOrderInsert.Builder> orderlist = new List<PBMsgOrderInsert.Builder>();

            // 如果是平仓
            if (pb.CombOffsetFlag == "1")
            {
                PBMsgQueryRspPosition.Builder position = null;
                if (!(mLPosition.ContainsKey(pb.InstrumentID) && (pb.Direction == "1"))// THOST_FTDC_D_Sell
                    || (!(mSPosition.ContainsKey(pb.InstrumentID) && (pb.Direction == "0")))) // THOST_FTDC_D_Buy
                {
                    pb.CombOffsetFlag = "3";// THOST_FTDC_OF_CloseToday
                    orderlist.Add(pb);
                    return orderlist;
                }
                // sell
                if (pb.Direction == "1")
                {
                    position = mLPosition[pb.InstrumentID];
                }
                // buy
                else
                {
                    position = mSPosition[pb.InstrumentID];
                }

                if (mInstrumentInfo[pb.InstrumentID].ExchangeID == "SHFE" ||
                    mInstrumentInfo[pb.InstrumentID].ExchangeID == "")
                {
                    PBMsgOrderInsert.Builder yPb = pb.Clone();
                    PBMsgOrderInsert.Builder tPb = pb.Clone();
                    // 有昨仓
                    if (position.YdPosition > 0)
                    {
                        yPb.CombOffsetFlag = "1"; // THOST_FTDC_OF_Close
                        yPb.VolumeTotalOriginal = pb.VolumeTotalOriginal > position.YdPosition ? position.YdPosition : pb.VolumeTotalOriginal;
                        tPb.CombOffsetFlag = "3"; // THOST_FTDC_OF_CloseToday;
                        tPb.VolumeTotalOriginal = pb.VolumeTotalOriginal - yPb.VolumeTotalOriginal;
                        orderlist.Add(yPb);
                        if (tPb.VolumeTotalOriginal > 0)
                        {
                            orderlist.Add(tPb);
                        }
                        //修改历史持仓
                        if (pb.Direction == "1")// THOST_FTDC_D_Sell
                        {
                            mLPosition[pb.InstrumentID].YdPosition -= yPb.VolumeTotalOriginal;
                        }
                        else
                        {
                            mSPosition[pb.InstrumentID].YdPosition -= yPb.VolumeTotalOriginal;
                        }
                    }
                    // 无昨仓
                    else
                    {
                        pb.CombOffsetFlag = "3";// THOST_FTDC_OF_CloseToday
                        orderlist.Add(pb);
                    }
                }
                // 非SHFE品种
                else
                {
                    orderlist.Add(pb);
                }
            }
            // 非平仓单
            else
            {
                orderlist.Add(pb);
            }
            return orderlist;
        }
        // gzs add 20140109 end
        public void CreateOrder(PBMsgOrderInsert.Builder pb)
        {
            // gzs mod 20140109 start
            List<PBMsgOrderInsert.Builder> orderlist = PreCreateOrder(pb);
            foreach (PBMsgOrderInsert.Builder newpb in orderlist)
            {
                newpb.SetBrokerID(UserInfo.BrokerID);
                newpb.SetInvestorID(UserInfo.UserID);
                newpb.SetUserID("");
                //newpb.SetOrderRef("" + mOrderRefBase++);
                newpb.SetOrderRef("");
                newpb.SetBusinessUnit("");
                newpb.SetGTDDate("");
                newpb.SetRequestID(0);
                newpb.SetUserForceClose(0);
                newpb.SetIsSwapOrder(0);

                //??
                newpb.SetMinVolume(1);
                newpb.SetForceCloseReason("0");
                newpb.SetIsAutoSuspend(0);

                //任何数量
                newpb.SetVolumeCondition("1");

                //套保
                newpb.SetCombHedgeFlag("1");

                //Logger.Dump(newpb.AllFields);

                //if (producerBeReady)
                //{
                //sendMsgToBroker(pbTrade.Build().ToByteArray());
                //}
                this.MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_ORDER_NEW, pb.Build());


                Logger.Debug("CreateOrder");
            }
        }

        public int GetPosition(string symbol, bool longOrShort)
        {
            if (longOrShort)
            {
                if (mLPosition.ContainsKey(symbol))
                {
                    return mLPosition[symbol].Position;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                if (mSPosition.ContainsKey(symbol))
                {
                    return mSPosition[symbol].Position;
                }
                else
                {
                    return 0;
                }
            }
        }

        public void CancelOrder(ExecutionViewModel vm)
        {
            PBMsgOrderAction.Builder pb = PBMsgOrderAction.CreateBuilder();
            //pb.SetMsgId((int)MsgIdOrder.ID_ORDER_REQ_CAN);
            pb.SetBrokerID(UserInfo.BrokerID);
            pb.SetInvestorID(UserInfo.UserID);
            pb.SetOrderActionRef(mOrderActionRef++);
            pb.SetOrderRef(vm.OrderRef);
            pb.SetRequestID(0);
            pb.SetFrontID(UserInfo.FrontID ?? 0);
            pb.SetSessionID(UserInfo.SessionID ?? 0);
            pb.SetExchangeID(vm.Exchange);
            pb.SetOrderSysID(vm.OrderSysID);
            pb.SetActionFlag("0");
            pb.SetLimitPrice(0.0);
            pb.SetVolumeChange(0);
            pb.SetActionDate("");
            pb.SetActionTime("");
            pb.SetTraderID("");
            pb.SetInstallID(0);
            pb.SetOrderLocalID("");
            pb.SetActionLocalID("");
            pb.SetParticipantID("");
            pb.SetClientID("");
            pb.SetBusinessUnit("");
            pb.SetOrderActionStatus("");
            pb.SetUserID(UserInfo.UserID);
            pb.SetStatusMsg(ByteString.Empty);
            pb.SetInstrumentID(vm.InstrumentID);

            //Logger.Dump(pb.AllFields);

            //if (producerBeReady)
            //{
            //sendMsgToBroker(pbTrade.Build().ToByteArray());
            //}
            this.MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_ORDER_CANCEL,
                pb.Build());


            Logger.Debug("CancelOrder");
        }

        public void CloseMarketOrder(PositionViewModel vm)
        {
            PBMsgOrderInsert.Builder pb = PBMsgOrderInsert.CreateBuilder();
            //pb.SetMsgId((int)MsgIdOrder.ID_ORDER_REQ_NEW);
            pb.SetInstrumentID(vm.RawData.InstrumentID);
            pb.SetVolumeTotalOriginal(vm.RawData.Position);

            if (vm.PosiDirection == "多")
            {
                pb.SetDirection("1");
            }
            else
            {
                pb.SetDirection("0");
            }

            //市价
            pb.SetOrderPriceType("1");
            pb.SetContingentCondition("1");
            pb.SetTimeCondition("1");

            pb.SetStopPrice(0.0);
            pb.SetLimitPrice(0.0);
            pb.SetCombOffsetFlag("1");

            Logger.Debug("CloseMarketOrder");
            CreateOrder(pb);
        }

        public bool OneKeyMakeOrder(AddOrderViewModel vm, ButtonSetting setting, KeyboardOrderViewModel globalvm)
        {
            if (string.IsNullOrEmpty(vm.SymbolID))
            {
                MessageBox.Show("合约缺失", "错误");
                return false;
            }

            if (vm.Size <= 0)
            {
                MessageBox.Show("量有问题", "错误");
                return false;
            }

            if (!globalvm.OneKeyOrderPermitted)
            {
                MessageBox.Show("一键下单被禁用，请于设置中启用", "提示");
                return false;
            }

            var query1 = from row in MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>().QuoteVMCollection where row.Symbol == vm.SymbolID select row;
            QuoteViewModel quote = query1.ElementAt(0);

            if (setting.BidOrAskePrice == 0)
            {
                vm.LimitPrice = quote.BidPrice;
            }
            else
            {
                vm.LimitPrice = quote.AskPrice;
            }

            switch ((ActionChoiceType)setting.ActionChoice)
            {
                case ActionChoiceType.Buy:
                    if (vm.FutureFlag == PBWrapMsgOG.FutureFlag.OPEN)
                    {
                        if (globalvm.OpenOnlyOneOrder)
                        {
                            //挂单
                            var query = from row in ExecutionViewModel
                                        where (row.InstrumentID == vm.SymbolID) && (row.VolumeTotal > 0) &&
                                        row.IsOrderOrTrade && ((row.Status == PBOrderStatus.PARTLY_FINISHED || row.Status == PBOrderStatus.TTIS_ORDER_INSERT_SUCCESS))
                                        select row;
                            if (query.Count() > 0)
                            {
                                //view.Quotes.Add(query.ElementAt(0));
                                return false;
                            }
                        }
                        else if (globalvm.OpenOnlyOneDirectionOrder)
                        {
                            //挂单
                            var query = from row in ExecutionViewModel
                                        where (row.InstrumentID == vm.SymbolID) && (row.VolumeTotal > 0) && (row.Direction == "卖") &&
                                        row.IsOrderOrTrade && ((row.Status == PBOrderStatus.PARTLY_FINISHED || row.Status == PBOrderStatus.TTIS_ORDER_INSERT_SUCCESS))
                                        select row;
                            if (query.Count() > 0)
                            {
                                //反方向有持仓
                                return false;
                            }
                        }

                        vm.DirectMakeOrder(PBWrapMsgOG.OrderDirection.BUY, PBWrapMsgOG.ExecuteType.LIMIT);
                        return true;
                    }
                    else
                    //平今和平仓均视为平仓
                    {
                        int shortPosition = GetPosition(vm.SymbolID, false);
                        switch (globalvm.CloseChoice)
                        {
                            case CloseChoiceType.All:
                                vm.Size = shortPosition;
                                vm.DirectMakeOrder(PBWrapMsgOG.OrderDirection.BUY, PBWrapMsgOG.ExecuteType.LIMIT);
                                break;
                            case CloseChoiceType.IgnorePlus:
                                if (vm.Size > shortPosition)
                                {
                                    //下单数量应为持仓量，超出部分不处理
                                    vm.Size = shortPosition;
                                }

                                vm.DirectMakeOrder(PBWrapMsgOG.OrderDirection.BUY, PBWrapMsgOG.ExecuteType.LIMIT);
                                break;
                            case CloseChoiceType.PlusOpen:
                                if (vm.Size > shortPosition)
                                {
                                    //反向开仓数量
                                    int position = vm.Size - shortPosition;
                                    if (shortPosition > 0)
                                    {
                                        vm.Size = shortPosition;
                                        vm.DirectMakeOrder(PBWrapMsgOG.OrderDirection.BUY, PBWrapMsgOG.ExecuteType.LIMIT);
                                    }

                                    //反向开仓
                                    if (position > 0)
                                    {
                                        vm.FutureFlag = PBWrapMsgOG.FutureFlag.OPEN;
                                        vm.Size = position;
                                        vm.DirectMakeOrder(PBWrapMsgOG.OrderDirection.SELL, PBWrapMsgOG.ExecuteType.LIMIT);
                                    }
                                }
                                else
                                {
                                    vm.DirectMakeOrder(PBWrapMsgOG.OrderDirection.BUY, PBWrapMsgOG.ExecuteType.LIMIT);
                                }
                                break;
                            default:
                                break;
                        }

                        return true;
                    }
                case ActionChoiceType.Sell:
                    if (vm.FutureFlag == PBWrapMsgOG.FutureFlag.OPEN)
                    {
                        if (globalvm.OpenOnlyOneOrder)
                        {
                            //挂单
                            var query = from row in ExecutionViewModel
                                        where (row.InstrumentID == vm.SymbolID) && (row.VolumeTotal > 0) &&
                                        row.IsOrderOrTrade && ((row.Status == PBOrderStatus.PARTLY_FINISHED || row.Status == PBOrderStatus.TTIS_ORDER_INSERT_SUCCESS))
                                        select row;
                            if (query.Count() > 0)
                            {
                                //view.Quotes.Add(query.ElementAt(0));
                                return false;
                            }
                        }
                        else if (globalvm.OpenOnlyOneDirectionOrder)
                        {
                            //挂单
                            var query = from row in ExecutionViewModel
                                        where (row.InstrumentID == vm.SymbolID) && (row.VolumeTotal > 0) && (row.Direction == "买") &&
                                        row.IsOrderOrTrade && ((row.Status == PBOrderStatus.PARTLY_FINISHED || row.Status == PBOrderStatus.TTIS_ORDER_INSERT_SUCCESS))
                                        select row;
                            if (query.Count() > 0)
                            {
                                //反方向有持仓
                                return false;
                            }
                        }

                        vm.DirectMakeOrder(PBWrapMsgOG.OrderDirection.SELL, PBWrapMsgOG.ExecuteType.LIMIT);
                        return true;
                    }
                    else
                    //平今和平仓均视为平仓
                    {
                        int longPosition = GetPosition(vm.SymbolID, true);
                        switch (globalvm.CloseChoice)
                        {
                            case CloseChoiceType.All:
                                vm.Size = longPosition;
                                vm.DirectMakeOrder(PBWrapMsgOG.OrderDirection.SELL, PBWrapMsgOG.ExecuteType.LIMIT);
                                break;
                            case CloseChoiceType.IgnorePlus:
                                if (vm.Size > longPosition)
                                {
                                    //下单数量应为持仓量，超出部分不处理
                                    vm.Size = longPosition;
                                }

                                vm.DirectMakeOrder(PBWrapMsgOG.OrderDirection.SELL, PBWrapMsgOG.ExecuteType.LIMIT);
                                break;
                            case CloseChoiceType.PlusOpen:
                                if (vm.Size > longPosition)
                                {
                                    //反向开仓
                                    int position = vm.Size - longPosition;
                                    if (longPosition > 0)
                                    {
                                        vm.Size = longPosition;
                                        vm.DirectMakeOrder(PBWrapMsgOG.OrderDirection.SELL, PBWrapMsgOG.ExecuteType.LIMIT);
                                    }
                                    //反向开仓
                                    if (position > 0)
                                    {
                                        vm.FutureFlag = PBWrapMsgOG.FutureFlag.OPEN;
                                        vm.Size = position;
                                        vm.DirectMakeOrder(PBWrapMsgOG.OrderDirection.BUY, PBWrapMsgOG.ExecuteType.LIMIT);
                                    }
                                }
                                else
                                {
                                    vm.DirectMakeOrder(PBWrapMsgOG.OrderDirection.SELL, PBWrapMsgOG.ExecuteType.LIMIT);
                                }

                                break;
                            default:
                                break;
                        }
                        return true;
                    }
                case ActionChoiceType.AllCancel:
                    foreach (ExecutionViewModel execution in ExecutionViewModel)
                    {
                        if ((execution.InstrumentID == vm.SymbolID) && (execution.VolumeTotal > 0))
                        {
                            CancelOrder(execution);
                        }
                    }
                    return true;
                default:
                    return false;
            }
        }
    }
}
