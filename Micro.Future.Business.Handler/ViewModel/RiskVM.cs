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
        public double Vega100
        {
            get
            {
                return Vega / 100;
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
        public double Theta365
        {
            get
            {
                return Theta / 365;
            }
        }
        private double _rho;
        public double Rho
        {
            get { return _rho; }
            set
            {
                _rho = value;
                OnPropertyChanged("Rho");

            }
        }
        public double Rho100
        {
            get
            {
                return Rho100 / 100;
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
        private string _expiration;
        public string Expiration
        {
            get
            {
                return _expiration;
            }
            set
            {
                _expiration = value;
                OnPropertyChanged("Expiration");
            }
        }
        private double _strikeprice;
        public double StrikePrice
        {
            get { return _strikeprice; }
            set
            {
                _strikeprice = value;
                OnPropertyChanged("StrikePrice");
            }
        }
    }
}
