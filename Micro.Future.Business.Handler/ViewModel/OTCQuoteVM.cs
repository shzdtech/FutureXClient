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
        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
            }
        }


        private void updateQuantity()
        {
            MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>().
                UpdateQuantity(Exchange, Contract, Quantity);
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
