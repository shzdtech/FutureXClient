using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public class ModelParamsVM : ViewModelBase
    {
        private string _instanceName;
        public string InstanceName
        {
            get
            {
                return _instanceName;
            }
            set
            {
                _instanceName = value;
                OnPropertyChanged("InstanceName");
            }
        }

        private string _model;
        public string Model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged("Model");
            }
        }

        private ObservableCollection<NamedParamVM> _params = new ObservableCollection<NamedParamVM>();
        public ObservableCollection<NamedParamVM> Params
        {
            get
            {
                return _params;
            }
        }


    }
}
