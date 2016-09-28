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
        private int _bidQT = 1;
        public int BidQT
        {
            get { return _bidQT; }
            set
            {
                _bidQT = value;
                OnPropertyChanged(nameof(BidQT));
            }
        }

        private int _askQT = 1;
        public int AskQT
        {
            get { return _askQT; }
            set
            {
                _askQT = value;
                OnPropertyChanged(nameof(AskQT));
            }
        }

        private void updateQuantity()
        {
            MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>().
                UpdateQuantity(Exchange, Contract, BidQT);
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
