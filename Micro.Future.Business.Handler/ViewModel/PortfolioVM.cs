using Micro.Future.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public class PortfolioVM : ViewModelBase
    {
        public PortfolioVM(AbstractOTCHandler otcHandler)
        {
            OTCHandler = otcHandler;
        }

        public AbstractOTCHandler OTCHandler { get; set; }
        public override string ToString()
        {
            return _name;
        }
        private string _name;
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        private int _delay;
        public int Delay
        {
            get { return _delay; }
            set
            {
                _delay = value;
                OnPropertyChanged(nameof(Delay));
            }
        }
        private double _threshold;
        public double Threshold
        {
            get { return _threshold; }
            set
            {
                _threshold = value;
                OnPropertyChanged(nameof(Threshold));
            }
        }
        public void UpdatePortfolio()
        {
            OTCHandler.UpdatePortfolio(this);
        }
    }
}
