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
                OnPropertyChanged(nameof(Weight));
            }
        }
    }
    public class StrategyVM : OTCPricingVM
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
                OnPropertyChanged(nameof(Underlying));
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
                OnPropertyChanged(nameof(StrategySym));
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
                OnPropertyChanged(nameof(Description));
            }
        }

        private bool _bidEnabled;
        public bool BidEnabled
        {
            get
            {
                return _bidEnabled;
            }
            set
            {
                _bidEnabled = value;
                OnPropertyChanged(nameof(BidEnabled));
            }
        }

        private bool _askEnabled;
        public bool AskEnabled
        {
            get
            {
                return _askEnabled;
            }
            set
            {
                _askEnabled = value;
                OnPropertyChanged(nameof(AskEnabled));
            }
        }

        private bool _hedgeing;
        public bool Hedging
        {
            get
            {
                return _hedgeing;
            }
            set
            {
                _hedgeing = value;
                OnPropertyChanged(nameof(Hedging));
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
                OnPropertyChanged(nameof(Depth));
            }
        }

        private string _pricingModel;
        public string PricingModel
        {
            get { return _pricingModel; }
            set
            {
                _pricingModel = value;
                OnPropertyChanged(nameof(PricingModel));
            }
        }

        private string _ivModel;
        public string IVModel
        {
            get { return _ivModel; }
            set
            {
                _ivModel = value;
                OnPropertyChanged(nameof(IVModel));
            }
        }

        private string _volModel;
        public string VolModel
        {
            get { return _volModel; }
            set
            {
                _volModel = value;
                OnPropertyChanged(nameof(VolModel));
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
