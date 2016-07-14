using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Micro.Future.Utility;
using System.Windows.Threading;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using Micro.Future.Message.Business;
using System.Collections.ObjectModel;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;

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
            MessageWrapper.RegisterAction<PBContractInfoList, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_QUERY_INSTRUMENT, OnContractInfo, ErrorMsgAction);
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
            //MessageWrapper.RegisterAction<PBOptionInfo, BizErrorMsg>
            //    ((uint)BusinessMessageID.MSG_ID_OPTION_UPDATE, OnUpdateOption, ErrorMsgAction);
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


        // 保存个人合约信息
        private int OnPersonalContract(PBContractInfoList rsp)//need to be updated to relvant rsp
        {
            int res = -1;
            try
            {
                using (var clientDBCtx = new ClientDbContext())
                {
                    foreach (var personalContract in rsp.ContractInfo)//rsp.ContractInfo need to be updated
                    {
                        clientDBCtx.ContractInfo.Add(new ContractInfo() {Exchange = personalContract.Exchange, Contract = personalContract.Contract });
                    }
                    clientDBCtx.SaveChanges();
                    res = 1;
                }


                //var resPersonalContract = select
               
            }
            catch(Exception ex)
            {
                //log handle
                Console.WriteLine(ex.Message);
            }
            return res;
        }


        //To invoke the function of saving contract data to local sqlite
        private void OnContractInfo(PBContractInfoList rsp)
        {
            int res = 0;
            int rspCount = 0;
            try
            {
                using (var clientDBCtx = new ClientDbContext())
                {
                    foreach (var contract in rsp.ContractInfo)
                    {
                        rspCount = rspCount + 1;
                        clientDBCtx.ContractInfo.Add(new ContractInfo()
                        {
                            //Id = contract.Id,
                            Exchange = contract.Exchange,
                            Contract = contract.Contract,
                            Name = Encoding.UTF8.GetString(contract.Name.ToByteArray()),
                            ProductID = contract.ProductID,
                            ProductType = contract.ProductType,
                            DeliveryYear = contract.DeliveryYear,
                            DeliveryMonth = contract.DeliveryMonth,
                            MaxMarketOrderVolume = contract.MaxMarketOrderVolume,
                            MinMarketOrderVolume = contract.MinMarketOrderVolume,
                            MaxLimitOrderVolume = contract.MaxMarketOrderVolume,
                            MinLimitOrderVolume = contract.MinMarketOrderVolume,
                            VolumeMultiple = contract.VolumeMultiple,
                            PriceTick = contract.PriceTick,
                            CreateDate = contract.CreateDate,
                            OpenDate = contract.OpenDate,
                            ExpireDate = contract.ExpireDate,
                            StartDelivDate = contract.EndDelivDate,
                            EndDelivDate = contract.EndDelivDate,
                            LifePhase = contract.LifePhase,
                            IsTrading = contract.IsTrading,
                            PositionType = contract.PositionType,
                            PositionDateType = contract.PositionDateType,
                            LongMarginRatio = contract.LongMarginRatio,
                            ShortMarginRatio = contract.ShortMarginRatio,
                            MaxMarginSideAlgorithm = contract.MaxMarginSideAlgorithm
                        });
                    }
                    clientDBCtx.SaveChanges();
                    res = 1;
                    //log to be handle 

                    var queryContractorInfo = from ci in clientDBCtx.ContractInfo
                                              select ci;

                    
                    foreach (var query in queryContractorInfo)
                    {
                        foreach (var contract in rsp.ContractInfo)
                        {
                            if ((contract.Exchange == query.Exchange) && (contract.Contract == query.Contract))
                            {
                                //log handle
                                res = res + 1;
                                continue;
                            }

                        }
                    }

                    if (rspCount == res)
                    {
                        Console.WriteLine("本地数据保存成功");
                        //log handle
                    }


                }


               

            }
            catch (Exception ex)
            {
                res = -1;
                //Log to be handle
                Console.WriteLine(ex.InnerException);
            }

            //return res;
        }


        private void OnPosition(PBPosition rsp)
        {

            if (PositionVMCollection != null)
            {
                lock (PositionVMCollection)
                {
                    PositionVM positionVM = null;
                    foreach (var position in PositionVMCollection)
                    {
                        if (position.Contract == rsp.Contract &&
                            (int)position.Direction == rsp.Direction)
                        {
                            positionVM = position;
                            break;
                        }
                    }

                    if (positionVM == null)
                    {
                        positionVM = new PositionVM();
                        PositionVMCollection.Add(positionVM);
                    }

                    positionVM.Direction = (PositionDirectionType)rsp.Direction;
                    positionVM.Position = rsp.Position;
                    positionVM.YdPosition = rsp.YdPosition;
                    positionVM.PositionDate = rsp.PositionDate;
                    positionVM.OpenVolume = rsp.OpenVolume;
                    positionVM.CloseVolume = rsp.CloseVolume;
                    positionVM.OpenAmount = rsp.OpenAmount;
                    positionVM.CloseAmount = rsp.CloseAmount;
                    positionVM.Cost = rsp.Cost;
                    positionVM.OpenCost = rsp.OpenCost;
                    positionVM.Profit = rsp.Profit;
                    positionVM.CloseProfit = rsp.CloseProfit;
                    positionVM.UseMargin = rsp.UseMargin;
                    positionVM.HedgeFlag = (HedgeType)rsp.HedgeFlag;
                    positionVM.Contract = rsp.Contract;
                    positionVM.Exchange = rsp.Exchange;
                }
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

                    foreach (var trade in TradeVMCollection)
                    {
                        if (trade.TradeID == rsp.TradeID)
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
                                    Contract = rsp.Contract,
                                    TradeID = rsp.TradeID,
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
            var sst = new StringMap();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_ACCOUNT_INFO, sst);

        }

        public void QueryPosition()
        {
            var sst = new StringMap();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_POSITION, sst);

        }

        public void QueryOrder()
        {
            var sst = new StringMap();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_ORDER, sst);

        }

        public void QueryTrade()
        {
            var sst = new StringMap();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_TRADE, sst);

        }

        public void QueryContractInfo(string contractID = null)
        {

            var sst = new StringMap();
            if (contractID != null)
                sst.Entry[FieldName.INSTRUMENT_ID] = contractID;
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_INSTRUMENT, sst);



        }


        public void CreateOrder(OrderVM orderVM)
        {
            var pb = new PBOrderInfo();
            pb.Contract = orderVM.Contract;
            pb.LimitPrice = orderVM.LimitPrice;
            pb.Tif = (int)orderVM.TIF;
            pb.Volume = orderVM.Volume;
            pb.ExecType = (int)orderVM.ExecType;
            pb.Direction = (int)orderVM.Direction;
            pb.Openclose = (int)orderVM.OffsetFlag;

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_ORDER_NEW, pb);



        }


        public void CancelOrder(OrderVM orderVM)
        {
            var sendobjBld = new PBOrderInfo();
            sendobjBld.Exchange = orderVM.Exchange;
            sendobjBld.Contract = orderVM.Contract;
            sendobjBld.OrderID = orderVM.OrderID;
            sendobjBld.OrderSysID = orderVM.OrderSysID;
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_ORDER_CANCEL, sendobjBld);
        }

        public void ModifyOrder(OrderVM orderVM)
        {
            CancelOrder(orderVM);
            CreateOrder(orderVM);
        }

    }
}
