using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Micro.Future.LocalStorage.DataObject
{
    [Table("UserSettingInfo")]
    public class UserSettingInfo
    {
        public int Id { get; set; }
    }
}
