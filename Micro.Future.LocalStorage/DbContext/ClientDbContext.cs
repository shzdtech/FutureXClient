using Micro.Future.LocalStorage.DataObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.LocalStorage
{
    public class ClientDbContext : DbContext
    {
        public ClientDbContext() : base() { }

        public DbSet<ContractInfo> ContractInfoSet { get; set; }

        public DbSet<UserSettingInfo> UserSettingInfoSet { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("DataSource=C:/Users/huzha/WorkSpace/FutureXClient/Micro.Future.LocalStorage/Data/clientcache.db");
        }

        
    }
}
