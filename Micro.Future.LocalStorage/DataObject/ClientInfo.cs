using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.LocalStorage.DataObject
{
    public class ClientInfo
    {
        ///Id
        public int Id { get; set; }
        ///客户端版本号
        public string Version { get; set; }
    }
}
