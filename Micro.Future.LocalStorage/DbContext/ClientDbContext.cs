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
        public ClientDbContext()
        {
            ConnectionString = "data source=" + Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Data"), "clientcache.db");
        }

        public ClientDbContext(string connectionString) { ConnectionString = connectionString; }

        public DbSet<SyncInfo> SyncInfo { get; set; }

        public DbSet<ClientInfo> ClientInfo { get; set; }

        public DbSet<ContractInfo> ContractInfo { get; set; }

        public DbSet<PersonalContract> PersonalContract { get; set; }

        public DbSet<FilterSettings> FilterSettings { get; set; }

        public DbSet<MarketContract> MarketContract { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Composite primary key 
            modelBuilder.Entity<ContractInfo>().HasKey(c => new { c.Exchange, c.Contract });
            modelBuilder.Entity<PersonalContract>().HasKey(p => new { p.UserID, p.Contract });
            modelBuilder.Entity<MarketContract>().HasKey(m => new { m.AccountID, m.Contract });

        }

        public string ConnectionString { get; protected set; }

        //public DbSet<UserSetting> UserSetting { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(ConnectionString);
            //optionsBuilder.UseSqlite("Filename=E:\\Projects\\FutureXClient\\Micro.Future.LocalStorage\\Data\\clientcache.db");
        }


        public IList<ContractInfo> GetContractsByProductType(int productType)
        {
            return ContractInfo.Where(c => c.ProductType == productType).ToList();
        }

        private static IDictionary<int, IList<ContractInfo>> _contractCache = new Dictionary<int, IList<ContractInfo>>();

        public static IList<ContractInfo> GetContractFromCache(int productType)
        {
            IList<ContractInfo> ret;
            _contractCache.TryGetValue(productType, out ret);
            if (ret == null)
            {
                using (var ctx = new ClientDbContext())
                {
                    ret = ctx.GetContractsByProductType(productType);
                }
            }

            return ret;
        }


        public string GetSyncVersion(string item)
        {
            var version = from sync in SyncInfo
                          where sync.Item == item
                          select sync.Version;

            return version.FirstOrDefault();
        }

        public DateTime SetSyncVersion(string item, string version)
        {
            var syncItem = SyncInfo.Where(i => i.Item == item).FirstOrDefault();

            var now = DateTime.Now;

            if (syncItem == null)
            {
                syncItem = new SyncInfo()
                {
                    Item = item,
                    Version = version,
                    SyncTime = now
                };

                SyncInfo.Add(syncItem);
            }
            else
            {
                syncItem.Version = version;
                syncItem.SyncTime = now;
            }

            return now;
        }

        public static void SaveFilterSettings(string ctrlID, int Id, string title, string exchange, string contract, string underlying)
        {
            using (var clientCtx = new ClientDbContext())
            {
                var filterinfo = clientCtx.FilterSettings.FirstOrDefault(t => t.Id == Id);
                if (filterinfo == null)
                {
                    filterinfo = new FilterSettings();
                    //insert new record
                    clientCtx.FilterSettings.Add(filterinfo);
                }
                filterinfo.CtrlID = ctrlID;
                filterinfo.Title = title;
                filterinfo.Exchange = exchange;
                filterinfo.Underlying = underlying;
                filterinfo.Contract = contract;

                clientCtx.SaveChanges();
            }
        }

        public static void SaveMarketContract(string userID, string contract)
        {
            using (var clientCtx = new ClientDbContext())
            {
                var marketcontract = clientCtx.MarketContract.FirstOrDefault(t => t.Contract == contract);
                if (marketcontract == null)
                {
                    marketcontract = new MarketContract();
                    //insert new record
                    clientCtx.MarketContract.Add(marketcontract);
                }

                marketcontract.AccountID = userID;
                marketcontract.Contract = contract;

                clientCtx.SaveChanges();
            }
        }

        public static IEnumerable<string> GetUserContracts(string userId)
        {
            using (var clientCtx = new ClientDbContext())
            {
                return (from u in clientCtx.MarketContract
                        where u.AccountID == userId
                        select u.Contract).ToList();
            }
                
        }
        public static IList<FilterSettings> GetFilterSettings(string userId, string ctrlID)
        {
            using (var clientCtx = new ClientDbContext())
            {
                return (clientCtx.FilterSettings.Where(c => c.UserID == userId && c.CtrlID == ctrlID)).ToList();
            }
        }


    }
}

