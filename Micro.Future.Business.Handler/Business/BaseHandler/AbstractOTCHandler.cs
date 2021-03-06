﻿using Micro.Future.Message.Business;
using Micro.Future.ViewModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Threading;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.LocalStorage;
using System.Collections.Concurrent;

namespace Micro.Future.Message
{
    public abstract class AbstractOTCHandler : AbstractMessageHandler
    {
        public event Action<StrategyVM> OnStrategyUpdated;
        public ConcurrentDictionary<ContractKeyVM, WeakReference<ContractKeyVM>> TradingDeskDataMap
        {
            get;
        } = new ConcurrentDictionary<ContractKeyVM, WeakReference<ContractKeyVM>>();

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
            if (!_modelDict.TryGetValue(modelAim, out ret))
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


        public ObservableCollection<PortfolioVM> PortfolioVMCollection   //portfolioVMCollection
        {
            get;
        } = new ObservableCollection<PortfolioVM>();




        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            MessageWrapper.RegisterAction<PBStrategyList, ExceptionMessage>
                        ((uint)BusinessMessageID.MSG_ID_QUERY_STRATEGY, OnQueryStrategySuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<PBContractParamList, ExceptionMessage>
                       ((uint)BusinessMessageID.MSG_ID_QUERY_CONTRACT_PARAM, OnQueryContractParamSuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<PBStrategy, ExceptionMessage>
                       ((uint)BusinessMessageID.MSG_ID_MODIFY_STRATEGY, OnUpdateStrategySuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<Result, ExceptionMessage>
                       ((uint)BusinessMessageID.MSG_ID_MODIFY_CONTRACT_PARAM, OnUpdateSuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<Result, ExceptionMessage>
                       ((uint)BusinessMessageID.MSG_ID_MODIFY_USER_PARAM, OnUpdateSuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<PBUserInfoList, ExceptionMessage>
                      ((uint)BusinessMessageID.MSG_ID_QUERY_TRADINGDESK, OnQueryTradingDeskSuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<PBPortfolioList, ExceptionMessage>
                      ((uint)BusinessMessageID.MSG_ID_QUERY_PORTFOLIO, OnQueryPortfolioSuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<PBPortfolio, ExceptionMessage>
                     ((uint)BusinessMessageID.MSG_ID_PORTFOLIO_NEW, OnPortfolioUpdated, OnErrorAction);
            MessageWrapper.RegisterAction<PBPortfolio, ExceptionMessage>
                     ((uint)BusinessMessageID.MSG_ID_MODIFY_PORTFOLIO, OnPortfolioUpdated, OnErrorAction);
            MessageWrapper.RegisterAction<PBPortfolio, ExceptionMessage>
                     ((uint)BusinessMessageID.MSG_ID_HEDGE_CONTRACT_UPDATE, OnPortfolioHedgeContracts, OnErrorAction);
            MessageWrapper.RegisterAction<PBStrategyList, ExceptionMessage>
                      ((uint)BusinessMessageID.MSG_ID_MODIFY_PRICING_CONTRACT, OnQueryStrategySuccessAction, OnErrorAction);
            MessageWrapper.RegisterAction<PBInstrumentList, ExceptionMessage>
                      ((uint)BusinessMessageID.MSG_ID_UNSUB_TRADINGDESK_PRICING, UnsubTDSuccessAction, OnErrorAction);
        }

        public Task<ModelParamsVM> QueryModelParamsAsync(string modelName, int timeout = 10000)
        {
            if (string.IsNullOrEmpty(modelName))
                return Task.FromResult<ModelParamsVM>(null);

            var msgId = (uint)SystemMessageID.MSG_ID_QUERY_MODELPARAMS;

            var tcs = new TaskCompletionSource<ModelParamsVM>(new CancellationTokenSource(timeout));

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


        public Task<IDictionary<string, ObservableCollection<ModelParamsVM>>> QueryAllModelParamsAsync(int timeout = 10000)
        {
            var msgId = (uint)SystemMessageID.MSG_ID_QUERY_MODELPARAMS;

            var tcs = new TaskCompletionSource<IDictionary<string, ObservableCollection<ModelParamsVM>>>(new CancellationTokenSource(timeout));

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
            ModelParamsVM ret = null;
            if (!string.IsNullOrEmpty(resp.InstanceName))
            {
                ret = modelParamsVMCollection.FirstOrDefault(c => c.InstanceName == resp.InstanceName);
                if (ret == null)
                {
                    ret = new ModelParamsVM()
                    {
                        InstanceName = resp.InstanceName,
                        Model = resp.Model,
                        ModelAim = resp.ModelAim
                    };
                    modelParamsVMCollection.Add(ret);
                }

                foreach (var param in resp.Params)
                {
                    ret[param.Key] = new NamedParamVM()
                    {
                        Name = param.Key,
                        Value = param.Value,
                    };
                }
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

        public void UpdateModelParams(string modelName, IDictionary<string, double> keyvalues)
        {
            var model = new ModelParams();
            model.InstanceName = modelName;
            foreach (var pair in keyvalues)
                model.Params[pair.Key] = pair.Value;

            MessageWrapper.SendMessage((uint)SystemMessageID.MSG_ID_UPDATE_MODELPARAMS, model);
        }

        public void UpdateTempModelParams(string modelName, string paramName, double paramValue)
        {
            var model = new ModelParams();
            model.InstanceName = modelName;
            model.Params[paramName] = paramValue;

            MessageWrapper.SendMessage((uint)SystemMessageID.MSG_ID_UPDATE_TEMPMODELPARAMS, model);
        }

        public void RemoveTempModel(string modelName)
        {
            var model = new ModelParams();
            model.InstanceName = modelName;

            MessageWrapper.SendMessage((uint)SystemMessageID.MSG_ID_UPDATE_TEMPMODELPARAMS, model);
        }


        public void NewWingModelInstance(string modelName)
        {
            var modelParam = new ModelParams();
            modelParam.InstanceName = modelName;
            modelParam.Model = "wing";

            MessageWrapper.SendMessage((uint)SystemMessageID.MSG_ID_UPDATE_MODELPARAMS, modelParam);
        }

        public void CreatePortfolio(string portfolio)
        {
            var portfolioList = new PBPortfolioList();

            portfolioList.Portfolio.Add(new PBPortfolio { Name = portfolio });

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_PORTFOLIO_NEW, portfolioList);
        }

        public void CreatePortfolios(IEnumerable<PortfolioVM> portfolios)
        {
            var portfolioList = new PBPortfolioList();

            foreach (var portfolio in portfolios)
            {
                portfolioList.Portfolio.Add(new PBPortfolio { Name = portfolio.Name, HedgeDelay = portfolio.Delay, Threshold = portfolio.Threshold });
            }

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_PORTFOLIO_NEW, portfolioList);
        }

        private void OnQueryPortfolioSuccessAction(PBPortfolioList PB)
        {
            //this.CreatePortfolios();
            PortfolioVMCollection.Clear();
            foreach (var portfolio in PB.Portfolio)
            {
                OnPortfolioUpdated(portfolio);
                OnPortfolioHedgeContracts(portfolio);
            }
        }

        private void OnPortfolioUpdated(PBPortfolio portfolio)
        {
            var port = PortfolioVMCollection.FirstOrDefault(p => p.Name == portfolio.Name);
            if (port == null)
            {
                PortfolioVMCollection.Add(new PortfolioVM(this) { Name = portfolio.Name, Delay = portfolio.HedgeDelay, Threshold = portfolio.Threshold, Hedging = portfolio.Hedging, HedgeVolume = portfolio.HedgeVolume });
            }
            else
            {
                port.Delay = portfolio.HedgeDelay;
                port.Threshold = portfolio.Threshold;
                port.Hedging = portfolio.Hedging;
                port.HedgeVolume = portfolio.HedgeVolume;
            }
        }

        private void OnPortfolioHedgeContracts(PBPortfolio portfolio)
        {
            var port = PortfolioVMCollection.FirstOrDefault(p => p.Name == portfolio.Name);
            if (port != null)
            {
                foreach (var hc in portfolio.HedgeContracts)
                {
                    var hc_current = port.HedgeContractParams.FirstOrDefault(h => hc.Exchange == hc.Exchange && h.Underlying == hc.Underlying);
                    if (hc_current == null)
                    {
                        hc_current = new HedgeVM { Exchange = hc.Exchange, Underlying = hc.Underlying, Contract = hc.Contract };
                        port.HedgeContractParams.Add(hc_current);
                    }
                    else
                    {
                        hc_current.Contract = hc.Contract;
                    }
                }
            }
        }


        protected void OnUpdateStrategySuccessAction(PBStrategy strategy)
        {
            var strategyVM = StrategyVMCollection.FindContract(strategy.Exchange, strategy.Contract);
            if (strategyVM != null)
            {
                strategyVM.Hedging = strategy.Hedging;
                strategyVM.Depth = strategy.Depth;
                strategyVM.BidEnabled = strategy.BidEnabled;
                strategyVM.AskEnabled = strategy.AskEnabled;
                strategyVM.BidQV = strategy.BidQV;
                strategyVM.AskQV = strategy.AskQV;
                strategyVM.MaxAutoTrade = strategy.MaxAutoTrade;
                strategyVM.BidNotCross = strategy.BidNotCross;
                strategyVM.BidCounter = strategy.BidCounter;
                strategyVM.AskCounter = strategy.AskCounter;
                strategyVM.OrderCounter = strategy.LimitOrderCounter;
                strategyVM.CloseMode = strategy.CloseMode;
                strategyVM.TickSize = strategy.TickSizeMult;
                if (strategyVM.AskCounter >= strategyVM.MaxAutoTrade)
                    strategyVM.CounterAskDirection = 1;
                else
                    strategyVM.CounterAskDirection = -1;
                if (strategyVM.BidCounter >= strategyVM.MaxAutoTrade)
                    strategyVM.CounterBidDirection = 1;
                else
                    strategyVM.CounterBidDirection = -1;
                if (strategyVM.OrderCounter >= StrategyVM.MaxLimitOrder && strategyVM.TIF == OrderTIFType.GFD && strategy.VolCond == (int)OrderVolType.ANYVOLUME)
                    strategyVM.OrderCounterDirection = 1;
                else
                    strategyVM.OrderCounterDirection = -1;
                OnStrategyUpdated?.Invoke(strategyVM);
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

        public void UpdateStrategy(StrategyVM sVM, bool resetCounter = false)
        {
            var strategy = new PBStrategy();
            strategy.Exchange = sVM.Exchange;
            strategy.Contract = sVM.Contract;
            strategy.Symbol = sVM.StrategySym;
            strategy.Depth = sVM.Depth;
            strategy.BidQV = sVM.BidQV;
            strategy.AskQV = sVM.AskQV;
            strategy.Hedging = sVM.Hedging;
            strategy.AskEnabled = sVM.AskEnabled;
            strategy.BidEnabled = sVM.BidEnabled;
            strategy.MaxAutoTrade = sVM.MaxAutoTrade;
            strategy.BidNotCross = sVM.BidNotCross;
            strategy.CloseMode = sVM.CloseMode;
            strategy.Tif = (int)sVM.TIF;
            strategy.VolCond = (int)sVM.VolCondition;
            strategy.TickSizeMult = sVM.TickSize;
            if (resetCounter)
            {
                strategy.BidCounter = -1;
                strategy.AskCounter = -1;
            }


            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_MODIFY_STRATEGY, strategy);
        }


        public void UpdatePortfolio(PortfolioVM pVM)
        {
            var portfolio = new PBPortfolio();
            portfolio.Name = pVM.Name;
            portfolio.HedgeDelay = pVM.Delay;
            portfolio.Threshold = pVM.Threshold;
            portfolio.Hedging = pVM.Hedging;
            portfolio.HedgeVolume = pVM.HedgeVolume;

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_MODIFY_PORTFOLIO, portfolio);
        }


        public void UpdateStrategyModel(StrategyVM sVM, StrategyVM.Model model)
        {
            var strategy = new PBStrategy();
            strategy.Exchange = sVM.Exchange;
            strategy.Contract = sVM.Contract;
            strategy.Symbol = sVM.StrategySym;
            strategy.BaseContract = sVM.BaseContract;

            if (model == StrategyVM.Model.PM)
            {
                strategy.PricingModel = sVM.PricingModel;
            }

            if (model == StrategyVM.Model.IVM)
            {
                strategy.IvModel = sVM.IVModel;
            }

            if (model == StrategyVM.Model.VM)
            {
                strategy.VolModel = sVM.VolModel;
            }

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_MODIFY_PRICING_CONTRACT, strategy);
        }

        public void UpdateHedgeContracts(HedgeVM hVM, string PortfolioName)
        {
            var hedge = new PBHedgeContract();
            hedge.Exchange = hVM.Exchange;
            hedge.Contract = hVM.Contract;
            hedge.Underlying = hVM.Underlying;

            var portfolio = new PBPortfolio();
            portfolio.Name = PortfolioName;
            portfolio.HedgeContracts.Add(hedge);


            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_HEDGE_CONTRACT_UPDATE, portfolio);
        }


        public void UpdateStrategyPricingContracts(StrategyVM sVM, StrategyVM.Model model)
        {
            var strategy = new PBStrategy();
            strategy.Exchange = sVM.Exchange;
            strategy.Contract = sVM.Contract;
            strategy.Symbol = sVM.StrategySym;

            if (model == StrategyVM.Model.PM)
            {
                foreach (var pc in sVM.PricingContractParams)
                {
                    strategy.PricingContracts.Add(new PBPricingContract() { Exchange = pc.Exchange, Contract = pc.Contract, Adjust = pc.Adjust, Weight = pc.Weight });
                }
            }

            if (model == StrategyVM.Model.IVM)
            {
                foreach (var pc in sVM.IVMContractParams)
                {
                    strategy.IvmContracts.Add(new PBPricingContract() { Exchange = pc.Exchange, Contract = pc.Contract, Adjust = pc.Adjust, Weight = pc.Weight });
                }
            }

            if (model == StrategyVM.Model.VM)
            {
                foreach (var pc in sVM.VMContractParams)
                {
                    strategy.VolContracts.Add(new PBPricingContract() { Exchange = pc.Exchange, Contract = pc.Contract, Adjust = pc.Adjust, Weight = pc.Weight });
                }
            }

            MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_MODIFY_PRICING_CONTRACT, strategy);
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
        public Task<int> QueryMaxLimitOrderAsync(int timeout = 10000)
        {
            const string LIMITORDERMAX = "LIMITORDER.MAXCOUNT";

            var sst = new StringMap();
            var msgId = (uint)SystemMessageID.MSG_ID_QUERY_SYSPARAMS;
            var tcs = new TaskCompletionSource<int>(new CancellationTokenSource(timeout));

            var serialId = NextSerialId;
            sst.Header = new DataHeader { SerialId = serialId };
            sst.Entry.Add(LIMITORDERMAX, string.Empty);

            MessageWrapper?.RegisterAction<StringMap, ExceptionMessage>
                (msgId,
                (resp) =>
                {
                    if (resp.Header?.SerialId == serialId)
                    {
                        string strMaxOrder;
                        if (resp.Entry.TryGetValue(LIMITORDERMAX, out strMaxOrder))
                        {
                            tcs.SetResult(int.Parse(strMaxOrder));
                        }
                        else
                        {
                            tcs.SetResult(0);
                        }
                    }
                },
                (bizErr) =>
                {
                    OnErrorAction(bizErr);
                    tcs.SetResult(0);
                }
                );

            MessageWrapper?.SendMessage(msgId, sst);

            return tcs.Task;
        }

        public Task<ObservableCollection<StrategyVM>> QueryStrategyAsync(int timeout = 10000)
        {
            var sst = new StringMap();
            var msgId = (uint)BusinessMessageID.MSG_ID_QUERY_STRATEGY;
            var tcs = new TaskCompletionSource<ObservableCollection<StrategyVM>>(new CancellationTokenSource(timeout));

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
                strategyVM.Depth = strategy.Depth;
                strategyVM.AskEnabled = strategy.AskEnabled;
                strategyVM.BidEnabled = strategy.BidEnabled;
                strategyVM.BidQV = strategy.BidQV;
                strategyVM.AskQV = strategy.AskQV;
                strategyVM.IVModel = strategy.IvModel;
                strategyVM.VolModel = strategy.VolModel;
                strategyVM.PricingModel = strategy.PricingModel;
                strategyVM.BaseContract = strategy.BaseContract;
                strategyVM.Portfolio = strategy.Portfolio;
                strategyVM.MaxAutoTrade = strategy.MaxAutoTrade;
                strategyVM.BidNotCross = strategy.BidNotCross;
                strategyVM.AskCounter = strategy.AskCounter;
                strategyVM.BidCounter = strategy.BidCounter;
                strategyVM.CloseMode = strategy.CloseMode;
                strategyVM.TickSize = strategy.TickSizeMult;
                strategyVM.OrderCounter = strategy.LimitOrderCounter;
                strategyVM.PricingContractParams.Clear();

                foreach (var wtContract in strategy.PricingContracts)
                {
                    strategyVM.PricingContractParams.Add(
                        new PricingContractParamVM()
                        {
                            Exchange = wtContract.Exchange,
                            Contract = wtContract.Contract,
                            Adjust = wtContract.Adjust,
                            Weight = wtContract.Weight
                        });
                }

                strategyVM.IVMContractParams.Clear();
                foreach (var wtContract in strategy.IvmContracts)
                {
                    strategyVM.IVMContractParams.Add(
                        new PricingContractParamVM()
                        {
                            Exchange = wtContract.Exchange,
                            Contract = wtContract.Contract,
                            Adjust = wtContract.Adjust,
                            Weight = wtContract.Weight
                        });
                }

                strategyVM.VMContractParams.Clear();
                foreach (var wtContract in strategy.VolContracts)
                {
                    strategyVM.VMContractParams.Add(
                        new PricingContractParamVM()
                        {
                            Exchange = wtContract.Exchange,
                            Contract = wtContract.Contract,
                            Adjust = wtContract.Adjust,
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

        public Task<ObservableCollection<PortfolioVM>> QueryPortfolioAsync(int timeout = 10000)
        {
            var sst = new StringMap();
            var msgId = (uint)BusinessMessageID.MSG_ID_QUERY_PORTFOLIO;
            var tcs = new TaskCompletionSource<ObservableCollection<PortfolioVM>>(new CancellationTokenSource(timeout));

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

        public async Task<ContractKeyVM> SubTradingDeskDataAsync(ContractKeyVM contractKey)
        {
            var tdDataList = await SubTradingDeskDataAsync(new[] { contractKey });
            return tdDataList?.FirstOrDefault();
        }


        public Task<IList<ContractKeyVM>> SubTradingDeskDataAsync(IEnumerable<ContractKeyVM> instrIDList, int timeout = 10000)
        {
            var msgId = (uint)BusinessMessageID.MSG_ID_SUB_TRADINGDESK_PRICING;

            var tcs = new TimeoutTaskCompletionSource<IList<ContractKeyVM>>(timeout);

            var serialId = NextSerialId;

            #region callback
            MessageWrapper.RegisterAction<PBInstrumentList, ExceptionMessage>
            (msgId,
            (resp) =>
            {
                if (resp.Header?.SerialId == serialId)
                {
                    tcs.TrySetResult(SubTDSuccessAction(resp));
                }
            },
            (bizErr) =>
            {
                if (bizErr.SerialId == serialId)
                    tcs.TrySetException(new MessageException(bizErr.MessageId, ErrorType.BIZ_ERROR, bizErr.Errorcode, bizErr.Description.ToStringUtf8()));
            }
            );
            #endregion

            SendMessage(serialId, msgId, instrIDList);

            return tcs.Task;
        }


        public void SendMessage(uint serialId, uint msgId, IEnumerable<ContractKeyVM> instrIDList)
        {
            var instr = new PBInstrumentList();

            foreach (var instrID in instrIDList)
            {
                instr.Instrument.Add(new PBInstrument { Exchange = instrID.Exchange, Contract = instrID.Contract });
            }

            instr.Header = new DataHeader { SerialId = serialId };

            MessageWrapper.SendMessage(msgId, instr);
        }

        public void UnsubTradingDeskData(IEnumerable<ContractKeyVM> instList)
        {
            var instrList = new List<ContractKeyVM>();
            foreach (var quoteVM in instList)
            {
                WeakReference<ContractKeyVM> mktData;
                TradingDeskDataMap.TryRemove(quoteVM, out mktData);
                instrList.Add(quoteVM);
            }

            SendMessage(NextSerialId, (uint)BusinessMessageID.MSG_ID_UNSUB_TRADINGDESK_PRICING, instrList);
        }

        public ContractKeyVM FindTradingDeskData(ContractKeyVM contract)
        {
            WeakReference<ContractKeyVM> tdVMRef;
            ContractKeyVM tdVM = null;
            if (TradingDeskDataMap.TryGetValue(contract, out tdVMRef))
                tdVMRef.TryGetTarget(out tdVM);

            return tdVM;
        }

        protected IList<ContractKeyVM> SubTDSuccessAction(PBInstrumentList instList)
        {
            var ret = new List<ContractKeyVM>();
            foreach (var contract in instList.Instrument)
            {
                var contractKey = new ContractKeyVM(contract.Exchange, contract.Contract);
                var tdVM = FindTradingDeskData(contractKey);
                if (tdVM == null)
                {
                    tdVM = contractKey;
                    TradingDeskDataMap[contractKey] = new WeakReference<ContractKeyVM>(tdVM);
                }

                ret.Add(tdVM);
            }

            return ret;
        }

        protected void UnsubTDSuccessAction(PBInstrumentList pbInstList)
        {
            foreach (var contract in pbInstList.Instrument)
            {
                WeakReference<ContractKeyVM> tdVMRef;
                TradingDeskDataMap.TryRemove(new ContractKeyVM(contract.Exchange, contract.Contract), out tdVMRef);
            }
        }
    }
}
