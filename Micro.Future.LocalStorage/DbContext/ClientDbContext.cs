using Micro.Future.LocalStorage.DataObject;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.LocalStorage
{
    class ClientDbContext : DbContext
    {
        public DbSet<InstrumentInfo> InstrumentInfoSet { get; set; }
    }
}
