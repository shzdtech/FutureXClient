using Micro.Future.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Micro.Future.ViewModel
{
    public class ContractParamVM : ContractNotifyPropertyChanged
    {
        private double _gamma;
        public double Gamma
        {
            set
            {
                _gamma = value;
                OnPropertyChanged("Gamma");
            }
            get
            {
                return _gamma;
            }
        }

        private int _depthVol;
        public int DepthVol
        {
            set
            {
                _depthVol = value;
                OnPropertyChanged("DepthVol");
            }
            get
            {
                return _depthVol;
            }
        }

        public void UpdateContractParam()
        {
            MessageHandlerContainer.DefaultInstance.Get<AbstractOTCMarketDataHandler>().
                UpdateContractParam(this);
        }

        RelayCommand _updateCPCommand;
        public ICommand UpdateContractParamCommand
        {
            get
            {
                if (_updateCPCommand == null)
                {
                    _updateCPCommand = new RelayCommand(
                            param => UpdateContractParam()
                        );
                }
                return _updateCPCommand;
            }
        }
    }
}
