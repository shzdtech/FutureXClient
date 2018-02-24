using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel 
{
    public class RiskControlVM : ContractKeyVM
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
                return Rho / 100;
            }
        }

        private int _czceSR;
        public int CZCESR
        {
            get { return _czceSR; }
            set
            {
                _czceSR = value;
                OnPropertyChanged("CZCESR");

            }
        }

        private int _czceSR_O;
        public int CZCESR_O
        {
            get { return _czceSR_O; }
            set
            {
                _czceSR_O = value;
                OnPropertyChanged("CZCESR_O");

            }
        }

        private int _dceM;
        public int DCEM
        {
            get { return _dceM; }
            set
            {
                _dceM = value;
                OnPropertyChanged("DCEM");

            }
        }

        private int _dceM_O;
        public int DCEM_O
        {
            get { return _dceM_O; }
            set
            {
                _dceM_O = value;
                OnPropertyChanged("DCEM_O");

            }
        }

        private int _sse510050;
        public int SSE510050
        {
            get { return _sse510050; }
            set
            {
                _sse510050 = value;
                OnPropertyChanged("SSE510050");

            }
        }

        private int _sse510050_O;
        public int SSE510050_O
        {
            get { return _sse510050_O; }
            set
            {
                _sse510050_O = value;
                OnPropertyChanged("SSE510050_O");

            }
        }


    }
}
