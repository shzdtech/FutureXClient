using Micro.Future.LocalStorage.DataObject;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.LocalStorage
{
    public class ClientDbContext : DbContext
    {
        public ClientDbContext() : base() {
            ConnectionString = "Filename=" + Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "clientcache.db");
        }

        public ClientDbContext(string connectionString) : base() { ConnectionString = connectionString; }

        public DbSet<ClientInfo> ClientInfo { get; set; }


        public DbSet<ContractInfo> ContractInfo { get; set; }

        public string ConnectionString { get; protected set; }

        //public DbSet<UserSetting> UserSetting { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
            //optionsBuilder.UseSqlite("Filename=E:\\Projects\\FutureXClient\\Micro.Future.LocalStorage\\Data\\clientcache.db");
        }


    }
}
