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
        public override string ToString()
        {
            return _instanceName;
        }

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
                OnPropertyChanged(nameof(InstanceName));
            }
        }

        private string _model;
        public string Model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged(nameof(Model));
            }
        }

        private string _modelaim;
        public string ModelAim
        {
            get { return _modelaim; }
            set
            {
                _modelaim = value;
                OnPropertyChanged(nameof(ModelAim));
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

        private Dictionary<string,string> _paramsstring = new Dictionary<string, string>();
        public Dictionary<string, string> ParamsString
        {
            get
            {
                return _paramsstring;
            }
        }
        public NamedParamVM this[string paramName]
        {
            get
            {
               return Params.FirstOrDefault(p => p.Name == paramName);
            }
            set
            {
                var nameValue = Params.FirstOrDefault(p => p.Name == paramName);
                if(nameValue == null)
                {
                    Params.Add(value);
                }
                else
                {
                    nameValue.Value = value.Value;
                }
            }
        }
    }
}
