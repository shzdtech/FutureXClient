using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Micro.Future.LocalStorage.DataObject
{
    public class FilterSettings
    {
        ///交易所代码
        public string Exchange { get; set; }
        ///合约代码
        public string Contract { get; set; }

        public string Underlying { get; set; }

        public string Title { get; set; }

        public string UserID { get; set; }

        public int Id { get; set; }
    }
}
