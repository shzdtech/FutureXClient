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
    public class PricingContractParamVM : ContractNotifyPropertyChanged
    {
        private double _weight;
        public double Weight
        {
            get
            {
                return _weight;
            }
            set
            {
                _weight = value;
                OnPropertyChanged("Weight");
            }
        }
    }
    public class StrategyVM : OTCQuoteVM
    {
        public StrategyVM(AbstractOTCHandler otcHandler)
        {
            OTCHandler = otcHandler;
        }

        public AbstractOTCHandler OTCHandler { get; set; }



        private string _underlying;
        public string Underlying
        {
            get
            {
                return _underlying;
            }
            set
            {
                _underlying = value;
                OnPropertyChanged("Underlying");
            }
        }

        private string _strategySym;
        public string StrategySym
        {
            get
            {
                return _strategySym;
            }
            set
            {
                _strategySym = value;
                OnPropertyChanged("StrategySym");
            }
        }

        private string _description;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        private bool _enabled;
        public bool Enabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
                OnPropertyChanged("Enabled");
            }
        }

        private bool _tradingAllowed;
        public bool IsTradingAllowed
        {
            get
            {
                return _tradingAllowed;
            }
            set
            {
                _tradingAllowed = value;
                OnPropertyChanged("IsTradingAllowed");
            }
        }

        private int _depth;
        public int Depth
        {
            get
            {
                return _depth;
            }
            set
            {
                _depth = value;
                OnPropertyChanged("Depth");
            }
        }

        private ObservableCollection<NamedParamVM> _strategyParams = new ObservableCollection<NamedParamVM>();
        public ObservableCollection<NamedParamVM> Params
        {
            get
            {
                return _strategyParams;
            }
        }

        private ObservableCollection<PricingContractParamVM> _pricingContractParams = new ObservableCollection<PricingContractParamVM>();
        public ObservableCollection<PricingContractParamVM> PricingContractParams
        {
            get
            {
                return _pricingContractParams;
            }
        }

        public void UpdateStrategy()
        {
            OTCHandler.UpdateStrategy(this);
        }

        RelayCommand _updateCommand;
        public ICommand UpdateCommand
        {
            get
            {
                if (_updateCommand == null)
                {
                    _updateCommand = new RelayCommand(
                            param => UpdateStrategy()
                        );
                }
                return _updateCommand;
            }
        }
    }
}
