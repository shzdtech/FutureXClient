using System;
using System.Linq;
using System.Text;
using Micro.Future.ViewModel;
using Micro.Future.Message.Business;
using System.Collections.ObjectModel;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using System.Threading.Tasks;
using System.Collections.Generic;
using Micro.Future.Utility;
using System.Threading;

namespace Micro.Future.Message
{
    public class BaseTraderHandler : AbstractMessageHandler
    {
        public event Action<Exception> OnOrderError;

        public FundVM FundVM
        {
            get;
        } = new FundVM();

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

        public ISet<string> PositionContractSet { get; } = new HashSet<string>();

        public ObservableCollection<FundVM> FundVMCollection
        {
            get;
        } = new ObservableCollection<FundVM>();

        //to add VMCollection for ContractName
        /*
        public ObservableCollection<ContractVM> ContractVMCollection
        {
            get;
        } = new ObservableCollection<ContractVM>();
        */

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            MessageWrapper.RegisterAction<PBMarketInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_QUERY_EXCHANGE, OnMarketInfo, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBPosition, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_QUERY_POSITION, OnQueryPosition, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBAccountInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_QUERY_ACCOUNT_INFO, OnFund, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBOrderInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_QUERY_ORDER, OnReturnOrder, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBOrderInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_ORDER_NEW, OnReturnOrder, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBOrderInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_ORDER_UPDATE, OnUpdateOrder, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBTradeInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_QUERY_TRADE, OnReturnTrade, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBTradeInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_TRADE_RTN, OnReturnTrade, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBOrderInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_ORDER_CANCEL, OnUpdateOrder, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBPosition, ExceptionMessage>
               ((uint)BusinessMessageID.MSG_ID_POSITION_UPDATED, OnUpdatePosition, ErrorMsgAction);
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

        //To read contract data into contractNameList
        //To invoke the function of saving contract data to local sqlite
        private void OnSyncContractInfo(string key, PBContractInfoList rsp)
        {
            using (var clientCtx = new ClientDbContext())
            {
                var types = rsp.ContractInfo.Select(c => c.ProductType).Distinct().ToList();
                foreach (var productType in types)
                {
                    var oldContracts = from p in clientCtx.ContractInfo
                                       where p.ProductType == productType
                                       select p;

                    if (oldContracts.Any())
                    {
                        clientCtx.RemoveRange(oldContracts);
                        clientCtx.SaveChanges();
                    }

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

                    ClientDbContext.GetContractFromCache(productType);
                }

                clientCtx.SetSyncVersion(key, DateTime.Now.Date.ToShortDateString());
                clientCtx.SaveChanges();
            }
        }

        private void OnQueryPosition(PBPosition rsp)
        {
            UpdatePosition(rsp);
        }

        private void OnUpdatePosition(PBPosition rsp)
        {
            UpdatePosition(rsp);
        }

