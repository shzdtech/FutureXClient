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


        private double _delta;
        public double Delta
        {
            get
            {
                return _delta;
            }
            set
            {
                _delta = value;
                OnPropertyChanged(nameof(Delta));
            }
        }


        private double _Gamma;
        public double Gamma
        {
            get
            {
                return _Gamma;
            }
            set
            {
                _Gamma = value;
                OnPropertyChanged(nameof(Gamma));
            }
        }


        private double _theta;
        public double Theta
        {
            get
            {
                return _theta;
            }
            set
            {
                _theta = value;
                OnPropertyChanged(nameof(Theta));
                OnPropertyChanged(nameof(Theta365));
            }
        }
        public double Theta365
        {
            get
            {
                return Theta / 365;
            }
        }

        private double _vega;
        public double Vega
        {
            get
            {
                return _vega;
            }
            set
            {
                _vega = value;
                OnPropertyChanged(nameof(Vega));
                OnPropertyChanged(nameof(Vega100));

            }
        }
        public double Vega100
        {
            get
            {
                return Vega / 100;
            }
        }
        private double _rho;
        public double Rho
        {
            get
            {
                return _rho;
            }
            set
            {
                _vega = value;
                OnPropertyChanged(nameof(Rho));
                OnPropertyChanged(nameof(Rho100));

            }
        }
        public double Rho100
        {
            get
            {
                return Rho / 100;
            }
        }
        private int _askdirection;
        public int Askdirection
        {
            get
            {
                return _askdirection;
            }
            set
            {
                _askdirection = value;
                OnPropertyChanged(nameof(Askdirection));
            }
        }
        private int _biddirection;
        public int Biddirection
        {
            get
            {
                return _biddirection;
            }
            set
            {
                _biddirection = value;
                OnPropertyChanged(nameof(Biddirection));
            }
        }
    }
}
