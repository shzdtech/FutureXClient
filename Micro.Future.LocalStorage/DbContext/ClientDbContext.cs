using Micro.Future.LocalStorage.DataObject;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.LocalStorage
{
    public class ClientDbContext : DbContext
    {
        public DbSet<ContractInfo> ContractInfoSet { get; set; }




    }
}