        private void UpdatePosition(PBPosition rsp)
        {
            lock (PositionVMCollection)
            {
                PositionVM positionVM = PositionVMCollection.FirstOrDefault(p =>
                    p.Contract == rsp.Contract && (int)p.Direction == rsp.Direction && p.Portfolio == rsp.Portfolio);

                if (rsp.Position == 0)
                {
                    if (positionVM != null)
                        PositionVMCollection.Remove(positionVM);

                    if (!PositionVMCollection.Any(p => p.Contract == rsp.Contract))
                    {
                        PositionContractSet.Remove(rsp.Contract);
                    }
                }
                else
                {
                    if (positionVM == null)
                    {
                        var contractInfo = ClientDbContext.FindContract(rsp.Contract);
                        positionVM = new PositionVM
                        {
                            Contract = rsp.Contract,
                            Exchange = rsp.Exchange,
                            Portfolio = rsp.Portfolio,
                            Direction = (PositionDirectionType)rsp.Direction,
                            HedgeFlag = (HedgeType)rsp.HedgeFlag,
                            PositionDateFlag = (PositionDateFlagType)rsp.PositionDateFlag,
                            Multiplier = contractInfo == null ? 1 : contractInfo.VolumeMultiple
                        };
                        PositionVMCollection.Add(positionVM);
                        PositionContractSet.Add(rsp.Contract);
                    }

                    positionVM.Position = rsp.Position;
                    positionVM.TodayPosition = rsp.TdPosition;
                    positionVM.YdPosition = rsp.YdPosition;
                    positionVM.OpenVolume = rsp.OpenVolume;
                    positionVM.CloseVolume = rsp.CloseVolume;
                    positionVM.OpenAmount = rsp.OpenAmount;
                    positionVM.CloseAmount = rsp.CloseAmount;
                    positionVM.Cost = rsp.Cost;
                    positionVM.OpenCost = rsp.OpenCost;
                    positionVM.Profit = rsp.Profit;
                    positionVM.CloseProfit = rsp.CloseProfit;
                    positionVM.UseMargin = rsp.UseMargin;
                    positionVM.MeanCost = rsp.Cost / rsp.Position / positionVM.Multiplier;
                }
                OnPositionUpdated?.Invoke(positionVM);
            }
        }
        public event Action<PositionVM> OnPositionUpdated;

