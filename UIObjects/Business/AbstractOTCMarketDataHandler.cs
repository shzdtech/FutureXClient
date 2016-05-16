using Micro.Future.Message.Business;
using Micro.Future.ViewModel;
using System.Collections.ObjectModel;
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

            MessageWrapper.RegisterAction<PBPricingDataList, BizErrorMsg>
                        ((uint)BusinessMessageID.MSG_ID_SUB_PRICING, OnSubMarketDataSuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<PBPricingDataList, BizErrorMsg>
                            ((uint)BusinessMessageID.MSG_ID_RTN_PRICING, OnReturningPricing, OnErrorAction);
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
            var sst = SimpleStringTable.CreateBuilder();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_TRADINGDESK, sst.Build());
        }

        protected void OnQueryTradingDeskSuccessAction(PBUserInfoList obj)
        {
            if (TradingDeskVMCollection != null)
            {
                foreach (var userInfo in obj.UserInfoList)
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
                foreach (var strategy in PB.StrategyList)
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
                }
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
                foreach (var param in PB.ParamsList)
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
            var sst = SimpleStringTable.CreateBuilder();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_SUB_PRICING, sst.Build());
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
                foreach (var md in PB.PricingList)
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
                foreach (var md in PB.PricingList)
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
