using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel 
{
    public class RiskVM : ViewModelBase
    {

        private double _value;
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }

        private double _delta;
        public double Delta
        {
            get { return _delta; }
            set
            {
                _delta = value;
                OnPropertyChanged("Delta");
            }
        }

        private double _vega;
        public double Vega
        {
            get { return _vega; }
            set
            {
                _vega = value;
                OnPropertyChanged("Vega");
            }
        }

        private double _positionDelta;
        public double PositionDelta
        {
            get { return _positionDelta; }
            set
            {
                _positionDelta = value;
                OnPropertyChanged("PositionDelta");
            }
        }

        private double _positionVega;
        public double PositionVega
        {
            get { return _positionVega; }
            set
            {
                _positionVega = value;
                OnPropertyChanged("PositionVega");
            }
        }
    }
}
