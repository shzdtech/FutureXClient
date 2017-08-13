using System.Collections.Generic;
using System.Linq;
using System.Text;
using Micro.Future.ViewModel;
using Micro.Future.Message.Business;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.LocalStorage;
using Micro.Future.Utility;

namespace Micro.Future.Message
{
    public class BaseMarketDataHandler : AbstractMessageHandler
    {
        protected uint MSG_ID_SUB_MD = (uint)BusinessMessageID.MSG_ID_SUB_MARKETDATA;
        protected uint MSG_ID_UNSUB_MD = (uint)BusinessMessageID.MSG_ID_UNSUB_MARKETDATA;
        protected uint MSG_ID_RET_MD = (uint)BusinessMessageID.MSG_ID_RET_MARKETDATA;

        public const int RETRY_TIMES = 5;

        public event Action<MarketDataVM> OnNewMarketData;


        protected void RaiseNewMD(MarketDataVM md)
        {
            OnNewMarketData?.Invoke(md);
        }

        public ConcurrentDictionary<string, WeakReference<MarketDataVM>> MarketDataMap
        {
            get;
        } = new ConcurrentDictionary<string, WeakReference<MarketDataVM>>();

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            MessageWrapper.RegisterAction<SimpleStringTable, ExceptionMessage>(MSG_ID_UNSUB_MD, UnsubMDSuccessAction, ErrorMsgAction);
            MessageWrapper.RegisterAction<PBMarketData, ExceptionMessage>(MSG_ID_RET_MD, RetMDSuccessAction, ErrorMsgAction);
        }

        public virtual async Task<MarketDataVM> SubMarketDataAsync(string contract, string exchange = null)
        {
            var mktDataList = await SubMarketDataAsync(new[] { new ContractKeyVM(exchange, contract) });
            return mktDataList?.FirstOrDefault();
        }


        public virtual Task<IList<MarketDataVM>> SubMarketDataAsync(IEnumerable<ContractKeyVM> instrIDList, int timeout = 10000)
        {
            var tempList = AddToMarketDataMap(instrIDList);

            var tcs = new TimeoutTaskCompletionSource<IList<MarketDataVM>>(timeout);

            var serialId = NextSerialId;

            #region callback
            MessageWrapper.RegisterAction<PBMarketDataList, ExceptionMessage>
            (MSG_ID_SUB_MD,
            (resp) =>
            {
                if (resp.Header?.SerialId == serialId)
                {
                    tcs.TrySetResult(SubMDSuccessAction(resp));
                }

                tempList.Clear();
            },
            (bizErr) =>
            {
                if (bizErr.SerialId == serialId)
                    tcs.TrySetException(new MessageException(bizErr.MessageId, ErrorType.BIZ_ERROR, bizErr.Errorcode, bizErr.Description.ToStringUtf8()));

                tempList.Clear();
            }
            );
            #endregion

            SendMessage(serialId, MSG_ID_SUB_MD, instrIDList);

            return tcs.Task;
        }


        public virtual void SendMessage(uint serialId, uint msgId, IEnumerable<ContractKeyVM> instrIDList)
        {
            var instr = new NamedStringVector();
            instr.Name = (FieldName.INSTRUMENT_ID);

            foreach (var instrID in instrIDList)
            {
                instr.Entry.Add(instrID.Contract);
            }

            var sst = new SimpleStringTable();
            sst.Header = new DataHeader { SerialId = serialId };
            sst.Columns.Add(instr);

            MessageWrapper.SendMessage(msgId, sst);
        }

        public virtual void UnsubMarketData(IEnumerable<ContractKeyVM> quoteCollection)
        {
            var instrList = new List<ContractKeyVM>();
            foreach (var quoteVM in quoteCollection)
            {
                WeakReference<MarketDataVM> mktData;
                MarketDataMap.TryRemove(quoteVM.Contract, out mktData);
                instrList.Add(quoteVM);
            }

            SendMessage(NextSerialId, MSG_ID_UNSUB_MD, instrList);
        }

        public MarketDataVM FindMarketData(string contract)
        {
            WeakReference<MarketDataVM> mktVMRef;
            MarketDataVM mktVM = null;
            if (MarketDataMap.TryGetValue(contract, out mktVMRef))
                mktVMRef.TryGetTarget(out mktVM);

            return mktVM;
        }


