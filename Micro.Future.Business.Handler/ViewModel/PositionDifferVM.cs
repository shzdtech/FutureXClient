using Micro.Future.Message;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Micro.Future.ViewModel
{
    public class PositionDifferVM : ContractKeyVM
    {        ///持仓多空方向
        private PositionDirectionType _direction;
        public PositionDirectionType Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                OnPropertyChanged("Direction");
            }
        }
        private int _position;
        public int Position
        {
            get { return _position; }
            set
            {
                _position = value;
                OnPropertyChanged("Position");
            }
        }

        private int _dbPosition;
        public int DBPosition
        {
            get { return _dbPosition; }
            set
            {
                _dbPosition = value;
                OnPropertyChanged("DBPosition");
            }
        }

        private int _sysPosition;

        public int SysPosition
        {
            get { return _sysPosition; }
            set
            {
                _sysPosition = value;
                OnPropertyChanged(nameof(SysPosition));
            }
        }
        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                OnPropertyChanged("Selected");
            }
        }
    }
}
