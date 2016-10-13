using System;
using System.Linq;
using System.Text;
using Micro.Future.ViewModel;
using Micro.Future.Message.Business;
using System.Collections.ObjectModel;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public class TraderExHandler : MessageHandlerTemplate<TraderExHandler>
    {
        public event Action<Exception> OnOrderError;

        public ObservableCollection<TradeVM> TradeVMCollection
        {
            get;
        } = new ObservableCollection<TradeVM>();

        public ObservableCollection<OrderVM> OrderVMCollection
        {
            get;
        } = new ObservableCollection<OrderVM>();

        public ObservableCollection<PositionVM> PositionVMCollection
        {
            get;
        } = new ObservableCollection<PositionVM>();

        public ObservableCollection<FundVM> FundVMCollection
        {
            get;
        } = new ObservableCollection<FundVM>();

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            MessageWrapper.RegisterAction<PBMarketInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_QUERY_EXCHANGE, OnMarketInfo, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBPosition, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_QUERY_POSITION, OnPosition, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBAccountInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_QUERY_ACCOUNT_INFO, OnFund, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBOrderInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_QUERY_ORDER, OnQueryOrder, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBOrderInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_ORDER_NEW, OnQueryOrder, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBOrderInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_ORDER_UPDATE, OnUpdateOrder, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBTradeInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_QUERY_TRADE, OnQueryTrade, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBTradeInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_TRADE_RTN, OnReturnTrade, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBOrderInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_ORDER_CANCEL, OnCancel, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBPosition, ExceptionMessage>
               ((uint)BusinessMessageID.MSG_ID_POSITION_UPDATED, OnPosition, ErrorMsgAction);
        }


        private void OnCancel(PBOrderInfo rsp)
        {
            OnUpdateOrder(rsp);
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

        private void OnMarketInfo(PBMarketInfo rsp)
        {

        }
        //public String[] 

        //To invoke the function of saving contract data to local sqlite
        private void OnSyncContractInfo(PBContractInfoList rsp)
        {
            using (var clientCtx = new ClientDbContext())
            {
                var types = rsp.ContractInfo.Select(c => c.ProductType).Distinct().ToList();
                foreach (var productType in types)
                {
                    var oldContracts = from p in clientCtx.ContractInfo
                                       where p.ProductType == productType
                                       select p;

                    clientCtx.RemoveRange(oldContracts);
                    clientCtx.SaveChanges();

                    var contractList = from c in rsp.ContractInfo
                                       where c.ProductType == productType
                                       select c;

                    foreach (var contract in contractList)
                    {
                        clientCtx.ContractInfo.Add(new ContractInfo()
                        {
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
                            UnderlyingExchange = contract.UnderlyingExchange,
                            UnderlyingContract = contract.UnderlyingContract,
                            StrikePrice = contract.StrikePrice,
                            ContractType = contract.ContractType
                        });
                    }

                    if (contractList.Any())
                        clientCtx.SaveChanges();
                }

                clientCtx.SetSyncVersion(nameof(ContractInfo), DateTime.Now.Date.ToShortDateString());
                clientCtx.SaveChanges();
            }
        }

        private void OnPosition(PBPosition rsp)
        {

            if (PositionVMCollection != null)
            {
                lock (PositionVMCollection)
                {
                    PositionVM positionVM = PositionVMCollection.FirstOrDefault(position =>
                        position.Contract == rsp.Contract &&
                            (int)position.Direction == rsp.Direction &&
                            (int)position.PositionDateFlag == rsp.PositionDateFlag);

                    if (positionVM == null)
                    {
                        positionVM = new PositionVM();
                        PositionVMCollection.Add(positionVM);
                    }

                    positionVM.Direction = (PositionDirectionType)rsp.Direction;
                    positionVM.Position = rsp.Position;
                    positionVM.YdPosition = rsp.YdPosition;
                    positionVM.PositionDateFlag = (PositionDateFlagType)rsp.PositionDateFlag;
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
                    if (!OrderVMCollection.Any(order =>
                             (rsp.OrderSysID != 0 && rsp.OrderSysID == order.OrderSysID) ||
                             (rsp.OrderID == order.OrderID && rsp.SessionID == order.SessionID)))
                    {
                        var orderVM = new OrderVM(this)
                        {
                            OrderID = rsp.OrderID,
                            OrderSysID = rsp.OrderSysID,
                            Portfolio = rsp.Portfolio,
                            SessionID = rsp.SessionID,
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


        private void OnUpdateOrder(PBOrderInfo rsp)
        {
            if (OrderVMCollection != null)
            {
                lock (OrderVMCollection)
                {
                    var orderVM = OrderVMCollection.FirstOrDefault(order =>
                            (rsp.OrderSysID != 0 && rsp.OrderSysID == order.OrderSysID) ||
                            (rsp.OrderID == order.OrderID && rsp.SessionID == order.SessionID));
                    if (orderVM != null)
                    {
                        orderVM.Active = rsp.Active;
                        orderVM.Status = (OrderStatus)rsp.OrderStatus;
                        orderVM.OrderSysID = rsp.OrderSysID;
                        orderVM.UpdateTime = rsp.UpdateTime;
                        orderVM.Message = Encoding.UTF8.GetString(rsp.Message.ToByteArray());
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
                    if (!TradeVMCollection.Any(trade => trade.TradeID == rsp.TradeID))
                    {
                        TradeVMCollection.Add(
                                new TradeVM()
                                {
                                    OrderID = rsp.OrderID,
                                    Exchange = rsp.Exchange,
                                    OrderSysID = rsp.OrderSysID,
                                    Portfolio = rsp.Portfolio,
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
                                    Portfolio = rsp.Portfolio,
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

        public Task<bool> SyncContractInfoAsync()
        {

            var sst = new StringMap();
            var msgId = (uint)BusinessMessageID.MSG_ID_QUERY_INSTRUMENT;

            var tcs = new TaskCompletionSource<bool>();

            var serialId = NextSerialId;
            sst.Header = new DataHeader { SerialId = serialId };

            MessageWrapper.RegisterAction<PBContractInfoList, ExceptionMessage>
                (msgId,
                (resp) =>
                {
                    if (resp.Header?.SerialId == serialId)
                    {
                        OnSyncContractInfo(resp);

                        tcs.TrySetResult(true);
                    }
                },
                (bizErr) =>
                {
                    tcs.SetResult(false);
                }
                );

            MessageWrapper.SendMessage(msgId, sst);

            return tcs.Task;
        }


        public void CreateOrder(OrderVM orderVM)
        {
            if (orderVM == null)
            {

                OnOrderError?.Invoke(new Exception("订单对象异常"));
                return;
            }

            if (orderVM.Contract == null)
            {
                OnOrderError?.Invoke(new Exception("订单合约不能为空"));
                return;
            }

            if (orderVM.Volume <= 0)
            {
                OnOrderError?.Invoke(new Exception("订单数量不正确"));
                return;
            }

            double tickPrice = 0;
            double price = 0;

            using (var clientCtx = new ClientDbContext())
            {
                tickPrice = (from contract in clientCtx.ContractInfo
                             where contract.Contract == orderVM.Contract
                             select contract.PriceTick).FirstOrDefault();

                if (tickPrice != 0)
                {
                    price = Math.Round((orderVM.LimitPrice / tickPrice)) * tickPrice;
                }
                else
                {
                    OnOrderError?.Invoke(new Exception("输入合约不存在"));
                    return;
                }

                var pb = new PBOrderRequest();
                pb.Contract = orderVM.Contract;
                pb.LimitPrice = orderVM.LimitPrice;
                pb.Tif = (int)orderVM.TIF;
                pb.Volume = orderVM.Volume;
                pb.ExecType = (int)orderVM.ExecType;
                pb.Direction = (int)orderVM.Direction;
                pb.Openclose = (int)orderVM.OffsetFlag;

                MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_ORDER_NEW, pb);
            }
        }


        public void CancelOrder(OrderVM orderVM)
        {
            var sendobjBld = new PBOrderRequest();
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

