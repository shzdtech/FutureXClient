using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
   public class VolatilityVM : ViewModelBase
    {
        private double _bidVol;
        public double BidVol
        {
            get { return _bidVol; }
            set
            {
                _bidVol = value;
                OnPropertyChanged(nameof(BidVol));
            }
        }

        private double _midVol;
        public double MidVol
        {
            get { return _midVol; }
            set
            {
                _midVol = value;
                OnPropertyChanged(nameof(MidVol));
            }
        }

        private double _askVol;
        public double AskVol
        {
            get { return _askVol; }
            set
            {
                _askVol = value;
                OnPropertyChanged(nameof(AskVol));
            }
        }
    }
}
