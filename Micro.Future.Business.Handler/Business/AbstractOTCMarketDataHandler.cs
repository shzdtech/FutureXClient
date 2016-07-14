﻿using Micro.Future.Message.Business;
using Micro.Future.ViewModel;
using System.Collections.ObjectModel;
using System.Text;

namespace Micro.Future.Message
{
    public abstract class AbstractOTCMarketDataHandler :
        AbstractMessageHandler
    {
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

        public ObservableCollection<StrategyVM> StrategyVMCollection
        {
            get;
        } = new ObservableCollection<StrategyVM>();

        public ObservableCollection<ContractParamVM> ContractParamVMCollection
        {
            get;
        } = new ObservableCollection<ContractParamVM>();
        public ObservableCollection<TradingDeskVM> TradingDeskVMCollection
        {
            get;
        } = new ObservableCollection<TradingDeskVM>();

        public ObservableCollection<OTCQuoteVM> OTCQuoteVMCollection
        {
            get;
        } = new ObservableCollection<OTCQuoteVM>();

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {

            MessageWrapper.RegisterAction<PBPricingDataList, ExceptionMessage>
                        ((uint)BusinessMessageID.MSG_ID_SUB_PRICING, OnSubMarketDataSuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<PBPricingDataList, ExceptionMessage>
                            ((uint)BusinessMessageID.MSG_ID_RTN_PRICING, OnReturningPricing, OnErrorAction);
            MessageWrapper.RegisterAction<PBStrategyList, ExceptionMessage>
                        ((uint)BusinessMessageID.MSG_ID_QUERY_STRATEGY, OnQueryStrategySuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<PBContractParamList, ExceptionMessage>
                       ((uint)BusinessMessageID.MSG_ID_QUERY_CONTRACT_PARAM, OnQueryContractParamSuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<PBStrategyList, ExceptionMessage>
                       ((uint)BusinessMessageID.MSG_ID_MODIFY_STRATEGY, OnUpdateStrategySuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<Result, ExceptionMessage>
                       ((uint)BusinessMessageID.MSG_ID_MODIFY_CONTRACT_PARAM, OnUpdateSuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<Result, ExceptionMessage>
                       ((uint)BusinessMessageID.MSG_ID_MODIFY_USER_PARAM, OnUpdateSuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<PBUserInfoList, ExceptionMessage>
                      ((uint)BusinessMessageID.MSG_ID_QUERY_TRADINGDESK, OnQueryTradingDeskSuccessAction, OnErrorAction);

        }

        private void OnUpdateStrategySuccessAction(PBStrategyList PB)
        {
            if (StrategyVMCollection != null)
            {
                foreach (var strategy in PB.Strategy)
                {
                    var strategyVM = StrategyVMCollection.FindContract(strategy.Exchange, strategy.Contract);
                    if (strategyVM != null)
                    {
                        strategyVM.IsTradingAllowed = strategy.AllowTrading;
                        strategyVM.Offset = strategy.Offset;
                        strategyVM.Depth = strategy.Depth;
                        strategyVM.Spread = strategy.Spread;
                        strategyVM.Enabled = strategy.Enabled;
                        strategyVM.Quantity = strategy.Quantity;
                        break;
                    }
                }
            }
        }

        public void QueryTradingDesk()
        {
            var sst = new StringMap();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_TRADINGDESK, sst);
        }

        protected void OnQueryTradingDeskSuccessAction(PBUserInfoList obj)
        {
            if (TradingDeskVMCollection != null)
            {
                foreach (var userInfo in obj.UserInfo)
                {
                    TradingDeskVMCollection.Add(new TradingDeskVM()
                    {
                        Name = userInfo.LastName + " " + userInfo.FirstName,
                        ContactNum = userInfo.ContactNum,
                        Email = userInfo.Email
                    });
                }
            }
        }

        public void UpdateStrategy(StrategyVM sVM)
        {
            var strategy = new PBStrategy();
            strategy.Exchange = sVM.Exchange;
            strategy.Contract = sVM.Contract;
            strategy.Offset = (float)sVM.Offset;
            strategy.Depth = sVM.Depth;
            strategy.Spread = sVM.Spread;
            strategy.Quantity = sVM.Quantity;
            strategy.AllowTrading = sVM.IsTradingAllowed;
            strategy.Enabled = sVM.Enabled;
            var strategyListBd = new PBStrategyList();
            strategyListBd.Strategy.Add(strategy);
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_MODIFY_STRATEGY, strategyListBd);
        }

        public void UpdateQuantity(string exchange, string contract, int quantity)
        {
            var userParamBd = new PBOTCUserParam();
            userParamBd.Exchange = exchange;
            userParamBd.Contract = contract;
            userParamBd.Quantity = quantity;
            var userParamListBd = new PBOTCUserParamList();
            userParamListBd.Params.Add(userParamBd);
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_MODIFY_USER_PARAM, userParamListBd);
        }

        public void UpdateContractParam(ContractParamVM cpVM)
        {
            var cpBd = new PBContractParam();
            cpBd.Exchange = cpVM.Exchange;
            cpBd.Contract = cpVM.Contract;
            cpBd.DepthVol = cpVM.DepthVol;
            cpBd.Gamma = cpVM.Gamma;
            var cpLstBd = new PBContractParamList();
            cpLstBd.Params.Add(cpBd);
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_MODIFY_CONTRACT_PARAM, cpLstBd);
        }

        protected void OnUpdateSuccessAction(Result result)
        {

        }

        public void QueryStrategy()
        {
            var sst = new StringMap();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_STRATEGY, sst);
        }

        protected void OnQueryStrategySuccessAction(PBStrategyList PB)
        {
            if (StrategyVMCollection != null)
            {
                foreach (var strategy in PB.Strategy)
                {

                    var strategyVM = StrategyVMCollection.FindContract(strategy.Exchange, strategy.Contract);
                    if (strategyVM == null)
                    {
                        strategyVM = new StrategyVM();
                        StrategyVMCollection.Add(strategyVM);
                    }

                    strategyVM.Exchange = strategy.Exchange;
                    strategyVM.Contract = strategy.Contract;
                    strategyVM.IsTradingAllowed = strategy.AllowTrading;
                    strategyVM.Underlying = strategy.Underlying;
                    strategyVM.StrategySym = strategy.Symbol;
                    strategyVM.Description = strategy.Description;
                    strategyVM.Offset = strategy.Offset;
                    strategyVM.Depth = strategy.Depth;
                    strategyVM.Spread = strategy.Spread;
                    strategyVM.Enabled = strategy.Enabled;
                    strategyVM.Quantity = strategy.Quantity;

                    foreach (var param in strategy.Params)
                    {
                        strategyVM.Params.Add(
                            new NamedParamVM()
                            {
                                Name = param.Name,
                                Value = param.Value,
                            });
                    }

                    foreach (var wtContract in strategy.WeightContract)
                    {
                        strategyVM.BaseContractParams.Add(
                            new BaseContractParamVM()
                            {
                                Exchange = wtContract.Exchange,
                                Contract = wtContract.Contract,
                                Weight = wtContract.Weight
                            });
                    }
                }
            }
        }

        public void QueryContractParam()
        {
            var sst = new StringMap();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_CONTRACT_PARAM, sst);
        }

        protected void OnQueryContractParamSuccessAction(PBContractParamList PB)
        {
            if (ContractParamVMCollection != null)
            {
                foreach (var param in PB.Params)
                {
                    var contractParamVM = ContractParamVMCollection.
                        FindContract(param.Exchange, param.Contract);
                    if (contractParamVM == null)
                    {
                        contractParamVM = new ContractParamVM();
                        ContractParamVMCollection.Add(contractParamVM);
                    }

                    contractParamVM.Exchange = param.Exchange;
                    contractParamVM.Contract = param.Contract;
                    contractParamVM.DepthVol = param.DepthVol;
                    contractParamVM.Gamma = param.Gamma;
                }
            }

        }

        public void SubMarketData()
        {
            var sst = new SimpleStringTable();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_SUB_PRICING, sst);
        }

        public void UnsubMarketData()
        {
            var sst = new SimpleStringTable();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_UNSUB_MARKETDATA, sst);
        }

        protected void OnSubMarketDataSuccessAction(PBPricingDataList PB)
        {
            if (OTCQuoteVMCollection != null)
            {
                foreach (var md in PB.Pricing)
                {
                    OTCQuoteVMCollection.Add(new OTCQuoteVM()
                    {
                        Exchange = md.Exchange,
                        Contract = md.Contract,
                        Quantity = 1,
                    });
                }
            }
        }

        protected virtual void OnReturningPricing(PBPricingDataList PB)
        {
            if (OTCQuoteVMCollection != null)
            {
                foreach (var md in PB.Pricing)
                {
                    var quote = OTCQuoteVMCollection.FindContract(md.Exchange, md.Contract);
                    if (quote != null)
                    {
                        quote.BidPrice = md.BidPrice;
                        quote.AskPrice = md.AskPrice;
                    }
                }
            }
        }
    }
}