        protected virtual IList<MarketDataVM> AddToMarketDataMap(IEnumerable<ContractKeyVM> subList)
        {
            var ret = new List<MarketDataVM>();
            foreach (var md in subList)
            {
                MarketDataVM mktVM = FindMarketData(md.Contract);
                if (mktVM == null)
                {
                    var contractInfo = ClientDbContext.FindContract(md.Contract);
                        mktVM = new MarketDataVM

                    {
                        Exchange = md.Exchange,
                        Contract = md.Contract,                            
                        Multiple = contractInfo != null ? contractInfo.VolumeMultiple : 1
                        };
                    MarketDataMap[md.Contract] = new WeakReference<MarketDataVM>(mktVM);
                }

                ret.Add(mktVM);
            }

            return ret;
        }

        protected virtual IList<MarketDataVM> SubMDSuccessAction(PBMarketDataList marketList)
        {
            var ret = new List<MarketDataVM>();
            foreach (var md in marketList.MarketData)
            {
                MarketDataVM mktVM = FindMarketData(md.Contract);

                if (mktVM != null)
                {
                    if (mktVM.OpenValue == 0)
                    {
                        UpdateMarketDataVM(mktVM, md);
                    }

                    if (string.IsNullOrEmpty(mktVM.Exchange))
                    {
                        mktVM.Exchange = md.Exchange;
                    }

                    ret.Add(mktVM);
                }
            }

            return ret;
        }

        protected virtual void UnsubMDSuccessAction(SimpleStringTable strTbl)
        {
            foreach (var contract in strTbl.Columns[0].Entry)
            {
                WeakReference<MarketDataVM> mktVMRef;
                MarketDataMap.TryRemove(contract, out mktVMRef);
            }
        }

        protected void UpdateMarketDataVM(MarketDataVM mktVM, PBMarketData md)
        {
            //var contractInfo = ClientDbContext.FindContract(md.Contract);

            mktVM.LastPrice = md.MatchPrice;
            mktVM.BidPrice = md.BidPrice[0];
            mktVM.AskPrice = md.AskPrice[0];
            mktVM.BidSize = md.BidVolume[0];
            mktVM.AskSize = md.AskVolume[0];
            mktVM.Volume = md.Volume;
            mktVM.OpenValue = md.OpenValue;
            mktVM.PreCloseValue = md.PreCloseValue;
            mktVM.HighValue = md.HighValue;
            mktVM.LowValue = md.LowValue;
            mktVM.UpperLimitPrice = md.HighLimit;
            mktVM.LowerLimitPrice = md.LowLimit;
            mktVM.SettlePrice = md.SettlePrice;
            mktVM.PreSettlePrice = md.PreSettlePrice;
            mktVM.AveragePrice = md.AveragePrice;
            mktVM.HighLimint = md.HighLimit;
            mktVM.LowLimint = md.LowLimit;
            mktVM.OpenInterest = md.OpenInterest;
            mktVM.OpenValue = md.OpenValue;
            mktVM.PreOpenInterest = md.PreOpenInterest;
            mktVM.PriceChange = md.PriceChange;
            mktVM.CloseValue = md.CloseValue;
            mktVM.Turnover = md.Turnover;
            mktVM.MidPrice = (mktVM.BidPrice + mktVM.AskPrice) / 2;
            mktVM.AveragePriceMultiplier = mktVM.AveragePrice / mktVM.Multiple;
            mktVM.UpdateTime = string.Format("{0:D2}:{1:D2}:{2:D2}", md.UpdateTime / 3600, (md.UpdateTime / 60) % 60, md.UpdateTime % 60);
        }

        protected virtual void RetMDSuccessAction(PBMarketData md)
        {
            var mktVM = FindMarketData(md.Contract);
            if (mktVM != null)
            {
                UpdateMarketDataVM(mktVM, md);
                RaiseNewMD(mktVM);
            }
            else
            {
                UnsubMarketData(new[] { new ContractKeyVM(md.Exchange, md.Contract) });
            }
        }

        protected virtual void ErrorMsgAction(ExceptionMessage bizErr)
        {
            if (bizErr.Description != null)
            {
                var msg = bizErr.Description.ToByteArray();
                if (msg.Length > 0)
                    RaiseOnError(new MessageException(bizErr.MessageId, ErrorType.UNSPECIFIED_ERROR, bizErr.Errorcode,
                        Encoding.UTF8.GetString(msg)));
            }
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
    }
}