        private void OnFund(PBAccountInfo rsp)
        {
            FundVM.Commission = rsp.Commission;
            FundVM.BrokerID = rsp.BrokerID;
            FundVM.AccountID = rsp.AccountID;
            FundVM.SettlementID = rsp.SettlementID;
            FundVM.PreMortgage = rsp.PreMortgage;
            FundVM.PreCredit = rsp.PreCredit;
            FundVM.PreDeposit = rsp.PreDeposit;
            FundVM.PreBalance = rsp.PreBalance;
            FundVM.PreMargin = rsp.PreMargin;
            FundVM.InterestBase = rsp.InterestBase;
            FundVM.Deposit = rsp.Deposit;
            FundVM.Withdraw = rsp.Withdraw;
            FundVM.FrozenMargin = rsp.FrozenMargin;
            FundVM.FrozenCash = rsp.FrozenCash;
            FundVM.FrozenCommission = rsp.FrozenCommission;
            FundVM.CurrMargin = rsp.CurrMargin;
            FundVM.CashIn = rsp.CashIn;
            FundVM.CloseProfit = rsp.CloseProfit;
            FundVM.PositionProfit = rsp.PositionProfit;
            FundVM.Balance = rsp.Balance;
            FundVM.Available = rsp.Available;
            FundVM.WithdrawQuota = rsp.WithdrawQuota;
            FundVM.Reserve = rsp.Reserve;
            FundVM.TradingDay = rsp.TradingDay;
            FundVM.Credit = rsp.Credit;
            FundVM.Mortgage = rsp.Mortgage;
            FundVM.ExchangeMargin = rsp.ExchangeMargin;
            FundVM.DeliveryMargin = rsp.DeliveryMargin;
            FundVM.ExchangeDeliveryMargin = rsp.ExchangeDeliveryMargin;
            FundVM.ReserveBalance = rsp.ReserveBalance;
            FundVM.StaticBalance = rsp.PreBalance - rsp.PreCredit - rsp.PreCredit + rsp.Mortgage - rsp.Withdraw + rsp.Deposit;
        }
        private void OnReturnOrder(PBOrderInfo rsp)
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
                        VOLTYPE = (OrderVolType)rsp.VolumeCondition,
                        TradingType = (TradingType)rsp.TradingType,
                        Active = rsp.Active,
                        Status = (OrderStatus)rsp.OrderStatus,
                        OpenClose = (OrderOpenCloseType)rsp.Openclose,
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
        private void OnUpdateOrder(PBOrderInfo rsp)
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
                else
                {
                    OnReturnOrder(rsp);
                }
            }
        }
        private void OnReturnTrade(PBTradeInfo rsp)
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
                            OpenClose = (OrderOpenCloseType)rsp.Openclose,
                            Commission = rsp.Commission,
                        });
                }
            }
        }

        public void QueryAccountInfo()
        {
            var sst = new StringMap();
            sst.Header = new DataHeader();
            sst.Header.SerialId = NextSerialId;
            MessageWrapper?.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_ACCOUNT_INFO, sst);

        }

        public void QueryPosition()
        {
            var sst = new StringMap();
            sst.Header = new DataHeader();
            sst.Header.SerialId = NextSerialId;
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_POSITION, sst);

        }

        public void QueryOrder()
        {
            var sst = new StringMap();
            sst.Header = new DataHeader();
            sst.Header.SerialId = NextSerialId;
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_ORDER, sst);

        }

        public void QueryTrade()
        {
            var sst = new StringMap();
            sst.Header = new DataHeader();
            sst.Header.SerialId = NextSerialId;
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_TRADE, sst);

        }

        public Task<bool> SyncContractInfoAsync(bool forced = false)
        {
            var today = DateTime.Now.Date.ToShortDateString();
            var key = string.Format("{0}:{1}", nameof(ContractInfo), GetType().Name);

            if (!forced && MFUtilities.GetSyncVersion(key) == today)
            {
                return Task.FromResult(true);
            }

            var sst = new StringMap();
            sst.Header = new DataHeader();
            sst.Header.SerialId = NextSerialId;
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
                        OnSyncContractInfo(key, resp);

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


            double price = 0;

            var contractInfo = ClientDbContext.FindContract(orderVM.Contract);
            if (contractInfo != null)
            {
                price = Math.Round(orderVM.LimitPrice / contractInfo.PriceTick) * contractInfo.PriceTick;
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
            pb.VolCond = (int)orderVM.VOLTYPE;
            pb.Volume = orderVM.Volume;
            pb.ExecType = (int)orderVM.ExecType;
            pb.Direction = (int)orderVM.Direction;
            pb.Openclose = (int)orderVM.OpenClose;
            if (orderVM.Portfolio != null)
                pb.Portfolio = orderVM.Portfolio;

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_ORDER_NEW, pb);
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
        public Task<ObservableCollection<RiskVM>> QueryRiskAsync(string portfolio, int timeout = 10000)
        {
            var sst = new StringMap();
            var msgId = (uint)BusinessMessageID.MSG_ID_QUERY_RISK;
            var tcs = new TaskCompletionSource<ObservableCollection<RiskVM>>(new CancellationTokenSource(timeout));

            var serialId = NextSerialId;
            sst.Header = new DataHeader { SerialId = serialId };
            sst.Entry.Add(string.Empty, portfolio);

            MessageWrapper.RegisterAction<PBRiskList, ExceptionMessage>
                (msgId,
                (resp) =>
                {
                    if (resp.Header?.SerialId == serialId)
                    {
                        tcs.TrySetResult(OnQueryRiskSuccessAction(resp));
                    }
                },
                (bizErr) =>
                {
                    OnErrorAction(bizErr);
                    tcs.SetResult(null);
                }
                );

            MessageWrapper.SendMessage(msgId, sst);

            return tcs.Task;
        }

        private ObservableCollection<RiskVM> OnQueryRiskSuccessAction(PBRiskList rsp)
        {
            var riskList = new ObservableCollection<RiskVM>();
            foreach (var risk in rsp.Risk)
            {
                //var contractinfo = ClientDbContext.FindContract(risk.Contract);
                //if (contractinfo.UnderlyingContract != null)
                riskList.Add(new RiskVM
                {
                    Exchange = risk.Exchange,
                    Contract = risk.Contract,
                    Underlying = risk.Underlying,
                    Delta = risk.Delta,
                    Gamma = risk.Gamma,
                    Theta = risk.Theta,
                    Vega = risk.Vega,
                    Position = risk.Position,
                    //ContractKey = contractinfo.UnderlyingContract
                });
            }

            return riskList;
        }
        protected void OnErrorAction(ExceptionMessage bizErr)
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

