using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public class WingsReturnVM : ViewModelBase
    {
        private double _volCurr;
        public double VolCurr
        {
            get
            {
                return _volCurr;
            }
            set
            {
                _volCurr = value;
                OnPropertyChanged(nameof(VolCurr));
            }
        }
        private double _volCurrOffset;
        public double VolCurrOffset
        {
            get
            {
                return _volCurrOffset;
            }
            set
            {
                _volCurrOffset = value;
                OnPropertyChanged(nameof(VolCurrOffset));
            }
        }
        private double _slopeCurr;
        public double SlopeCurr
        {
            get
            {
                return _slopeCurr;
            }
            set
            {
                _slopeCurr = value;
                OnPropertyChanged(nameof(SlopeCurr));
            }
        }
        private double _slopeCurrOffset;
        public double SlopeCurrOffset
        {
            get
            {
                return _slopeCurrOffset;
            }
            set
            {
                _slopeCurrOffset = value;
                OnPropertyChanged(nameof(SlopeCurrOffset));
            }
        }
        private double _refPrice;
        public double RefPrice
        {
            get
            {
                return _refPrice;
            }
            set
            {
                _refPrice = value;
                OnPropertyChanged(nameof(RefPrice));
            }
        }
        private double _ATMFPrice;
        public double ATMFPrice
        {
            get
            {
                return _ATMFPrice;
            }
            set
            {
                _ATMFPrice = value;
                OnPropertyChanged(nameof(ATMFPrice));
            }
        }
        private double _syncFPrice;
        public double SyncFPrice
        {
            get
            {
                return _syncFPrice;
            }
            set
            {
                _syncFPrice = value;
                OnPropertyChanged(nameof(SyncFPrice));
            }
        }
        private double _x0;
        public double X0
        {
            get
            {
                return _x0;
            }
            set
            {
                _x0 = value;
                OnPropertyChanged(nameof(X0));
            }
        }
        private double _x1;
        public double X1
        {
            get
            {
                return _x1;
            }
            set
            {
                _x1 = value;
                OnPropertyChanged(nameof(X1));
            }
        }
        private double _x2;
        public double X2
        {
            get
            {
                return _x2;
            }
            set
            {
                _x2 = value;
                OnPropertyChanged(nameof(X2));
            }
        }
        private double _x3;
        public double X3
        {
            get
            {
                return _x3;
            }
            set
            {
                _x3 = value;
                OnPropertyChanged(nameof(X3));
            }
        }

    }
}
