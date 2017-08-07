using Micro.Future.Message;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
        private int _hedgeVolume;
        public int HedgeVolume
        {
            get { return _hedgeVolume; }
            set
            {
                _hedgeVolume = value;
                OnPropertyChanged(nameof(HedgeVolume));
            }
        }
        private bool _hedging;
        public bool Hedging
        {
            get { return _hedging; }
            set
            {
                _hedging = value;
                OnPropertyChanged(nameof(Hedging));
            }
        }
        public ObservableCollection<HedgeVM> HedgeContractParams
        {
            get;
        } = new ObservableCollection<HedgeVM>();
        public void UpdatePortfolio()
        {
            OTCHandler.UpdatePortfolio(this);
        }

        RelayCommand _updateCommand;
        public ICommand UpdateCommand
        {
            get
            {
                if (_updateCommand == null)
                {
                    _updateCommand = new RelayCommand(
                            param => UpdatePortfolio()
                        );
                }
                return _updateCommand;
            }
        }
    }
}
