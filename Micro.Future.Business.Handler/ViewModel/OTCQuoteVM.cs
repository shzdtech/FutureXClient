using Micro.Future.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Micro.Future.ViewModel
{
    public class OTCPricingVM : PricingVM
    {
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

        private void updateQuantity()
        {
            MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>().
                UpdateQuantity(Exchange, Contract, BidQV);
        }

        RelayCommand _updateQCommand;
        public ICommand UpdateQCommand
        {
            get
            {
                if (_updateQCommand == null)
                {
                    _updateQCommand = new RelayCommand(
                            param => updateQuantity()
                        );
                }
                return _updateQCommand;
            }
        }
    }
}
