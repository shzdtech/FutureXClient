using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public class ParamDefVM : ViewModelBase
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private double _defaultVal;
        public double DefaultVal
        {
            get { return _defaultVal; }
            set
            {
                _defaultVal = value;
                OnPropertyChanged(nameof(DefaultVal));
            }
        }

        private double _minVal;
        public double MinVal
        {
            get { return _minVal; }
            set
            {
                _minVal = value;
                OnPropertyChanged(nameof(MinVal));
            }
        }

        private double _maxVal;
        public double MaxVal
        {
            get { return _maxVal; }
            set
            {
                _maxVal = value;
                OnPropertyChanged(nameof(MaxVal));
            }
        }
        private int _dataType;
        public int DataType
        {
            get { return _dataType; }
            set
            {
                _dataType = value;
                OnPropertyChanged(nameof(DataType));
            }
        }

        private bool _visible;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                OnPropertyChanged(nameof(Visible));
            }
        }

        private bool _enable;
        public bool Enable
        {
            get { return _enable; }
            set
            {
                _enable = value;
                OnPropertyChanged(nameof(Enable));
            }
        }

        private double _step;
        public double Step
        {
            get { return _step; }
            set
            {
                _step = value;
                OnPropertyChanged(nameof(Step));
            }
        }

        private int _digits;
        public int Digits
        {
            get { return _digits; }
            set
            {
                _digits = value;
                OnPropertyChanged(nameof(Digits));
            }
        }
    }
    public class ModelParamDefVM : ViewModelBase
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

        private string _modelname;
        public string ModelName
        {
            get { return _modelname; }
            set
            {
                _modelname = value;
                OnPropertyChanged(nameof(ModelName));
            }
        }
        private ObservableCollection<ParamDefVM> _params = new ObservableCollection<ParamDefVM>();
        public ObservableCollection<ParamDefVM> Params
        {
            get
            {
                return _params;
            }
            set
            {
                _params = value;
                OnPropertyChanged(nameof(Params));
            }
        }
        public ParamDefVM this[string paramName]
        {
            get
            {
                return Params.FirstOrDefault(p => p.Name == paramName);
            }
            set
            {
                var nameValue = Params.FirstOrDefault(p => p.Name == paramName);
                if (nameValue == null)
                {
                    Params.Add(value);
                }
                else
                {
                    nameValue = value;
                }
            }
        }

    }
}
