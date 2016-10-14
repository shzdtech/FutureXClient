using Micro.Future.Message.Business;
using Micro.Future.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public abstract class AbstractOTCHandler : AbstractMessageHandler
    {
        protected IDictionary<string, ObservableCollection<ModelParamsVM>> _modelDict = new Dictionary<string, ObservableCollection<ModelParamsVM>>();

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

        public ObservableCollection<ModelParamsVM> GetModelParamsVMCollection(string modelAim = "pm")
        {
            ObservableCollection<ModelParamsVM> ret;
            if(!_modelDict.TryGetValue(modelAim, out ret))
            {
                ret = new ObservableCollection<ModelParamsVM>();
                _modelDict[modelAim] = ret;
            }

            return ret;
        }


        public ObservableCollection<ContractParamVM> ContractParamVMCollection
        {
            get;
        } = new ObservableCollection<ContractParamVM>();
        public ObservableCollection<TradingDeskVM> TradingDeskVMCollection
        {
            get;
        } = new ObservableCollection<TradingDeskVM>();

        public ObservableCollection<OTCPricingVM> OTCQuoteVMCollection
        {
            get;
        } = new ObservableCollection<OTCPricingVM>();

        public ObservableCollection<PortfolioVM> PortfolioVMCollection   //portfolioVMCollection
        {
            get;
        } = new ObservableCollection<PortfolioVM>();

        public static ObservableCollection<PortfolioVM> staticPortfolioVMCollection = null;

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            MessageWrapper.RegisterAction<PBPricingDataList, ExceptionMessage>
                        ((uint)BusinessMessageID.MSG_ID_SUB_PRICING, OnSubMarketDataSuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<PBPricingData, ExceptionMessage>
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
            MessageWrapper.RegisterAction<PBPortfolioList, ExceptionMessage>
                      ((uint)BusinessMessageID.MSG_ID_QUERY_PORTFOLIO, OnQueryPortfolioSuccessAction, OnErrorAction);
        }

        public Task<ModelParamsVM> QueryModelParamsAsync(string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
                return Task.FromResult<ModelParamsVM>(null);

            var msgId = (uint)SystemMessageID.MSG_ID_QUERY_MODELPARAMS;

            var tcs = new TaskCompletionSource<ModelParamsVM>();

            var serialId = NextSerialId;

            ModelParams modelParams = new ModelParams();
            modelParams.Header = new DataHeader { SerialId = serialId };
            modelParams.InstanceName = modelName;

            #region callback
            MessageWrapper.RegisterAction<ModelParams, ExceptionMessage>
            (msgId,
            (resp) =>
            {
                if (resp.Header?.SerialId == serialId)
                {
                    tcs.TrySetResult(OnQueryModelParamsSuccess(resp));
                }
            },
            (ExceptionMessage bizErr) =>
            {
                OnErrorAction(bizErr);
                tcs.SetResult(null);
            }
            );
            #endregion

            MessageWrapper.SendMessage(msgId, modelParams);

            return tcs.Task;
        }


        public Task<IDictionary<string, ObservableCollection<ModelParamsVM>>> QueryAllModelParamsAsync()
        {
            var msgId = (uint)SystemMessageID.MSG_ID_QUERY_MODELPARAMS;

            var tcs = new TaskCompletionSource<IDictionary<string, ObservableCollection<ModelParamsVM>>>();

            var serialId = NextSerialId;

            ModelParams modelParams = new ModelParams();
            modelParams.Header = new DataHeader() { SerialId = serialId };

            #region callback
            MessageWrapper.RegisterAction<ModelParams, ExceptionMessage>
            (msgId,
            (resp) =>
            {
                if (resp.Header?.SerialId == serialId)
                {
                    OnQueryModelParamsSuccess(resp);

                    if (resp.Header == null || !resp.Header.HasMore)
                        tcs.TrySetResult(_modelDict);
                }
            },
            (ExceptionMessage bizErr) =>
            {
                OnErrorAction(bizErr);
                tcs.SetResult(null);
            }
            );
            #endregion

            MessageWrapper.SendMessage(msgId, modelParams);

            return tcs.Task;

        }


        protected ModelParamsVM OnQueryModelParamsSuccess(ModelParams resp)
        {
            var modelParamsVMCollection = GetModelParamsVMCollection(resp.ModelAim);
            ModelParamsVM ret = modelParamsVMCollection.FirstOrDefault(c => c.InstanceName == resp.InstanceName);
            if (ret == null && !string.IsNullOrEmpty(resp.InstanceName))
            {
                ret = new ModelParamsVM()
                {
                    InstanceName = resp.InstanceName,
                    Model = resp.Model,
                    ModelAim = resp.ModelAim
                };

                foreach (var param in resp.Params)
                {
                    ret.Params.Add(new NamedParamVM()
                    {
                        Name = param.Key,
                        Value = param.Value,
                    });
                }

                modelParamsVMCollection.Add(ret);
            }

            return ret;
        }


        public void UpdateModelParams(string modelName, string paramName, double paramValue)
        {
            var model = new ModelParams();
            model.InstanceName = modelName;
            model.Params[paramName] = paramValue;

            MessageWrapper.SendMessage((uint)SystemMessageID.MSG_ID_UPDATE_MODELPARAMS, model);
        }

        public void NewWingModelInstance(string modelName)
        {
            var modelParam = new ModelParams();
            modelParam.InstanceName = modelName;
            modelParam.Model = "wing";

            MessageWrapper.SendMessage((uint)SystemMessageID.MSG_ID_UPDATE_MODELPARAMS, modelParam);
        }

        private void OnQueryPortfolioSuccessAction(PBPortfolioList PB)
        {
            PortfolioVMCollection.Clear();
            foreach (var portfolio in PB.Portfolio)
            {
                PortfolioVMCollection.Add(new PortfolioVM { Name = portfolio.Name });
            }
            staticPortfolioVMCollection = PortfolioVMCollection;

        }

        public void CreatePortfolios(IEnumerable<PortfolioVM> portfolios)
        {
            var portfolioList = new PBPortfolioList();

            foreach (var portfolio in portfolios)
            {
                portfolioList.Portfolio.Add(new PBPortfolio { Name = portfolio.Name });
            }

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSD_ID_PORTFOLIO_NEW, portfolioList);
        }

        //to send the created PortfolioVMCollection
        public static ObservableCollection<PortfolioVM> getPortfolioVMCollection()
        {
            return staticPortfolioVMCollection;
        }


        protected void OnUpdateStrategySuccessAction(PBStrategyList PB)
        {
            foreach (var strategy in PB.Strategy)
            {
                var strategyVM = StrategyVMCollection.FindContract(strategy.Exchange, strategy.Contract);
                if (strategyVM != null)
                {
                    strategyVM.Hedging = strategy.Hedging;
                    strategyVM.Depth = strategy.Depth;
                    strategyVM.BidEnabled = strategy.BidEnabled;
                    strategyVM.AskEnabled = strategy.AskEnabled;
                    strategyVM.BidQT = strategy.BidQT;
                    strategyVM.AskQT = strategy.AskQT;
                    break;
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

        public void UpdateStrategy(StrategyVM sVM)
        {
            var strategy = new PBStrategy();
            strategy.Exchange = sVM.Exchange;
            strategy.Contract = sVM.Contract;
            strategy.Depth = sVM.Depth;
            strategy.BidQT = sVM.BidQT;
            strategy.AskQT = sVM.AskQT;
            strategy.Hedging = sVM.Hedging;
            strategy.AskEnabled = sVM.AskEnabled;
            strategy.BidEnabled = sVM.BidEnabled;
            strategy.IvModel = sVM.IVModel;
            strategy.VolModel = sVM.VolModel;
            strategy.PricingModel = sVM.PricingModel;

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_MODIFY_STRATEGY, strategy);
        }

        public void UpdateStrategyPricingContracts(StrategyVM sVM)
        {
            var strategy = new PBStrategy();
            strategy.Exchange = sVM.Exchange;
            strategy.Contract = sVM.Contract;
            strategy.Depth = sVM.Depth;
            strategy.BidQT = sVM.BidQT;
            strategy.AskQT = sVM.AskQT;
            strategy.Hedging = sVM.Hedging;
            strategy.AskEnabled = sVM.AskEnabled;
            strategy.BidEnabled = sVM.BidEnabled;
            strategy.IvModel = sVM.IVModel;
            strategy.VolModel = sVM.VolModel;
            strategy.PricingModel = sVM.PricingModel;

            foreach (var pc in strategy.PricingContracts)
            {
                strategy.PricingContracts.Add(new PBPricingContract() { Exchange = pc.Exchange, Contract = pc.Contract, Weight = pc.Weight });
            }

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_MODIFY_STRATEGY, strategy);
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
            var cpBd = new PBContractParam
            {
                Exchange = cpVM.Exchange,
                Contract = cpVM.Contract,
                DepthVol = cpVM.DepthVol,
                Gamma = cpVM.Gamma
            };
            var cpLstBd = new PBContractParamList();
            cpLstBd.Params.Add(cpBd);
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_MODIFY_CONTRACT_PARAM, cpLstBd);
        }

        protected void OnUpdateSuccessAction(Result result)
        {

        }

        public Task<ObservableCollection<StrategyVM>> QueryStrategyAsync()
        {
            var sst = new StringMap();
            var msgId = (uint)BusinessMessageID.MSG_ID_QUERY_STRATEGY;
            var tcs = new TaskCompletionSource<ObservableCollection<StrategyVM>>();

            var serialId = NextSerialId;
            sst.Header = new DataHeader { SerialId = serialId };

            MessageWrapper.RegisterAction<PBStrategyList, ExceptionMessage>
                (msgId,
                (resp) =>
                {
                    if (resp.Header?.SerialId == serialId)
                    {
                        OnQueryStrategySuccessAction(resp);
                
                        tcs.TrySetResult(StrategyVMCollection);
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

        public void QueryStrategy()
        {
            var sst = new StringMap();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_STRATEGY, sst);
        }

        protected void OnQueryStrategySuccessAction(PBStrategyList PB)
        {
            foreach (var strategy in PB.Strategy)
            {

                var strategyVM = StrategyVMCollection.FindContract(strategy.Exchange, strategy.Contract);
                if (strategyVM == null)
                {
                    strategyVM = new StrategyVM(this);
                    StrategyVMCollection.Add(strategyVM);
                }

                strategyVM.Exchange = strategy.Exchange;
                strategyVM.Contract = strategy.Contract;
                strategyVM.Hedging = strategy.Hedging;
                strategyVM.Underlying = strategy.Underlying;
                strategyVM.StrategySym = strategy.Symbol;
                strategyVM.Description = strategy.Description;
                strategyVM.Depth = strategy.Depth;
                strategyVM.AskEnabled = strategy.AskEnabled;
                strategyVM.BidEnabled = strategy.BidEnabled;
                strategyVM.BidQT = strategy.BidQT;
                strategyVM.AskQT = strategy.AskQT;
                strategyVM.IVModel = strategy.IvModel;
                strategyVM.VolModel = strategy.VolModel;
                strategyVM.PricingModel = strategy.PricingModel;

                foreach (var wtContract in strategy.PricingContracts)
                {
                    strategyVM.PricingContractParams.Add(
                        new PricingContractParamVM()
                        {
                            Exchange = wtContract.Exchange,
                            Contract = wtContract.Contract,
                            Weight = wtContract.Weight
                        });
                }
            }
        }

        public void QueryContractParam()
        {
            var sst = new StringMap();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_CONTRACT_PARAM, sst);
        }

        public void QueryPortfolio()
        {
            var sst = new StringMap();
            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_QUERY_PORTFOLIO, sst);
        }

        public Task<ObservableCollection<PortfolioVM>> QueryPortfolioAsync()
        {
            var sst = new StringMap();
            var msgId = (uint)BusinessMessageID.MSG_ID_QUERY_PORTFOLIO;
            var tcs = new TaskCompletionSource<ObservableCollection<PortfolioVM>>();

            var serialId = NextSerialId;
            sst.Header = new DataHeader { SerialId = serialId };

            MessageWrapper.RegisterAction<PBPortfolioList, ExceptionMessage>
                (msgId,
                (resp) =>
                {
                    if (resp.Header?.SerialId == serialId)
                    {
                        OnQueryPortfolioSuccessAction(resp);

                        tcs.TrySetResult(PortfolioVMCollection);
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



        protected void OnQueryContractParamSuccessAction(PBContractParamList PB)
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

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_SUB_PRICING, sst);
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

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_UNSUB_MARKETDATA, sst);
        }

        protected void OnSubMarketDataSuccessAction(PBPricingDataList PB)
        {
            foreach (var md in PB.PricingData)
            {
                OTCQuoteVMCollection.Add(new OTCPricingVM()
                {
                    Exchange = md.Exchange,
                    Contract = md.Contract,
                });
            }
        }

        protected virtual void OnReturningPricing(PBPricingData md)
        {
            var quote = OTCQuoteVMCollection.FindContract(md.Exchange, md.Contract);
            if (quote != null)
            {
                quote.BidPrice = md.BidPrice;
                quote.AskPrice = md.AskPrice;
            }
            var ps = from s in StrategyVMCollection
                     where (s.Exchange == md.Exchange && s.Contract == md.Contract)
                     select s;
            foreach (var s in ps)
            {
                s.BidPrice = md.BidPrice;
                s.AskPrice = md.AskPrice;
            }
        }
    }
}
