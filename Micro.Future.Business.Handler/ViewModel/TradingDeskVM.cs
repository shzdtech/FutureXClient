using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public class TradingDeskVM : ViewModelBase
    {
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
                OnPropertyChanged("Name");
            }
        }

        public string _contractNum;
        public string ContactNum
        {
            get
            {
                return _contractNum;
            }
            set
            {
                _contractNum = value;
                OnPropertyChanged("ContactNum");
            }
        }

        public string _email;
        public string Email
        {
            get
            {
                return _email;
            }
            set
            {
                _email = value;
                OnPropertyChanged("Email");
            }
        }
    }
}
