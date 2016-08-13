using Micro.Future.Message;
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
    public class OTCOptionTradingDeskHandler: AbstractOTCMarketDataHandler
    {
        public override void OnMessageWrapperRegistered(AbstractMessageWrapper messageWrapper)
        {
            base.OnMessageWrapperRegistered(messageWrapper);
        }

        public ObservableCollection<NumericalSimVM> NumericalSimVMCollection
        {
            get;
        } = new ObservableCollection<NumericalSimVM>();

        public ObservableCollection<OptionOxyVM> OptionOxyVMCollection
        {
            get;
        } = new ObservableCollection<OptionOxyVM>();

        public void UpdateOptionParam(OptionVM opVM)
        {
            //var opBd = new PBOption();
            //opBd.StrikePriceIncrement = opVM.StrikePriceIncrement;
            //opBd.NumberofStrikePrice = opVM.NumberofStrikePrice;
            //opBd.RiskFreeInterest = opVM.RiskFreeInterest;
            //opBd.DaysMaturity = opVM.DaysMaturity;
            //opBd.TimeWeightingEffect = opVM.TimeWeightingEffect;
            //opBd.LogReturnThreshold = opVM.LogReturnThreshold;
            //opBd.ATMForwardPrice = opVM.ATMForwardPrice;
            //opBd.ReferencePrice = opVM.ReferencePrice;
            //opBd.VolatilityReference = opVM.VolatilityReference;
            //opBd.CurrentVolatility = opVM.CurrentVolatility;
            //opBd.CurrentSlope = opVM.CurrentSlope;
            //opBd.VolatilityChangeRate = opVM.VolatilityChangeRate;
            //opBd.SlopeReference = opVM.SlopeReference;
            //opBd.SlopeChangeRate = opVM.SlopeChangeRate;
            //opBd.PutCurvature = opVM.PutCurvature;
            //opBd.CallCurvature = opVM.CallCurvature;
            //opBd.DownCutoff = opVM.DownCutoff;
            //opBd.UpCutoff = opVM.UpCutoff;
            //opBd.DownSmoothingRange = opVM.DownSmoothingRange;
            //opBd.UpSmoothingRange = opVM.UpSmoothingRange;
            //opBd.DownSlope = opVM.DownSlope;
            //opBd.UpSlope = opVM.UpSlope;
            //opBd.VolatilityReference1 = opVM.VolatilityReference1;
            //opBd.CurrentVolatility1 = opVM.CurrentVolatility1;
            //opBd.CurrentSlope1 = opVM.CurrentSlope1;
            //opBd.VolatilityChangeRate1 = opVM.VolatilityChangeRate1;
            //opBd.SlopeReference1 = opVM.SlopeReference1;
            //opBd.SlopeChangeRate1 = opVM.SlopeChangeRate1;
            //opBd.PutCurvature1 = opVM.PutCurvature1;
            //opBd.CallCurvature1 = opVM.CallCurvature1;
            //opBd.DownCutoff1 = opVM.DownCutoff1;
            //opBd.UpCutoff1 = opVM.UpCutoff1;
            //opBd.DownSmoothingRange1 = opVM.DownSmoothingRange1;
            //opBd.UpSmoothingRange1 = opVM.UpSmoothingRange1;
            //opBd.DownSlope1 = opVM.DownSlope1;
            //opBd.UpSlope1 = opVM.UpSlope1;
            //opBd.SSR = opVM.SSR;
            var opLstBd = new PBContractParamList();
            //cpLstBd.Params.Add(opBd);
            //MessageWrapper.SendMessage((uint)BusinessMessageID.MSG_ID_MODIFY_CONTRACT_PARAM, cpLstBd);
        }

    }
}
