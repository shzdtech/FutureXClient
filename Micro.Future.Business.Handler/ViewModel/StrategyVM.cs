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
    public class PricingContractParamVM : ContractKeyVM
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

        private double _adjust;
        public double Adjust
        {
            get
            {
                return _adjust;
            }
            set
            {
                _adjust = value;
                OnPropertyChanged(nameof(Adjust));
            }
        }
    }

    public class StrategyVM : OTCPricingVM
    {
        public enum Model
        {
            PM,
            IVM,
            VM
        }

        public StrategyVM(AbstractOTCHandler otcHandler)
        {
            OTCHandler = otcHandler;
        }

        public AbstractOTCHandler OTCHandler { get; set; }



        private string _basecontract;
        public string BaseContract
        {
            get
            {
                return _basecontract;
            }
            set
            {
                _basecontract = value;
                OnPropertyChanged(nameof(BaseContract));
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

        private int _bidQV = 1;
        public int BidQV
        {
            get { return _bidQV; }
            set
            {
                _bidQV = value;
                OnPropertyChanged(nameof(BidQV));
            }
        }

        private int _askQV = 1;
        public int AskQV
        {
            get { return _askQV; }
            set
            {
                _askQV = value;
                OnPropertyChanged(nameof(AskQV));
            }
        }


        private int _maxAutoTrade;
        public int MaxAutoTrade
        {
            get { return _maxAutoTrade; }
            set
            {
                _maxAutoTrade = value;
                OnPropertyChanged(nameof(MaxAutoTrade));
            }
        }

        private int _bidCounter;
        public int BidCounter
        {
            get { return _bidCounter; }
            set
            {
                _bidCounter = value;
                OnPropertyChanged(nameof(BidCounter));
            }
        }

        private int _askCounter;
        public int AskCounter
        {
            get { return _askCounter; }
            set
            {
                _askCounter = value;
                OnPropertyChanged(nameof(AskCounter));
            }
        }

        private int _counterDirection;
        public int CounterDirection
        {
            get { return _counterDirection; }
            set
            {
                _counterDirection = value;
                OnPropertyChanged(nameof(CounterDirection));
            }
        }
        private int _counterAskDirection;
        public int CounterAskDirection
        {
            get { return _counterAskDirection; }
            set
            {
                _counterAskDirection = value;
                OnPropertyChanged(nameof(CounterAskDirection));
            }
        }
        private int _counterBidDirection;
        public int CounterBidDirection
        {
            get { return _counterBidDirection; }
            set
            {
                _counterBidDirection = value;
                OnPropertyChanged(nameof(CounterBidDirection));
            }
        }

        bool _bidNotCross;
        public bool BidNotCross
        {
            get { return _bidNotCross; }
            set
            {
                _bidNotCross = value;
                OnPropertyChanged(nameof(BidNotCross));
            }
        }
        private bool _closeMode;
        public bool CloseMode
        {
            get
            {
                return _closeMode;
            }
            set
            {
                _closeMode = value;
                OnPropertyChanged(nameof(CloseMode));
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

        public ObservableCollection<PricingContractParamVM> PricingContractParams
        {
            get;
        } = new ObservableCollection<PricingContractParamVM>();

        public ObservableCollection<PricingContractParamVM> IVMContractParams
        {
            get;
        } = new ObservableCollection<PricingContractParamVM>();

        public ObservableCollection<PricingContractParamVM> VMContractParams
        {
            get;
        } = new ObservableCollection<PricingContractParamVM>();

        public void UpdateStrategy(bool resetCounter = false)
        {
            OTCHandler.UpdateStrategy(this, resetCounter);
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
