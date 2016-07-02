using Micro.Future.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Micro.Future.ViewModel
{
    public class OTCQuoteVM : ContractNotifyPropertyChanged
    {
        private int _quantity = 1;
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                OnPropertyChanged("Quantity");
            }
        }


        private DoubleChange _askPrice = new DoubleChange();
        public DoubleChange AskPrice
        {
            get { return _askPrice; }
            set
            {
                _askPrice.Value = value.Value;
                OnPropertyChanged("AskPrice");
            }
        }

        private DoubleChange _bidPrice = new DoubleChange();
        public DoubleChange BidPrice
        {
            get { return _bidPrice; }
            set
            {
                _bidPrice.Value = value.Value;
                OnPropertyChanged("BidPrice");
            }
        }

        private void updateQuantity()
        {
            MessageHandlerContainer.DefaultInstance.Get<AbstractOTCMarketDataHandler>().
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
