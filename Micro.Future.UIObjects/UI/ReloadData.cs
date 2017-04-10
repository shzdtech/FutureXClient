using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public interface IReloadData
    {
        void Initialize();
        void ReloadData();
        string PersistanceId { get; set; }
    }
}
