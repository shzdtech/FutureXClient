using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
   public class VolatilityVM : ViewModelBase
    {

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

        private double _volBid;
        public double volBid
        {
            get { return _volBid; }
            set
            {
                _volBid = value;
                OnPropertyChanged("volBid");
            }
        }

        private double _volMid;
        public double VolMid
        {
            get { return _volMid; }
            set
            {
                _volMid = value;
                OnPropertyChanged("VolMid");
            }
        }

        private double _volAsk;
        public double VolAsk
        {
            get { return _volAsk; }
            set
            {
                _volAsk = value;
                OnPropertyChanged("VolAsk");
            }
        }
    }
}
