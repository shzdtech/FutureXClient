using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.LocalStorage.DataObject
{
    public class MarketDataDO
    {
        public int Id { get; set; }
        public string Exchange { get; set; }
        public string Contract { get; set; }
        public double AskPrice { get; set; }
        public double BidPrice { get; set; }
        public int AskSize { get; set; }
        public int BidSize { get; set; }
    }
}
