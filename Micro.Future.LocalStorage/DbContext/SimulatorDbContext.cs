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
    public class SimulatorDbContext : DbContext
    {
        public SimulatorDbContext()
        {
            ConnectionString = "data source=" + Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Data"), "simulation.db");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
        }

        public string ConnectionString { get; protected set; }
        public SimulatorDbContext(string connectionString) { ConnectionString = connectionString; }

        public DbSet<MarketData> MarketData { get; set; }
        public DbSet<MarketDataOpt> MarketDataOpt { get; set; }
    }
}
