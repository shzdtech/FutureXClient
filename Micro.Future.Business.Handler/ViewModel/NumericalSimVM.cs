using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public class NumericalSimVM : ViewModelBase
    {


        private double _strikePriceIncrement;
        public double StrikePriceIncrement
        {
            get { return _strikePriceIncrement; }
            set
            {
                _strikePriceIncrement = value;
                OnPropertyChanged("StrikePriceIncrement");
            }
        }

        private double _numberofStrikePrice;
        public double NumberofStrikePrice
        {
            get { return _numberofStrikePrice; }
            set
            {
                _numberofStrikePrice = value;
                OnPropertyChanged("NumberofStrikePrice");
            }
        }


        private double _riskFreeInterest;

        public double RiskFreeInterest
        {
            get { return _riskFreeInterest; }
            set
            {
                _riskFreeInterest = value;
                OnPropertyChanged("RiskFreeInterest");
            }
        }

        private double _daysMaturity;

        public double DaysMaturity
        {
            get { return _daysMaturity; }
            set
            {
                _daysMaturity = value;
                OnPropertyChanged("DaysMaturity");
            }
        }

        private double _timeWeightingEffect;

        public double TimeWeightingEffect
        {
            get { return _timeWeightingEffect; }
            set
            {
                _timeWeightingEffect = value;
                OnPropertyChanged("TimeWeightingEffect");
            }
        }

        private double _logReturnThreshold;

        public double LogReturnThreshold
        {
            get { return _logReturnThreshold; }
            set
            {
                _logReturnThreshold = value;
                OnPropertyChanged("LogReturnThreshold");
            }
        }

        private double _aTMForwardPrice;

        public double ATMForwardPrice
        {
            get { return _aTMForwardPrice; }
            set
            {
                _aTMForwardPrice = value;
                OnPropertyChanged("ATMForwardPrice");
            }
        }

        private double _referencePrice;

        public double ReferencePrice
        {
            get { return _referencePrice; }
            set
            {
                _referencePrice = value;
                OnPropertyChanged("ReferencePrice");
            }
        }

        private double _nSims;

        public double nSims
        {
            get { return _nSims; }
            set
            {
                _nSims = value;
                OnPropertyChanged("nSims");
            }
        }

        private double _miniSteps;

        public double miniSteps
        {
            get { return _miniSteps; }
            set
            {
                _miniSteps = value;
                OnPropertyChanged("miniSteps");
            }
        }

        private double _avgRuns;

        public double avgRuns
        {
            get { return _avgRuns; }
            set
            {
                _avgRuns = value;
                OnPropertyChanged("avgRuns");
            }
        }

        private double _ITM;

        public double ITM
        {
            get { return _ITM; }
            set
            {
                _ITM = value;
                OnPropertyChanged("ITM");
            }
        }

        private double _OTM;

        public double OTM
        {
            get { return _OTM; }
            set
            {
                _OTM = value;
                OnPropertyChanged("OTM");
            }
        }

        private double _Pct;

        public double Pct
        {
            get { return _Pct; }
            set
            {
                _Pct = value;
                OnPropertyChanged("Pct");
            }
        }

        private double _sSR;

        public double SSR
        {
            get { return _sSR; }
            set
            {
                _sSR = value;
                OnPropertyChanged("SSR");
            }
        }

        private double _expirationMonth;

        public double ExpirationMonth
        {
            get { return _expirationMonth; }
            set
            {
                _expirationMonth = value;
                OnPropertyChanged("ExpirationMonth");
            }
        }

        private string _axis;
        public string Axis
        {
            get { return _axis; }
            set
            {
                _axis = value;
                OnPropertyChanged("Axis");
            }
        }
    }
}
