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


                if (tradingDeskOption.ImpliedVol != null)
                {
                    quote.ImpliedVolVM = new VolatilityVM
                    {
                        AskVol = tradingDeskOption.ImpliedVol.AskVolatility,
                        BidVol = tradingDeskOption.ImpliedVol.BidVolatility,
                        MidVol = (tradingDeskOption.ImpliedVol.BidVolatility + tradingDeskOption.ImpliedVol.AskVolatility) / 2
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
                        AskVol = tradingDeskOption.TheoData.AskVolatility,
                        Delta = tradingDeskOption.TheoData.Delta,
                        Gamma = tradingDeskOption.TheoData.Gamma,
                        Theta = tradingDeskOption.TheoData.Theta,
                        Vega = tradingDeskOption.TheoData.Vega,
                        Rho = tradingDeskOption.TheoData.Rho,
                        BidPrice = tradingDeskOption.TheoData.BidPrice,
                        BidSize = tradingDeskOption.TheoData.BidSize,
                        BidVol = tradingDeskOption.TheoData.BidVolatility,
                        MidVol = tradingDeskOption.TheoData.MidVolatility,
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
                        BidVol = tradingDeskOption.TheoDataTemp.BidVolatility,
                        MidVol = tradingDeskOption.TheoDataTemp.MidVolatility,
                    };
                }
                if (tradingDeskOption.TheoData.BidPrice >= tradingDeskOption.MarketData?.AskPrice)
                    quote.TheoDataVM.Biddirection = 2;
                else
                {
                    if (tradingDeskOption.TheoData.BidPrice >= tradingDeskOption.MarketData?.BidPrice)
                        quote.TheoDataVM.Biddirection = 1;
                    else if (tradingDeskOption.TheoData.BidPrice < tradingDeskOption.MarketData?.AskPrice)
                        quote.TheoDataVM.Biddirection = -1;
                    else if (tradingDeskOption.TheoData.BidPrice < tradingDeskOption.MarketData?.AskPrice)
                        quote.TheoDataVM.Biddirection = -2;
                }

                if (tradingDeskOption.TheoData.AskPrice <= tradingDeskOption.MarketData?.BidPrice)
                    quote.TheoDataVM.Askdirection = 2;
                else
                {
                    if (tradingDeskOption.TheoData.AskPrice <= tradingDeskOption.MarketData?.AskPrice)
                        quote.TheoDataVM.Askdirection = 1;
                    else if (tradingDeskOption.TheoData.AskPrice > tradingDeskOption.MarketData?.AskPrice)
                        quote.TheoDataVM.Askdirection = -1;
                    else if (tradingDeskOption.TheoData.AskPrice > tradingDeskOption.MarketData?.BidPrice)
                        quote.TheoDataVM.Askdirection = -2;
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

        public IList<CallPutTDOptionVM> MakeCallPutTDOptionData(IList<double> strikeList, IList<ContractKeyVM> callList, IList<ContractKeyVM> putList, IList<MarketDataVM> marketDataList = null)
        {
            var retList = new List<CallPutTDOptionVM>();

            for (int i = 0; i < callList.Count; i++)
            {
                var callOption = new TradingDeskOptionVM { Exchange = callList[i].Exchange, Contract = callList[i].Contract };
                var putOption = new TradingDeskOptionVM { Exchange = putList[i].Exchange, Contract = putList[i].Contract };
                if (marketDataList != null)
                {
                    callOption.MarketDataVM = marketDataList.FirstOrDefault(c => c.Contract == callList[i].Contract);
                    putOption.MarketDataVM = marketDataList.FirstOrDefault(c => c.Contract == putList[i].Contract);
                }
                callOption.InitProperties();
                putOption.InitProperties();

                var callStrategyVM = StrategyVMCollection.FirstOrDefault(s => s.EqualContract(callList[i]));
                if (callStrategyVM != null)
                    callStrategyVM.Depth = 1;
                var putStrategyVM = StrategyVMCollection.FirstOrDefault(s => s.EqualContract(putList[i]));
                if (putStrategyVM != null)
                    putStrategyVM.Depth = 1;
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
                    quote.TheoDataVM.Delta = newVM.TheoDataVM.Delta;
                    quote.TheoDataVM.Gamma = newVM.TheoDataVM.Gamma;
                    quote.TheoDataVM.Theta = newVM.TheoDataVM.Theta;
                    quote.TheoDataVM.Vega = newVM.TheoDataVM.Vega;
                    quote.TheoDataVM.Rho = newVM.TheoDataVM.Rho;
                    quote.TheoDataVM.MidVol = newVM.TheoDataVM.MidVol;
                    quote.TheoDataVM.MidPrice = newVM.TheoDataVM.MidPrice;
                    quote.TheoDataVM.BidPrice = newVM.TheoDataVM.BidPrice;
                    quote.TheoDataVM.BidSize = newVM.TheoDataVM.BidSize;
                    quote.TheoDataVM.BidVol = newVM.TheoDataVM.BidVol;
                    quote.TheoDataVM.Askdirection = newVM.TheoDataVM.Askdirection;
                    quote.TheoDataVM.Biddirection = newVM.TheoDataVM.Biddirection;

                }

                //if (quote.TheoDataVM.BidPrice >= quote.MarketDataVM?.AskPrice)
                //    quote.TheoDataVM.Biddirection = 2;
                //else
                //{
                //    if (quote.TheoDataVM.BidPrice >= quote.MarketDataVM?.BidPrice)
                //        quote.TheoDataVM.Biddirection = 1;
                //    else if (quote.TheoDataVM.BidPrice < quote.MarketDataVM?.AskPrice)
                //        quote.TheoDataVM.Biddirection = -1;
                //    else if (quote.TheoDataVM.BidPrice < quote.MarketDataVM?.AskPrice)
                //        quote.TheoDataVM.Biddirection = -2;
                //}

                //if (quote.TheoDataVM.AskPrice <= quote.MarketDataVM?.BidPrice)
                //    quote.TheoDataVM.Askdirection = 2;
                //else
                //{
                //    if (quote.TheoDataVM.AskPrice <= quote.MarketDataVM?.AskPrice)
                //        quote.TheoDataVM.Askdirection = 1;
                //    else if (quote.TheoDataVM.AskPrice > quote.MarketDataVM?.AskPrice)
                //        quote.TheoDataVM.Askdirection = -1;
                //    else if (quote.TheoDataVM.AskPrice > quote.MarketDataVM?.BidPrice)
                //        quote.TheoDataVM.Askdirection = -2;
                //}

            }
            return quote;
        }
        public static TradingDeskOptionVM UpdatePosition(this IEnumerable<CallPutTDOptionVM> collection, PositionVM newVM)
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
                if (newVM.Direction == PositionDirectionType.PD_LONG)
                    quote.LongPosition = newVM.Position;
                else if (newVM.Direction == PositionDirectionType.PD_SHORT)
                    quote.ShortPosition = newVM.Position;
                quote.Position = quote.LongPosition - quote.ShortPosition;
            }
            if (cp != null)
            {
                cp.TotalPosition = 1;
                cp.MixFuture = 1;
            }
            return quote;
        }
    }
}
