using Micro.Future.Message.Business;
using Micro.Future.ViewModel;
using System.Text;

namespace Micro.Future.Message
{
    public abstract class AbstractOTCMarketDataHandler :
        AbstractMessageHandler
    {
        protected void OnErrorAction(BizErrorMsg bizErr)
        {
            RaiseOnError(
                new MessageException(bizErr.MessageId, bizErr.Errorcode,
                Encoding.UTF8.GetString(bizErr.Description.ToByteArray()),
                bizErr.Syserrcode));
        }

        public DispatchObservableCollection<StrategyVM> StrategyVMCollection
        {
            get;
            set;
        }

        public DispatchObservableCollection<ContractParamVM> ContractParamVMCollection
        {
            get;
            set;
        }
        public DispatchObservableCollection<TradingDeskVM> TradingDeskVMCollection
        {
            get;
            set;
        }

        public DispatchObservableCollection<OTCQuoteVM> OTCQuoteVMCollection
        {
            get;
            set;
        }

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {

            MessageWrapper.RegisterAction<PBPricingDataList, BizErrorMsg>
                        ((uint)BusinessMessageID.MSG_ID_OTC_SUB_MARKETDATA, OnSubMarketDataSuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<PBPricingDataList, BizErrorMsg>
                            ((uint)BusinessMessageID.MSG_ID_OTC_RET_MARKETDATA, OnReturningPricing, OnErrorAction);
            MessageWrapper.RegisterAction<PBStrategyList, BizErrorMsg>
                        ((uint)BusinessMessageID.MSG_ID_QUERY_STRATEGY, OnQueryStrategySuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<PBContractParamList, BizErrorMsg>
                       ((uint)BusinessMessageID.MSG_ID_QUERY_CONTRACT_PARAM, OnQueryContractParamSuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<PBStrategyList, BizErrorMsg>
                       ((uint)BusinessMessageID.MSG_ID_MODIFY_STRATEGY, OnUpdateStrategySuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<Result, BizErrorMsg>
                       ((uint)BusinessMessageID.MSG_ID_MODIFY_CONTRACT_PARAM, OnUpdateSuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<Result, BizErrorMsg>
                       ((uint)BusinessMessageID.MSG_ID_MODIFY_USER_PARAM, OnUpdateSuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<PBUserInfoList, BizErrorMsg>
                      ((uint)BusinessMessageID.MSG_ID_QUERY_TRADINGDESK, OnQueryTradingDeskSuccessAction, OnErrorAction);

        }

        private void OnUpdateStrategySuccessAction(PBStrategyList PB)
        {
            if (StrategyVMCollection != null)
            {
                foreach (var strategy in PB.StrategyList)
                {
                    foreach (var strategyVM in StrategyVMCollection)
                    {
                        if (strategyVM.EqualContract(strategy.Exchange, strategy.Contract))
                        {
                            StrategyVMCollection.Dispatcher.Invoke(() =>
                            {
                                strategyVM.IsTradingAllowed = strategy.AllowTrading;
                                strategyVM.Offset = strategy.Offset;
                                strategyVM.Depth = strategy.Depth;
                                strategyVM.Spread = strategy.Spread;
                                strategyVM.Enabled = strategy.Enabled;
                                strategyVM.Quantity = strategy.Quantity;
                            });
                            break;
                        }
                    }
                }
            }
        }

        public void QueryTradingDesk()
        {
            var sst = SimpleStringTable.CreateBuilder();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_TRADINGDESK, sst.Build());
        }

        protected void OnQueryTradingDeskSuccessAction(PBUserInfoList obj)
        {
            if (TradingDeskVMCollection != null)
            {
                TradingDeskVMCollection.Dispatcher.Invoke(() =>
                {
                    foreach (var userInfo in obj.UserInfoList)
                    {
                        TradingDeskVMCollection.Add(new TradingDeskVM()
                        {
                            Name = userInfo.Name,
                            ContactNum = userInfo.ContactNum,
                            Email = userInfo.Email
                        });
                    }
                });
            }
        }

        public void UpdateStrategy(StrategyVM sVM)
        {
            var strategy = PBStrategy.CreateBuilder();
            strategy.Exchange = sVM.Exchange;
            strategy.Contract = sVM.Contract;
            strategy.Offset = (float)sVM.Offset;
            strategy.Depth = sVM.Depth;
            strategy.Spread = sVM.Spread;
            strategy.Quantity = sVM.Quantity;
            strategy.AllowTrading = sVM.IsTradingAllowed;
            strategy.Enabled = sVM.Enabled;
            var strategyListBd = PBStrategyList.CreateBuilder();
            strategyListBd.AddStrategy(strategy);
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_MODIFY_STRATEGY, strategyListBd.Build());
        }

        public void UpdateQuantity(string exchange, string contract, int quantity)
        {
            var userParamBd = PBOTCUserParam.CreateBuilder();
            userParamBd.Exchange = exchange;
            userParamBd.Contract = contract;
            userParamBd.Quantity = quantity;
            var userParamListBd = PBOTCUserParamList.CreateBuilder().AddParams(userParamBd);
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_MODIFY_USER_PARAM, userParamListBd.Build());
        }

        public void UpdateContractParam(ContractParamVM cpVM)
        {
            var cpBd = PBContractParam.CreateBuilder();
            cpBd.Exchange = cpVM.Exchange;
            cpBd.Contract = cpVM.Contract;
            cpBd.DepthVol = cpVM.DepthVol;
            cpBd.Gamma = cpVM.Gamma;
            var cpLstBd = PBContractParamList.CreateBuilder().AddParams(cpBd);
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_MODIFY_CONTRACT_PARAM, cpLstBd.Build());
        }

        protected void OnUpdateSuccessAction(Result result)
        {

        }

        public void QueryStrategy()
        {
            var sst = SimpleStringTable.CreateBuilder();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_STRATEGY, sst.Build());
        }

        protected void OnQueryStrategySuccessAction(PBStrategyList PB)
        {
            if (StrategyVMCollection != null)
            {
                StrategyVMCollection.Dispatcher.Invoke(() =>
                {
                    foreach (var strategy in PB.StrategyList)
                    {
                        var strategyVM = new StrategyVM();
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

                        foreach (var param in strategy.ParamsList)
                        {
                            strategyVM.Params.Add(
                                new NamedParamVM()
                                {
                                    Name = param.Name,
                                    Value = param.Value,
                                });
                        }

                        foreach (var wtContract in strategy.WeightContractList)
                        {
                            strategyVM.BaseContractParams.Add(
                                new BaseContractParamVM()
                                {
                                    Exchange = wtContract.Exchange,
                                    Contract = wtContract.Contract,
                                    Weight = wtContract.Weight
                                });
                        }
                        StrategyVMCollection.Add(strategyVM);
                    }
                });
            }
        }

        public void QueryContractParam()
        {
            var sst = SimpleStringTable.CreateBuilder();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_CONTRACT_PARAM, sst.Build());
        }

        protected void OnQueryContractParamSuccessAction(PBContractParamList PB)
        {
            if (ContractParamVMCollection != null)
            {
                ContractParamVMCollection.Dispatcher.Invoke(() =>
                {
                    foreach (var param in PB.ParamsList)
                    {
                        ContractParamVMCollection.Add(new ContractParamVM()
                            {
                                Exchange = param.Exchange,
                                Contract = param.Contract,
                                DepthVol = param.DepthVol,
                                Gamma = param.Gamma
                            });
                    }
                });
            }

        }

        public void SubMarketData()
        {
            var sst = SimpleStringTable.CreateBuilder();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_OTC_SUB_MARKETDATA, sst.Build());
        }

        public void UnsubMarketData()
        {
            var sst = SimpleStringTable.CreateBuilder();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_UNSUB_MARKETDATA, sst.Build());
        }

        protected void OnSubMarketDataSuccessAction(PBPricingDataList PB)
        {
            if (OTCQuoteVMCollection != null)
            {
                OTCQuoteVMCollection.Dispatcher.Invoke(() =>
                {
                    foreach (var md in PB.PricingList)
                    {
                        OTCQuoteVMCollection.Add(new OTCQuoteVM()
                        {
                            Exchange = md.Exchange,
                            Contract = md.Contract,
                            Quantity = 1,
                        });
                    }
                });

            }
        }

        protected virtual void OnReturningPricing(PBPricingDataList PB)
        {
            if (OTCQuoteVMCollection != null)
            {
                OTCQuoteVMCollection.Dispatcher.Invoke(() =>
                {
                    foreach (var md in PB.PricingList)
                    {
                        foreach (OTCQuoteVM quote in OTCQuoteVMCollection)
                        {
                            if (quote.Contract == md.Contract)
                            {
                                quote.BidPrice = md.BidPrice;
                                quote.AskPrice = md.AskPrice;
                                break;
                            }
                        }
                    }
                });
            }
        }
    }
}
