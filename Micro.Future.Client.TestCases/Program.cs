using Micro.Future.LocalStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Client.TestCases
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var dbctx = new ClientDbContext())
            {
                dbctx.ContractInfo.Add(new LocalStorage.DataObject.ContractInfo()
                {
                    Id = new Random().Next(),
                });
                dbctx.SaveChanges();
            }
        }
    }
}
