using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public interface IContractKey
    {
        string Exchange
        {
            get;
            set;
        }

        string Contract
        {
            get;
            set;
        }

        bool EqualContract(IContractKey contractKey);

        bool EqualContract(string exchange, string contract);
    }
    public class ContractNotifyPropertyChanged : ViewModelBase, IContractKey
    {
        private string _exchange;
        public string Exchange
        {
            get
            {
                return _exchange;
            }
            set
            {
                _exchange = value;
                OnPropertyChanged("Exchange");
            }
        }

        private string _contract;
        public string Contract
        {
            get
            {
                return _contract;
            }
            set
            {
                _contract = value;
                OnPropertyChanged("Contract");
            }
        }


        public bool EqualContract(IContractKey contractKey)
        {
            return EqualContract(contractKey.Exchange, contractKey.Contract);
        }


        public bool EqualContract(string exchange, string contract)
        {
            return string.Compare(_contract, contract, true) == 0 &&
                string.Compare(_exchange, exchange, true) == 0;
        }
    }
}
