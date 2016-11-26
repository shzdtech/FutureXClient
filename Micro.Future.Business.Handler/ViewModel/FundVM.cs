namespace Micro.Future.ViewModel
{

    public class FundVM : ContractKeyVM
    {

        private int _eof;
        public int EOF
        {
            get { return _eof; }
            set
            {
                _eof = value;
                OnPropertyChanged("EOF");
            }
        }


        private string _brokerID;

        public string BrokerID
        {
            get { return _brokerID; }
            set
            {
                _brokerID = value;
                OnPropertyChanged("BrokerID");
            }
        }


        private string _accountID;
        public string AccountID
        {
            get { return _accountID; }
            set
            {
                _accountID = value;
                OnPropertyChanged("AccountID");
            }
        }


        private double _preMortgage;
        public double PreMortgage
        {
            get { return _preMortgage; }
            set
            {
                _preMortgage = value;
                OnPropertyChanged("PreMortgage");
            }
        }

        ///开仓金额
        private double _preCredit;
        public double PreCredit
        {
            get { return _preCredit; }
            set
            {
                _preCredit = value;
                OnPropertyChanged("PreCredit");
            }
        }


        private double _preDeposit;
        public double PreDeposit
        {
            get { return _preDeposit; }
            set
            {
                _preDeposit = value;
                OnPropertyChanged("PreDeposit");
            }
        }

        ///
        private double _preBalance;
        public double PreBalance
        {
            get { return _preBalance; }
            set
            {
                _preBalance = value;
                OnPropertyChanged("PreBalance");
            }
        }

        ///
        private double _preMargin;
        public double PreMargin
        {
            get { return _preMargin; }
            set
            {
                _preMargin = value;
                OnPropertyChanged("PreMargin");
            }
        }

        ///
        private double _interestBase;
        public double InterestBase
        {
            get { return _interestBase; }
            set
            {
                _interestBase = value;
                OnPropertyChanged("InterestBase");
            }
        }

        ///
        private double _interest;
        public double Interest
        {
            get { return _interest; }
            set
            {
                _interest = value;
                OnPropertyChanged("Interest");
            }
        }


        ///
        private double _deposit;
        public double Deposit
        {
            get { return _deposit; }
            set
            {
                _deposit = value;
                OnPropertyChanged("Deposit");
            }
        }

        ///
        private double _withdraw;
        public double Withdraw
        {
            get { return _withdraw; }
            set
            {
                _withdraw = value;
                OnPropertyChanged("Withdraw");
            }
        }

        private double _frozenMargin;
        public double FrozenMargin
        {
            get { return _frozenMargin; }
            set
            {
                _frozenMargin = value;
                OnPropertyChanged("FrozenMargin");
            }
        }

        private double _frozenCash;
        public double FrozenCash
        {
            get { return _frozenCash; }
            set
            {
                _frozenCash = value;
                OnPropertyChanged("FrozenCash");
            }
        }

        private double _frozenCommission;
        public double FrozenCommission
        {
            get { return _frozenCommission; }
            set
            {
                _frozenCommission = value;
                OnPropertyChanged("FrozenCommission");
            }
        }

        private double _currMargin;
        public double CurrMargin
        {
            get { return _currMargin; }
            set
            {
                _currMargin = value;
                OnPropertyChanged("CurrMargin");
            }
        }

        private double _cashIn;
        public double CashIn
        {
            get { return _cashIn; }
            set
            {
                _cashIn = value;
                OnPropertyChanged("CashIn");
            }
        }

        private double _commission;
        public double Commission
        {
            get { return _commission; }
            set
            {
                _commission = value;
                OnPropertyChanged("Commission");
            }
        }

        private double _closeProfit;
        public double CloseProfit
        {
            get { return _closeProfit; }
            set
            {
                _closeProfit = value;
                OnPropertyChanged("CloseProfit");
            }
        }

        private double _positionProfit;
        public double PositionProfit
        {
            get { return _positionProfit; }
            set
            {
                _positionProfit = value;
                OnPropertyChanged("PositionProfit");
            }
        }

        private double _balance;
        public double Balance
        {
            get { return _balance; }
            set
            {
                _balance = value;
                OnPropertyChanged("Balance");
            }
        }

        private double _available;
        public double Available
        {
            get { return _available; }
            set
            {
                _available = value;
                OnPropertyChanged("Available");
            }
        }

        private double _withdrawQuota;
        public double WithdrawQuota
        {
            get { return _withdrawQuota; }
            set
            {
                _withdrawQuota = value;
                OnPropertyChanged("WithdrawQuota");
            }
        }

        private double _reserve;
        public double Reserve
        {
            get { return _reserve; }
            set
            {
                _reserve = value;
                OnPropertyChanged("Reserve");
            }
        }

        private int _tradingDay;
        public int TradingDay
        {
            get { return _tradingDay; }
            set
            {
                _tradingDay = value;
                OnPropertyChanged("TradingDay");
            }
        }

        private double _settlementID;
        public double SettlementID
        {
            get { return _settlementID; }
            set
            {
                _settlementID = value;
                OnPropertyChanged("SettlementID");
            }
        }

        private double _credit;
        public double Credit
        {
            get { return _credit; }
            set
            {
                _credit = value;
                OnPropertyChanged("Credit");
            }
        }

        private double _mortgage;
        public double Mortgage
        {
            get { return _mortgage; }
            set
            {
                _mortgage = value;
                OnPropertyChanged("Mortgage");
            }
        }

        private double _exchangeMargin;
        public double ExchangeMargin
        {
            get { return _exchangeMargin; }
            set
            {
                _exchangeMargin = value;
                OnPropertyChanged("ExchangeMargin");
            }
        }

        private double _deliveryMargin;
        public double DeliveryMargin
        {
            get { return _deliveryMargin; }
            set
            {
                _deliveryMargin = value;
                OnPropertyChanged("DeliveryMargin");
            }
        }

        private double _exchangeDeliveryMargin;
        public double ExchangeDeliveryMargin
        {
            get { return _exchangeDeliveryMargin; }
            set
            {
                _exchangeDeliveryMargin = value;
                OnPropertyChanged("ExchangeDeliveryMargin");
            }
        }

        private double _reserveBalance;
        public double ReserveBalance
        {
            get { return _reserveBalance; }
            set
            {
                _reserveBalance = value;
                OnPropertyChanged("ReserveBalance");
            }
        }
    }
}
