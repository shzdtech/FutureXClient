using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Micro.Future.Util;
using System.Windows.Threading;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using Micro.Future.Message.Business;
using System.Collections.ObjectModel;

namespace Micro.Future.Message
{
    public class TraderExHandler : MessageHandlerTemplate<TraderExHandler>
    {
        public ObservableCollection<TradeVM> TradeVMCollection
        {
            get;
        } = new ObservableCollection<TradeVM>();

        public ObservableCollection<OrderVM> OrderVMCollection
        {
            get;
        } = new ObservableCollection<OrderVM>();
        public ObservableCollection<InstrumentViewModel> InstrumentVMCollection
        {
            get;
        } = new ObservableCollection<InstrumentViewModel>();

        public ObservableCollection<PositionVM> PositionVMCollection
        {
            get;
        } = new ObservableCollection<PositionVM>();

        public ExecutionCollection ExecutionVMCollection
        {
            get;
        } = new ExecutionCollection();

        public ObservableCollection<FundVM> FundVMCollection
        {
            get;
        } = new ObservableCollection<FundVM>();

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            MessageWrapper.RegisterAction<PBMsgTrader.PBMsgQueryRspMarketInfo, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_QUERY_EXCHANGE, OnMarketInfo, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBMsgTrader.PBMsgQueryRspInstrumentInfo, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_QUERY_INSTRUMENT, OnInstrumentInfo, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBPosition, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_QUERY_POSITION, OnPosition, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBAccountInfo, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_QUERY_ACCOUNT_INFO, OnFund, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBOrderInfo, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_QUERY_ORDER, OnQueryOrder, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBOrderInfo, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_ORDER_NEW, OnQueryOrder, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBOrderInfo, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_ORDER_UPDATE, OnUpdateOrder, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBTradeInfo, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_QUERY_TRADE, OnQueryTrade, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBTradeInfo, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_TRADE_RTN, OnReturnTrade, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBOrderInfo, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_ORDER_CANCEL, OnCancel, ErrorMsgAction);
        }

        private void OnCancel(PBOrderInfo rsp)
        {
            OnUpdateOrder(rsp);
        }



        private void ErrorMsgAction(BizErrorMsg bizErr)
        {
            RaiseOnError(
                new MessageException(bizErr.MessageId, bizErr.Errorcode,
                Encoding.UTF8.GetString(bizErr.Description.ToByteArray()),
                bizErr.Syserrcode));
        }

        private void OnMarketInfo(PBMsgTrader.PBMsgQueryRspMarketInfo rsp)
        {

        }

        private void OnInstrumentInfo(PBMsgTrader.PBMsgQueryRspInstrumentInfo rsp)
        {
            if (InstrumentVMCollection != null)
            {
                InstrumentVMCollection.Add(new InstrumentViewModel()
                {
                    RawData = rsp
                });
            }
        }
        private void OnPosition(PBPosition rsp)
        {

            if (PositionVMCollection != null)
            {
                PositionVMCollection.Dispatcher.Invoke(
                () =>
                {
                    PositionVMCollection.Add(new PositionVM()
                    {
                        Direction = (PositionDirectionType)rsp.Direction,
                        Position = rsp.Position,
                        YdPosition = rsp.YdPosition,
                        PositionDate = rsp.PositionDate,
                        OpenVolume = rsp.OpenVolume,
                        CloseVolume = rsp.CloseVolume,
                        OpenAmount = rsp.OpenAmount,
                        CloseAmount = rsp.CloseAmount,
                        Cost = rsp.Cost,
                        OpenCost = rsp.OpenCost,
                        Profit = rsp.Profit,
                        CloseProfit = rsp.CloseProfit,
                        UseMargin = rsp.UseMargin,
                        HedgeFlag = (HedgeType)rsp.HedgeFlag,
                        Contract = rsp.Contract,
                        Exchange = rsp.Exchange,
                        //TodayPosition=rsp.                     
                        //CancelTime=rsp.

                });
            }

        }
        private void OnFund(PBAccountInfo rsp)
        {
            if (FundVMCollection != null)
            {
                FundVMCollection.Add(new FundVM()
                {
                    BrokerID = rsp.BrokerID,
                    AccountID = rsp.AccountID,
                    PreMortgage = rsp.PreMortgage,
                    PreCredit = rsp.PreCredit,
                    PreDeposit = rsp.PreDeposit,
                    PreBalance = rsp.PreBalance,
                    PreMargin = rsp.PreMargin,
                    InterestBase = rsp.InterestBase,
                    Deposit = rsp.Deposit,
                    Withdraw = rsp.Withdraw,
                    FrozenMargin = rsp.FrozenMargin,
                    FrozenCash = rsp.FrozenCash,
                    FrozenCommission = rsp.FrozenCommission,
                    CurrMargin = rsp.CurrMargin,
                    CashIn = rsp.CashIn,
                    CloseProfit = rsp.CloseProfit,
                    PositionProfit = rsp.PositionProfit,
                    Balance = rsp.Balance,
                    Available = rsp.Available,
                    WithdrawQuota = rsp.WithdrawQuota,
                    Reserve = rsp.Reserve,
                    TradingDay = rsp.TradingDay,
                    SettlementID = rsp.SettlementID,
                    Credit = rsp.Credit,
                    Mortgage = rsp.Mortgage,
                    ExchangeMargin = rsp.ExchangeMargin,
                    DeliveryMargin = rsp.DeliveryMargin,
                    ExchangeDeliveryMargin = rsp.ExchangeDeliveryMargin,
                    ReserveBalance = rsp.ReserveBalance,
                });
            }
        }
        private void OnQueryOrder(PBOrderInfo rsp)
        {
            if (OrderVMCollection != null)
            {
                lock (OrderVMCollection)
                {
                    bool found = false;

                    foreach (var order in OrderVMCollection)
                    {
                        if (order.OrderID == rsp.OrderID)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        var orderVM = new OrderVM()
                        {
                            OrderID = rsp.OrderID,
                            OrderSysID = rsp.OrderSysID,
                            Direction = (DirectionType)rsp.Direction,
                            LimitPrice = rsp.LimitPrice,
                            Volume = rsp.Volume,
                            VolumeTraded = rsp.VolumeTraded,
                            VolumeRemain = rsp.VolumeRemain,
                            ExecType = (OrderExecType)rsp.ExecType,
                            TIF = (OrderTIFType)rsp.Tif,
                            TradingType = (TradingType)rsp.TradingType,
                            Active = rsp.Active,
                            Status = (OrderStatus)rsp.OrderStatus,
                            OffsetFlag = (OrderOffsetType)rsp.Openclose,
                            InsertTime = rsp.InsertTime,
                            UpdateTime = rsp.UpdateTime,
                            CancelTime = rsp.CancelTime,
                            Exchange = rsp.Exchange,
                            Contract = rsp.Contract,
                            Message = Encoding.UTF8.GetString(rsp.Message.ToByteArray()),
                        };

                        OrderVMCollection.Add(orderVM);
                    }
                }
            }
        }
        //                ExecutionVMCollection.Dispatcher.Invoke
        //                    (new Action<PBMsgTrader.PBMsgOrderRtn, string>(ExecutionVMCollection.Update),
        //                      rsp, rsp.ExchangeID); 


        private void OnUpdateOrder(PBOrderInfo rsp)
        {
            if (OrderVMCollection != null)
            {
                lock (OrderVMCollection)
                {
                    foreach (var order in OrderVMCollection)
                    {
                        if (order.OrderID == rsp.OrderID)
                        {
                            order.Status = (OrderStatus)rsp.OrderStatus;
                            order.OrderSysID = rsp.OrderSysID;
                            order.UpdateTime = order.UpdateTime;
                            order.Message = Encoding.UTF8.GetString(rsp.Message.ToByteArray());
                        }
                    }
                }
            }
        }
        private void OnQueryTrade(PBTradeInfo rsp)
        {
            if (TradeVMCollection != null)
            {
                lock (TradeVMCollection)
                {
                    bool found = false;

                    foreach (var trade in OrderVMCollection)
                    {
                        if (trade.OrderID == rsp.OrderID)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        TradeVMCollection.Add(
                                    new TradeVM()
                                    {
                                        OrderID = rsp.OrderID,
                                        Exchange = rsp.Exchange,
                                        OrderSysID = rsp.OrderSysID,
                                        Direction = (DirectionType)rsp.Direction,
                                        Price = rsp.Price,
                                        Volume = rsp.Volume,
                                        TradingType = (TradingType)rsp.TradeType,
                                        TradeID = rsp.TradeID,
                                        Contract = rsp.Contract,
                                        TradeDate = rsp.TradeDate,
                                        OpenClose = (OrderOffsetType)rsp.Openclose,
                                        Commission = rsp.Commission,
                                        //InsertTime = rsp.,
                                        //UpdateTime = rsp.,
                                    });
                    }
                }
            }
        }
        private void OnReturnTrade(PBTradeInfo rsp)
        {
            if (TradeVMCollection != null)
            {
                TradeVMCollection.Add(
                                new TradeVM()
                                {
                                    OrderID = rsp.OrderID,
                                    Exchange = rsp.Exchange,
                                    OrderSysID = rsp.OrderSysID,
                                    Direction = (DirectionType)rsp.Direction,
                                    Price = rsp.Price,
                                    Volume = rsp.Volume,
                                    TradingType = (TradingType)rsp.TradeType,
                                    TradeDate = rsp.TradeDate,
                                    TradeTime = rsp.TradeTime,
                                    OpenClose = (OrderOffsetType)rsp.Openclose
                                });
            }
        }

        public void QueryAccountInfo()
        {
            var sst = SimpleStringTable.CreateBuilder();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_ACCOUNT_INFO, sst.Build());

        }

        public void QueryPosition()
        {
            var sst = SimpleStringTable.CreateBuilder();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_POSITION, sst.Build());

        }

        public void QueryOrder()
        {
            var sst = SimpleStringTable.CreateBuilder();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_ORDER, sst.Build());

        }

        public void QueryTrade()
        {
            var sst = SimpleStringTable.CreateBuilder();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_TRADE, sst.Build());

        }
        public void CreateOrder(OrderVM orderVM)
        {
            var pb = PBOrderInfo.CreateBuilder();
            pb.Contract = orderVM.Contract;
            pb.LimitPrice = orderVM.LimitPrice;
            pb.Tif = (int)orderVM.TIF;
            pb.Volume = orderVM.Volume;
            pb.ExecType = (int)orderVM.ExecType;
            pb.Direction = (int)orderVM.Direction;
            pb.Openclose = (int)orderVM.OffsetFlag;

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_ORDER_NEW, pb.Build());

        }


        public void CancelOrder(OrderVM orderVM)
        {
            var sendobjBld = PBOrderInfo.CreateBuilder();
            sendobjBld.Exchange = orderVM.Exchange;
            sendobjBld.Contract = orderVM.Contract;
            sendobjBld.OrderID = orderVM.OrderID;
            sendobjBld.OrderSysID = orderVM.OrderSysID;
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_ORDER_CANCEL, sendobjBld.Build());
        }

        public void ModifyOrder(OrderVM orderVM)
        {
            CancelOrder(orderVM);
            CreateOrder(orderVM);
        }

    }
}
