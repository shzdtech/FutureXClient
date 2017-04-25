using Micro.Future.Message.Business;
using Micro.Future.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public class OTCOptionTradingDeskHandler : AbstractOTCHandler
    {
        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            base.OnMessageWrapperRegistered(messageWrapper);
            MessageWrapper.RegisterAction<PBTradingDeskOptionParams, ExceptionMessage>
                      ((uint)BusinessMessageID.MSG_ID_RTN_TRADINGDESK_PRICING, OnTradingDeskOptionParams, OnErrorAction);
        }

        #region TradingDesk Data

        public ObservableCollection<RiskVM> RiskVMCollection
        {
            get;
        } = new ObservableCollection<RiskVM>();

        public ObservableCollection<PositionVM> PositionVMCollection
        {
            get;
        } = new ObservableCollection<PositionVM>();

        private void OnTradingDeskOptionParams(PBTradingDeskOptionParams tradingDeskOption)
        {
            TradingDeskOptionVM quote = new TradingDeskOptionVM
            {
                Exchange = tradingDeskOption.Exchange,
                Contract = tradingDeskOption.Contract
            };

            if (FindTradingDeskData(quote) != null)
            {
                if (tradingDeskOption.MarketData != null)
                {
                    quote.MarketDataVM = new PricingVM
                    {
                        AskPrice = tradingDeskOption.MarketData.AskPrice,
                        AskSize = tradingDeskOption.MarketData.AskSize,
                        BidPrice = tradingDeskOption.MarketData.BidPrice,
                        BidSize = tradingDeskOption.MarketData.BidSize,
                        MidPrice = (tradingDeskOption.MarketData.BidPrice + tradingDeskOption.MarketData.AskPrice) / 2
                    };
                }

                if (tradingDeskOption.ImpliedVol != null)
                {
                    quote.ImpliedVolVM = new VolatilityVM
                    {
                        AskVol = tradingDeskOption.ImpliedVol.AskVolatility*100,
                        BidVol = tradingDeskOption.ImpliedVol.BidVolatility*100,
                        MidVol = (tradingDeskOption.ImpliedVol.BidVolatility + tradingDeskOption.ImpliedVol.AskVolatility)*50
                    };

                    if (double.IsNaN(quote.ImpliedVolVM.MidVol))
                    {
                        quote.ImpliedVolVM.MidVol = double.IsNaN(quote.ImpliedVolVM.AskVol) ? quote.ImpliedVolVM.BidVol : quote.ImpliedVolVM.AskVol;
                    }
                }

                if (tradingDeskOption.TheoData != null)
                {
                    quote.TheoDataVM = new OptionPricingVM
                    {
                        AskPrice = tradingDeskOption.TheoData.AskPrice,
                        AskSize = tradingDeskOption.TheoData.AskSize,
                        AskVol = tradingDeskOption.TheoData.AskVolatility*100,
                        AskDelta = tradingDeskOption.TheoData.AskDelta,
                        AskGamma = tradingDeskOption.TheoData.AskGamma,
                        AskTheta = tradingDeskOption.TheoData.AskTheta/365,
                        AskVega = tradingDeskOption.TheoData.AskVega/100,
                        BidPrice = tradingDeskOption.TheoData.BidPrice,
                        BidSize = tradingDeskOption.TheoData.BidSize,
                        BidVol = tradingDeskOption.TheoData.BidVolatility*100,
                        BidDelta = tradingDeskOption.TheoData.BidDelta,
                        BidGamma = tradingDeskOption.TheoData.BidGamma,
                        BidTheta = tradingDeskOption.TheoData.BidTheta/365,
                        BidVega = tradingDeskOption.TheoData.BidVega/100,
                        MidVol = (tradingDeskOption.TheoData.BidVolatility + tradingDeskOption.TheoData.AskVolatility)*50,
                        MidPrice = (tradingDeskOption.TheoData.BidPrice + tradingDeskOption.TheoData.AskPrice) / 2,
                    };
                }

                if (tradingDeskOption.WingsReturn != null)
                {
                    quote.WingsReturnVM = new WingsReturnVM
                    {
                        ATMFPrice = tradingDeskOption.WingsReturn.FAtm,
                        RefPrice = tradingDeskOption.WingsReturn.FRef,
                        SyncFPrice = tradingDeskOption.WingsReturn.FSyn,
                        X0 = tradingDeskOption.WingsReturn.X0,
                        X1 = tradingDeskOption.WingsReturn.X1,
                        X2 = tradingDeskOption.WingsReturn.X2,
                        X3 = tradingDeskOption.WingsReturn.X3,
                        SlopeCurr = tradingDeskOption.WingsReturn.SlopeCurr,
                        SlopeCurrOffset = tradingDeskOption.WingsReturn.SlopeCurrOffset,
                        VolCurr = tradingDeskOption.WingsReturn.VolCurr,
                        VolCurrOffset = tradingDeskOption.WingsReturn.VolCurrOffset,
                    };
                }

                if (tradingDeskOption.TheoDataTemp != null)
                {
                    quote.TempTheoDataVM = new OptionPricingVM
                    {
                        AskVol = tradingDeskOption.TheoDataTemp.AskVolatility,
                        BidVol = tradingDeskOption.TheoDataTemp.BidVolatility
                    };
                }

                OnTradingDeskOptionParamsReceived?.Invoke(quote);
            }
            else
            {
                UnsubTradingDeskData(new[] { quote
    });
            }
        }

        public event Action<TradingDeskOptionVM> OnTradingDeskOptionParamsReceived;

        public IList<CallPutTDOptionVM> MakeCallPutTDOptionData(IList<double> strikeList, IList<ContractKeyVM> callList, IList<ContractKeyVM> putList)
        {
            var retList = new List<CallPutTDOptionVM>();

            for (int i = 0; i < callList.Count; i++)
            {
                var callOption = new TradingDeskOptionVM { Exchange = callList[i].Exchange, Contract = callList[i].Contract };
                callOption.InitProperties();
                var putOption = new TradingDeskOptionVM { Exchange = putList[i].Exchange, Contract = putList[i].Contract };
                putOption.InitProperties();
                var callStrategyVM = StrategyVMCollection.FirstOrDefault(s => s.EqualContract(callList[i]));
                var putStrategyVM = StrategyVMCollection.FirstOrDefault(s => s.EqualContract(putList[i]));
                retList.Add(new CallPutTDOptionVM()
                {
                    StrikePrice = strikeList[i],
                    CallOptionVM = callOption,
                    PutOptionVM = putOption,
                    CallStrategyVM = callStrategyVM,
                    PutStrategyVM = putStrategyVM
                });
            }

            return retList;
        }

        #endregion
    }
    public static class OptionVMExtensions
    {
        public static TradingDeskOptionVM Update(this IEnumerable<CallPutTDOptionVM> collection, TradingDeskOptionVM newVM)
        {
            TradingDeskOptionVM quote = null;
            var cp = collection.FirstOrDefault((pb) => string.Compare(pb.PutOptionVM.Contract, newVM.Contract, true) == 0);
            if (cp != null)
            {
                quote = cp.PutOptionVM;
            }
            else
            {
                cp = collection.FirstOrDefault((pb) => string.Compare(pb.CallOptionVM.Contract, newVM.Contract, true) == 0);
                if (cp != null)
                {
                    quote = cp.CallOptionVM;
                }
            }

            if (quote != null)
            {
                if (newVM.MarketDataVM != null)
                {
                    quote.MarketDataVM.AskPrice = newVM.MarketDataVM.AskPrice;
                    quote.MarketDataVM.AskSize = newVM.MarketDataVM.AskSize;
                    quote.MarketDataVM.BidPrice = newVM.MarketDataVM.BidPrice;
                    quote.MarketDataVM.BidSize = newVM.MarketDataVM.BidSize;
                    quote.MarketDataVM.MidPrice = newVM.MarketDataVM.MidPrice;
                }

                if (newVM.ImpliedVolVM != null)
                {
                    quote.ImpliedVolVM.AskVol = newVM.ImpliedVolVM.AskVol;
                    quote.ImpliedVolVM.BidVol = newVM.ImpliedVolVM.BidVol;
                    quote.ImpliedVolVM.MidVol = newVM.ImpliedVolVM.MidVol;
                }

                if (newVM.TheoDataVM != null)
                {
                    quote.TheoDataVM.AskPrice = newVM.TheoDataVM.AskPrice;
                    quote.TheoDataVM.AskSize = newVM.TheoDataVM.AskSize;
                    quote.TheoDataVM.AskVol = newVM.TheoDataVM.AskVol;
                    quote.TheoDataVM.AskDelta = newVM.TheoDataVM.AskDelta;
                    quote.TheoDataVM.AskGamma = newVM.TheoDataVM.AskGamma;
                    quote.TheoDataVM.AskTheta = newVM.TheoDataVM.AskTheta;
                    quote.TheoDataVM.AskVega = newVM.TheoDataVM.AskVega;
                    quote.TheoDataVM.MidVol = newVM.TheoDataVM.MidVol;
                    quote.TheoDataVM.MidPrice = newVM.TheoDataVM.MidPrice;

                    quote.TheoDataVM.BidPrice = newVM.TheoDataVM.BidPrice;
                    quote.TheoDataVM.BidSize = newVM.TheoDataVM.BidSize;
                    quote.TheoDataVM.BidVol = newVM.TheoDataVM.BidVol;
                    quote.TheoDataVM.BidDelta = newVM.TheoDataVM.BidDelta;
                    quote.TheoDataVM.BidGamma = newVM.TheoDataVM.BidGamma;
                    quote.TheoDataVM.BidTheta = newVM.TheoDataVM.BidTheta;
                    quote.TheoDataVM.BidVega = newVM.TheoDataVM.BidVega;
                }
            }

            return quote;
        }
    }
}
