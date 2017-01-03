using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.LocalStorage.DataObject
{
    public class OrderStatusFilter
    {
        public int Orderstatus { get; set; }
        public string AccountID { get; set; }
        public string TabID { get; set; }
    }
}
