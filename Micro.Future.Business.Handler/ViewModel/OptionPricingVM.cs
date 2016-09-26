using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public class OptionPricingVM : PricingVM
    {
        private double _bidVol = .2;
        public double BidVol
        {
            get
            {
                return _bidVol;
            }
            set
            {
                _bidVol = value;
                OnPropertyChanged(nameof(BidVol));
            }
        }

        private double _askVol = .2;
        public double AskVol
        {
            get
            {
                return _askVol;
            }
            set
            {
                _askVol = value;
                OnPropertyChanged(nameof(AskVol));
            }
        }

        private double _midVol = .2;
        public double MidVol
        {
            get
            {
                return _midVol;
            }
            set
            {
                _midVol = value;
                OnPropertyChanged(nameof(MidVol));
            }
        }

        private double _midPrice;
        public double MidPrice
        {
            get
            {
                return _midPrice;
            }
            set
            {
                _midPrice = value;
                OnPropertyChanged(nameof(MidPrice));
            }
        }

        private double _bidDelta;
        public double BidDelta
        {
            get
            {
                return _bidDelta;
            }
            set
            {
                _bidDelta = value;
                OnPropertyChanged(nameof(BidDelta));
            }
        }

        private double _askDelta;
        public double AskDelta
        {
            get
            {
                return _askDelta;
            }
            set
            {
                _askDelta = value;
                OnPropertyChanged(nameof(AskDelta));
            }
        }

        private double _bidGamma;
        public double BidGamma
        {
            get
            {
                return _bidGamma;
            }
            set
            {
                _bidGamma = value;
                OnPropertyChanged(nameof(BidGamma));
            }
        }

        private double _askGamma;
        public double AskGamma
        {
            get
            {
                return _askGamma;
            }
            set
            {
                _askGamma = value;
                OnPropertyChanged(nameof(AskGamma));
            }
        }

        private double _bidTheta;
        public double BidTheta
        {
            get
            {
                return _bidTheta;
            }
            set
            {
                _bidTheta = value;
                OnPropertyChanged(nameof(BidTheta));
            }
        }

        private double _askTheta;
        public double AskTheta
        {
            get
            {
                return _askTheta;
            }
            set
            {
                _askTheta = value;
                OnPropertyChanged(nameof(AskTheta));
            }
        }

        private double _bidVega;
        public double BidVega
        {
            get
            {
                return _bidVega;
            }
            set
            {
                _bidVega = value;
                OnPropertyChanged(nameof(BidVega));
            }
        }

        private double _askVega;
        public double AskVega
        {
            get
            {
                return _askVega;
            }
            set
            {
                _askVega = value;
                OnPropertyChanged(nameof(AskVega));
            }
        }
    }
}
