using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Micro.Future.Utility;
using System.Windows.Threading;
using Micro.Future.ViewModel;
using Micro.Future.Message;
using Micro.Future.Message.Business;
using System.Collections.ObjectModel;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;

namespace Micro.Future.Business
{
    class LocalOperationHandler : MessageHandlerTemplate<LocalOperationHandler>
    {

        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {

            MessageWrapper.RegisterAction<PBMsgTrader.PBMsgQueryRspInstrumentInfo, BizErrorMsg>
                ((uint)BusinessMessageID.MSG_ID_QUERY_INSTRUMENT, OnContractInfo, null);
        }

        private void OnContractInfo(PBContractInfoList rsp)
        {
            using (var clientDBCtx = new ClientDbContext())
            {
                foreach (var contract in rsp.ContractInfo)
                {
                    clientDBCtx.ContractInfoSet.Add(new ContractInfo()
                    {
                        Id = contract.Id;
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
                        MaxMarginSideAlgorithm = contract.MaxMarginSideAlgorithm


                    });

            }
            clientDBCtx.SaveChanges();
        }
    }





}
}
