using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public class PriceGreekVM : ViewModelBase
    {
        private double _pVega;
        public double pVega
        {
            get { return _pVega; }
            set
            {
                _pVega = value;
                OnPropertyChanged("pVega");
            }
        }

        private double _pDelta;
        public double pDelta
        {
            get { return _pDelta; }
            set
            {
                _pDelta = value;
                OnPropertyChanged("pDelta");
            }
        }

        private double _pBid;
        public double pBid
        {
            get { return _pBid; }
            set
            {
                _pBid = value;
                OnPropertyChanged("pBid");
            }
        }

        private double _pMid;
        public double pMid
        {
            get { return _pMid; }
            set
            {
                _pMid = value;
                OnPropertyChanged("pMid");
            }
        }

        private double _pAsk;
        public double pAsk
        {
            get { return _pAsk; }
            set
            {
                _pAsk = value;
                OnPropertyChanged("pAsk");
            }
        }

        private double _Strike;
        public double Strike
        {
            get { return _Strike; }
            set
            {
                _Strike = value;
                OnPropertyChanged("Strike");
            }
        }

        private double _cBid;
        public double cBid
        {
            get { return _cBid; }
            set
            {
                _cBid = value;
                OnPropertyChanged("cBid");
            }
        }

        private double _cMid;
        public double cMid
        {
            get { return _cMid; }
            set
            {
                _cMid = value;
                OnPropertyChanged("cMid");
            }
        }

        private double _cAsk;
        public double cAsk
        {
            get { return _cAsk; }
            set
            {
                _cAsk = value;
                OnPropertyChanged("cAsk");
            }
        }

        private double _cDelta;
        public double cDelta
        {
            get { return _cDelta; }
            set
            {
                _cDelta = value;
                OnPropertyChanged("cDelta");
            }
        }

        private double _cVega;
        public double cVega
        {
            get { return _cVega; }
            set
            {
                _cVega = value;
                OnPropertyChanged("cVega");
            }
        }

    }
}
