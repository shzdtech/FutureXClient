using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.LocalStorage.DataObject
{
    class PersonalContract
    {
        ///Id
        public int UserID { get; set; }
        ///交易所代码
        public string Exchange { get; set; }
        ///合约代码
        public string Contract { get; set; }
    }
}
