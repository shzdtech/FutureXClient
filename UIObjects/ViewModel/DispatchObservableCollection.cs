using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Micro.Future.Message
{
    public class DispatchObservableCollection<T> : ObservableCollection<T>
    {
        public DispatchObservableCollection()
        {
        }

        public DispatchObservableCollection(Dispatcher dispatcher)
        {
            this.Dispatcher = dispatcher;
        }

        public DispatchObservableCollection(DispatcherObject dispatcherObj)
        {
            this.Dispatcher = dispatcherObj.Dispatcher;
        }

        public Dispatcher Dispatcher
        {
            get;
            private set;
        }
    }
}
