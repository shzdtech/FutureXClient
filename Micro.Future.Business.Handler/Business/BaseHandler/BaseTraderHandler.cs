﻿using System;
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
using Micro.Future.Message;
using System.Windows;

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
        public ObservableCollection<PositionVM> PositionProfitVMCollection
        {
            get;
        } = new ObservableCollection<PositionVM>();
        public ObservableCollection<PositionDifferVM> PositionDifferVMCollection
        {
            get;
        } = new ObservableCollection<PositionDifferVM>();
        public ObservableCollection<TradeDifferVM> TradeDifferVMCollection
        {
            get;
        } = new ObservableCollection<TradeDifferVM>();
        public ObservableCollection<PositionDifferVM> PositionSyncVMCollection
        {
            get;
        } = new ObservableCollection<PositionDifferVM>();
        public List<string> PositionSyncList { get; } = new List<string>();

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
            MessageWrapper.RegisterAction<PBPositionCompareList, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_QUERY_POSITION_DIFFER, OnQueryPositionDiffer, ErrorMsgAction);
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
                ((uint)BusinessMessageID.MSG_ID_QUERY_TRADE_DIFFER, OnQueryTradeDiffer, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBTradeInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_TRADE_RTN, OnReturnTrade, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBOrderInfo, ExceptionMessage>
                ((uint)BusinessMessageID.MSG_ID_ORDER_CANCEL, OnUpdateOrder, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBPosition, ExceptionMessage>
               ((uint)BusinessMessageID.MSG_ID_POSITION_UPDATED, OnUpdatePosition, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBPositionPnL, ExceptionMessage>
               ((uint)BusinessMessageID.MSG_ID_POSITIONPNL_UPDATED, OnUpdatePositionProfit, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBPositionPnL, ExceptionMessage>
               ((uint)BusinessMessageID.MSG_ID_QUERY_POSITIONPNL, OnQueryPositionProfit, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBPosition, ExceptionMessage>
               ((uint)BusinessMessageID.MSG_ID_EXCHANGE_POSITION_UPDATED, OnExchangeUpdatePosition, ErrorMsgAction);

        }

        private void OnExchangeUpdatePosition(PBPosition rsp)
        {
            lock (PositionVMCollection)
            {
                var positionCollection = PositionVMCollection.Where(p =>
                    p.Contract == rsp.Contract && (int)p.Direction == rsp.Direction);

                foreach (var positionVM in positionCollection)
                {
                    if (positionVM != null)
                    {
                        if (rsp.PositionDateFlag == (int)PositionDateFlagType.PSD_TODAY)
                        {
                            if (rsp.TdPosition > 0)
                            {
                                positionVM.TdCost = rsp.TdCost / rsp.TdPosition * positionVM.TdCost;
                            }
                        }
                        else
                        {
                            if (rsp.YdPosition > 0)
                            {
                                positionVM.YdCost = rsp.YdCost / rsp.YdPosition * positionVM.YdCost;
                            }
                        }
                    }
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

        private void OnMarketInfo(PBMarketInfo rsp)
        {

        }
        private void OnQueryPositionDiffer(PBPositionCompareList pb)
        {
            PositionDifferVMCollection.Clear();
            foreach (var positionDiffer in pb.Positions)
            {
                if (positionDiffer.DbPosition != 0 || positionDiffer.SysPosition != 0)
                {
                    PositionDifferVMCollection.
                        Add(new PositionDifferVM
                        {
                            Contract = positionDiffer.Contract,
                            Position = positionDiffer.DbPosition,
                            Direction = (PositionDirectionType)positionDiffer.Direction,
                            SysPosition = positionDiffer.SysPosition,
                            Portfolio = positionDiffer.Portfolio,
                            Selected = string.IsNullOrEmpty(positionDiffer.Portfolio)
                        });
                }
            }
        }
        private void OnQueryTradeDiffer(PBTradeInfo rsp)
        {
            var trade = new TradeDifferVM()
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
            };

            //OnTraded?.Invoke(trade);

            lock (TradeDifferVMCollection)
            {
                if (!TradeDifferVMCollection.Any(t => t.TradeID == rsp.TradeID))
                {
                    TradeDifferVMCollection.Add(trade);
                }
            }
        }
        public virtual void SyncPosition(IEnumerable<PositionDifferVM> positiondiffervmList)
        {

            var sst = new PBPositionCompareList();
            foreach (var positiondiffervm in positiondiffervmList)
            {
                sst.Positions.Add(new PBPositionCompare { Contract = positiondiffervm.Contract, Direction = (int)positiondiffervm.Direction, Portfolio = positiondiffervm.Portfolio, DbPosition = positiondiffervm.Position, SysPosition = positiondiffervm.SysPosition });
            }
            sst.Header = new DataHeader();
            sst.Header.SerialId = NextSerialId;
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_SYNC_POSITION, sst);
        }
        public virtual Task<bool> SyncTradeAsync(TradeDifferVM tradediffervm)
        {
            var sst = new PBTradeInfo();
            var msgId = (uint)BusinessMessageID.MSG_ID_SYNC_TRADE;
            var tcs = new TaskCompletionSource<bool>();
            var serialId = NextSerialId;
            sst.Header = new DataHeader { SerialId = serialId };
            sst.TradeID = tradediffervm.TradeID;
            sst.Portfolio = tradediffervm.Portfolio;
            sst.OrderSysID = tradediffervm.OrderSysID;
            MessageWrapper.RegisterAction<PBTradeInfo, ExceptionMessage>
    (msgId,
    (resp) =>
    {
        if (resp.Header?.SerialId == serialId)
        {
            tcs.TrySetResult(true);
        }
    },
    (bizErr) =>
    {
        OnErrorAction(bizErr);
        tcs.SetResult(false);
    }
    );
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_SYNC_TRADE, sst);
            return tcs.Task;
        }
        private void OnQueryPosition(PBPosition rsp)
        {
            UpdatePosition(rsp);
        }

        private void OnUpdatePosition(PBPosition rsp)
        {
            UpdatePosition(rsp);
        }

        protected void UpdatePosition(PBPosition rsp)
        {
            lock (PositionVMCollection)
            {
                PositionVM positionVM = PositionVMCollection.FirstOrDefault(p =>
                    p.Contract == rsp.Contract && (int)p.Direction == rsp.Direction && p.Portfolio == rsp.Portfolio);

                if (rsp.TdPosition + rsp.YdPosition == 0)
                {
                    if (positionVM != null)
                    {
                        positionVM.TodayPosition = rsp.TdPosition;
                        positionVM.YdPosition = rsp.YdPosition;
                        PositionVMCollection.Remove(positionVM);
                        OnPositionUpdated?.Invoke(positionVM);
                    }

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
                            Multiplier = contractInfo == null ? 1 : contractInfo.VolumeMultiple,
                            TodayPosition = rsp.TdPosition,
                            YdPosition = rsp.YdPosition,
                            OpenVolume = rsp.OpenVolume,
                            CloseVolume = rsp.CloseVolume,
                            OpenAmount = rsp.OpenAmount,
                            CloseAmount = rsp.CloseAmount,
                            TdCost = rsp.TdCost,
                            YdCost = rsp.YdCost,
                            OpenCost = rsp.OpenCost,
                            Profit = rsp.Profit,
                            CloseProfit = rsp.CloseProfit,
                            UseMargin = rsp.UseMargin,
                            Position = rsp.YdPosition + rsp.TdPosition,
                            AvgPrice = rsp.AvgPrice,
                            LastPrice = rsp.LastPrice
                        };

                        PositionVMCollection.Add(positionVM);
                        PositionContractSet.Add(rsp.Contract);

                        OnPositionUpdated?.Invoke(positionVM);
                    }
                    else
                    {
                        positionVM.OpenVolume = rsp.OpenVolume;
                        positionVM.CloseVolume = rsp.CloseVolume;
                        positionVM.OpenAmount = rsp.OpenAmount;
                        positionVM.CloseAmount = rsp.CloseAmount;
                        positionVM.OpenCost = rsp.OpenCost;
                        positionVM.Profit = rsp.Profit;
                        positionVM.AvgPrice = rsp.AvgPrice;
                        positionVM.LastPrice = rsp.LastPrice;

                        if (positionVM.YdPosition != rsp.YdPosition || positionVM.TodayPosition != rsp.TdPosition)
                        {
                            positionVM.TodayPosition = rsp.TdPosition;
                            positionVM.YdPosition = rsp.YdPosition;
                            positionVM.Position = rsp.YdPosition + rsp.TdPosition;
                            positionVM.Profit = rsp.Profit;
                            positionVM.AvgPrice = rsp.AvgPrice;
                            positionVM.LastPrice = rsp.LastPrice;
                            OnPositionUpdated?.Invoke(positionVM);
                        }
                    }
                }
            }
        }
        protected void UpdatePositionProfit(PBPositionPnL rsp)
        {
            lock (PositionProfitVMCollection)
            {
                var positionVMCollection = PositionProfitVMCollection.Where(p =>
                    p.Contract == rsp.Contract && p.Portfolio == rsp.Portfolio);
                if (positionVMCollection != null)
                {

                    var buyPositionVM = positionVMCollection.Where(p => p.TdBuyPosition + p.YdBuyPosition != 0).FirstOrDefault();
                    if (buyPositionVM != null)
                    {
                        if (rsp.TdBuyPosition + rsp.YdBuyPosition != 0)
                        {
                            buyPositionVM.AvgBuyPrice = buyPositionVM.AvgPrice = rsp.AvgBuyPrice;
                            buyPositionVM.TdBuyAmount = buyPositionVM.TdAmount = rsp.TdBuyAmount;
                            buyPositionVM.LastPrice = rsp.LastPrice;
                            buyPositionVM.Portfolio = rsp.Portfolio;
                            buyPositionVM.TdBuyPosition = rsp.TdBuyPosition;
                            buyPositionVM.YdBuyPosition = rsp.YdBuyPosition;
                            buyPositionVM.BuyProfit = rsp.BuyProfit;
                            buyPositionVM.Profit = rsp.BuyProfit;
                            buyPositionVM.TodayPosition = rsp.TdBuyPosition;
                            buyPositionVM.YdPosition = rsp.YdBuyPosition;
                            buyPositionVM.Position = rsp.TdBuyPosition + rsp.YdBuyPosition;
                        }
                        else
                            PositionProfitVMCollection.Remove(buyPositionVM);
                        OnPositionProfitUpdated?.Invoke(buyPositionVM);
                    }
                    else
                    {
                        if (rsp.TdBuyPosition + rsp.YdBuyPosition != 0)
                        {
                            var buypositionVM = new PositionVM
                            {
                                Contract = rsp.Contract,
                                Exchange = rsp.Exchange,
                                Portfolio = rsp.Portfolio,
                                OrderDirection = DirectionType.BUY,
                                AvgBuyPrice = rsp.AvgBuyPrice,
                                AvgPrice = rsp.AvgBuyPrice,
                                TdBuyAmount = rsp.TdBuyAmount,
                                TdAmount = rsp.TdBuyAmount,
                                TdBuyPosition = rsp.TdBuyPosition,
                                YdBuyPosition = rsp.YdBuyPosition,
                                LastPrice = rsp.LastPrice,
                                BuyProfit = rsp.BuyProfit,
                                Profit = rsp.BuyProfit,
                                TodayPosition = rsp.TdBuyPosition,
                                YdPosition = rsp.YdBuyPosition,
                                Position = rsp.TdBuyPosition + rsp.YdBuyPosition,
                            };
                            PositionProfitVMCollection.Add(buypositionVM);
                            OnPositionProfitUpdated?.Invoke(buypositionVM);
                        }
                    }



                    var sellPositionVM = positionVMCollection.Where(p => p.TdSellPosition + p.YdSellPosition != 0).FirstOrDefault();
                    if (sellPositionVM != null)
                    {
                        if (rsp.TdSellPosition + rsp.YdSellPosition != 0)
                        {
                            sellPositionVM.Portfolio = rsp.Portfolio;
                            sellPositionVM.AvgSellPrice = sellPositionVM.AvgPrice = rsp.AvgSellPrice;
                            sellPositionVM.TdSellAmount = sellPositionVM.TdAmount = rsp.TdSellAmount;
                            sellPositionVM.TdSellPosition = rsp.TdSellPosition;
                            sellPositionVM.YdSellPosition = rsp.YdSellPosition;
                            sellPositionVM.SellProfit = rsp.SellProfit;
                            sellPositionVM.Profit = rsp.SellProfit;
                            sellPositionVM.TodayPosition = rsp.TdSellPosition;
                            sellPositionVM.YdPosition = rsp.YdSellPosition;
                            sellPositionVM.Position = rsp.TdSellPosition + rsp.YdSellPosition;
                            sellPositionVM.LastPrice = rsp.LastPrice;
                        }
                        else
                            PositionProfitVMCollection.Remove(sellPositionVM);
                        OnPositionProfitUpdated?.Invoke(sellPositionVM);

                    }
                    else
                    {
                        if (rsp.TdSellPosition + rsp.YdSellPosition != 0)
                        {
                            var sellpositionVM = new PositionVM
                            {
                                Contract = rsp.Contract,
                                Exchange = rsp.Exchange,
                                Portfolio = rsp.Portfolio,
                                OrderDirection = DirectionType.SELL,
                                TdSellPosition = rsp.TdSellPosition,
                                YdSellPosition = rsp.YdSellPosition,
                                AvgSellPrice = rsp.AvgSellPrice,
                                AvgPrice = rsp.AvgSellPrice,
                                TdSellAmount = rsp.TdSellAmount,
                                TdAmount = rsp.TdSellAmount,
                                SellProfit = rsp.SellProfit,
                                Profit = rsp.SellProfit,
                                TodayPosition = rsp.TdSellPosition,
                                YdPosition = rsp.YdSellPosition,
                                Position = rsp.TdSellPosition + rsp.YdSellPosition,
                                LastPrice = rsp.LastPrice,
                            };
                            PositionProfitVMCollection.Add(sellpositionVM);
                            OnPositionProfitUpdated?.Invoke(sellPositionVM);

                        }
                    }
                }
                else
                {
                    if (rsp.TdBuyPosition + rsp.YdBuyPosition != 0)
                    {
                        var buypositionVM = new PositionVM
                        {
                            Contract = rsp.Contract,
                            Exchange = rsp.Exchange,
                            Portfolio = rsp.Portfolio,
                            OrderDirection = DirectionType.BUY,
                            TdBuyPosition = rsp.TdBuyPosition,
                            YdBuyPosition = rsp.YdBuyPosition,
                            AvgBuyPrice = rsp.AvgBuyPrice,
                            AvgPrice = rsp.AvgBuyPrice,
                            TdBuyAmount = rsp.TdBuyAmount,
                            TdAmount = rsp.TdBuyAmount,
                            BuyProfit = rsp.BuyProfit,
                            Profit = rsp.BuyProfit,
                            TodayPosition = rsp.TdBuyPosition,
                            YdPosition = rsp.YdBuyPosition,
                            Position = rsp.TdBuyPosition + rsp.YdBuyPosition,
                            LastPrice = rsp.LastPrice,
                        };
                        PositionProfitVMCollection.Add(buypositionVM);
                        OnPositionProfitUpdated?.Invoke(buypositionVM);

                    }
                    if (rsp.TdSellPosition + rsp.YdSellPosition != 0)
                    {
                        var sellpositionVM = new PositionVM
                        {
                            Contract = rsp.Contract,
                            Exchange = rsp.Exchange,
                            Portfolio = rsp.Portfolio,
                            OrderDirection = DirectionType.SELL,
                            TdSellPosition = rsp.TdSellPosition,
                            YdSellPosition = rsp.YdSellPosition,
                            AvgSellPrice = rsp.AvgSellPrice,
                            AvgPrice = rsp.AvgSellPrice,
                            TdSellAmount = rsp.TdSellAmount,
                            TdAmount = rsp.TdSellAmount,
                            SellProfit = rsp.SellProfit,
                            Profit = rsp.SellProfit,
                            TodayPosition = rsp.TdSellPosition,
                            YdPosition = rsp.YdSellPosition,
                            Position = rsp.TdSellPosition + rsp.YdSellPosition,
                            LastPrice = rsp.LastPrice,
                        };
                        PositionProfitVMCollection.Add(sellpositionVM);
                        OnPositionProfitUpdated?.Invoke(sellpositionVM);
                    }
                }
            }
        }
        protected void OnUpdatePositionProfit(PBPositionPnL rsp)
        {
            UpdatePositionProfit(rsp);
        }
        protected void OnQueryPositionProfit(PBPositionPnL rsp)
        {
            UpdatePositionProfit(rsp);
        }

        public event Action<PositionVM> OnPositionUpdated;
        public event Action<PositionVM> OnPositionProfitUpdated;


        private void OnFund(PBAccountInfo rsp)
        {
            FundVM.Commission = rsp.Commission;
            FundVM.BrokerID = rsp.BrokerID;
            FundVM.AccountID = rsp.AccountID;
            FundVM.SettlementID = rsp.SettlementID;
            FundVM.PreMortgage = rsp.PreMortgage;
            if (rsp.PreCredit >= 0)
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
                        VolCondition = (OrderVolType)rsp.VolumeCondition,
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
                        InvestorID = rsp.InvestorID,

                        //rsp.InsertDate
                        //rsp.Message
                        //rsp.OrderType
                        //rsp.StopPrice
                        //rsp.TradingDay
                    };
                    if (rsp.OrderStatus == 4 || rsp.OrderStatus == 5 || rsp.OrderStatus == 13)
                    {
                        string msg = string.Format("{0} {1}", Encoding.UTF8.GetString(rsp.Message.ToByteArray()), rsp.Contract);
                        MessageBoxResult dr = System.Windows.MessageBox.Show(msg, " ", MessageBoxButton.OK, MessageBoxImage.Question);
                    }

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

        public event Action<TradeVM> OnTraded;

        private void OnReturnTrade(PBTradeInfo rsp)
        {
            var trade = new TradeVM()
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
            };

            OnTraded?.Invoke(trade);

            lock (TradeVMCollection)
            {
                if (!TradeVMCollection.Any(t => t.TradeID == rsp.TradeID))
                {
                    TradeVMCollection.Add(trade);
                }
            }
        }

        public virtual void QueryAccountInfo()
        {
            var sst = new StringMap();
            sst.Header = new DataHeader();
            sst.Header.SerialId = NextSerialId;
            MessageWrapper?.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_ACCOUNT_INFO, sst);

        }

        public virtual void QueryPosition()
        {
            var sst = new StringMap();
            sst.Header = new DataHeader();
            sst.Header.SerialId = NextSerialId;
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_POSITION, sst);
        }
        public virtual void QueryPositionProfit()
        {
            var sst = new StringMap();
            sst.Header = new DataHeader();
            sst.Header.SerialId = NextSerialId;
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_POSITIONPNL, sst);
        }
        public void QueryPositionDiffer()
        {
            var sst = new StringMap();
            sst.Header = new DataHeader();
            sst.Header.SerialId = NextSerialId;
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_POSITION_DIFFER, sst);

        }
        public void QueryTradeDiffer()
        {
            TradeDifferVMCollection.Clear();
            var sst = new StringMap();
            sst.Header = new DataHeader();
            sst.Header.SerialId = NextSerialId;
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_TRADE_DIFFER, sst);
        }
        public virtual void QueryOrder()
        {
            var sst = new StringMap();
            sst.Header = new DataHeader();
            sst.Header.SerialId = NextSerialId;
            MessageWrapper?.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_ORDER, sst);

        }

        public virtual void QueryTrade()
        {
            var sst = new StringMap();
            sst.Header = new DataHeader();
            sst.Header.SerialId = NextSerialId;
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_TRADE, sst);
        }

        public virtual void CreateOrder(OrderVM orderVM)
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
            pb.VolCond = (int)orderVM.VolCondition;
            pb.Volume = orderVM.Volume;
            pb.ExecType = (int)orderVM.ExecType;
            pb.Direction = (int)orderVM.Direction;
            pb.Openclose = (int)orderVM.OpenClose;
            if (orderVM.Portfolio != null)
                pb.Portfolio = orderVM.Portfolio;

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_ORDER_NEW, pb);
        }


        public virtual void CancelOrder(OrderVM orderVM)
        {
            var sendobjBld = new PBOrderRequest();
            sendobjBld.Exchange = orderVM.Exchange;
            sendobjBld.Contract = orderVM.Contract;
            sendobjBld.OrderID = orderVM.OrderID;
            sendobjBld.OrderSysID = orderVM.OrderSysID;
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_ORDER_CANCEL, sendobjBld);
        }
        public virtual void AddTrade(TradeVM tradeVM)
        {

            var tradeInfo = new PBTradeInfo();
            if (tradeVM.Contract != null && tradeVM.Volume != 0 && tradeVM.Price != 0 && tradeVM.Portfolio != null)
            {
                tradeInfo.Contract = tradeVM.Contract;
                tradeInfo.Volume = tradeVM.Volume;
                tradeInfo.Price = tradeVM.Price;
                tradeInfo.Portfolio = tradeVM.Portfolio;
                tradeInfo.Direction = (int)tradeVM.Direction;
                tradeInfo.Openclose = (int)tradeVM.OpenClose;
                MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_ADD_MANUAL_TRADE, tradeInfo);
            }

        }
        public virtual void ModifyOrder(OrderVM orderVM)
        {
            CancelOrder(orderVM);
            CreateOrder(orderVM);
        }


        public virtual Task<ObservableCollection<RiskVM>> QueryValuationRiskAsync(QueryValuation queryValuation, string portfolio, int timeout = 10000)
        {
            var sst = new PBValuationRisk();
            var msgId = (uint)BusinessMessageID.MSG_ID_QUERY_VALUATION_RISK;
            var tcs = new TaskCompletionSource<ObservableCollection<RiskVM>>(new CancellationTokenSource(timeout));

            var serialId = NextSerialId;
            sst.Header = new DataHeader { SerialId = serialId };

            sst.Portfolio = portfolio;
            sst.Interest = queryValuation.Interest.HasValue ? queryValuation.Interest.Value : -1;
            if (queryValuation.DaysRemain.HasValue)
                sst.DaysRemain = queryValuation.DaysRemain.Value;

            foreach (var cv in queryValuation.ContractParams)
            {
                var valuation = new PBValuationContract()
                {
                    Contract = cv.Key,
                    Price = cv.Value.Price
                };

                if (cv.Value.Volatitly != 0)
                    valuation.Volatility = cv.Value.Volatitly;

                sst.ContractValue.Add(valuation);
            }

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


        public virtual Task<ObservableCollection<RiskVM>> QueryRiskAsync(string portfolio, int timeout = 10000)
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
                    Rho = risk.Rho,
                    Position = risk.Position,
                    Price = risk.Price,
                    //ContractKey = contractinfo.UnderlyingContract
                });
            }
            //riskList.Add(new RiskVM { Contract="test",Underlying="m",Delta=0.2,Gamma=0.3,Theta=0.4,Vega=0.5,Rho=0.6,Position=3,Portfolio="DCE-M_O"});

            return riskList;
        }

        public virtual Task<bool> SyncContractInfoAsync(bool forced = false)
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

