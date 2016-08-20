using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.LocalStorage.DataObject
{
    public class SyncInfo
    {
        [Key]
        public string Item { get; set; }

        public string Version { get; set; }

        public DateTime SyncTime { get; set; }
    }
}
