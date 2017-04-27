using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel 
{
    public class RiskVM : ContractKeyVM
    {
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

        private double _gamma;
        public double Gamma
        {
            get { return _gamma; }
            set
            {
                _gamma = value;
                OnPropertyChanged("Gamma");
            }
        }
        private double _theta;
        public double Theta
        {
            get { return _theta; }
            set
            {
                _theta = value;
                OnPropertyChanged("Theta");
            }
        }
        private double _position;
        public double Position
        {
            get { return _position; }
            set
            {
                _position = value;
                OnPropertyChanged("Position");
            }
        }
    }
}
