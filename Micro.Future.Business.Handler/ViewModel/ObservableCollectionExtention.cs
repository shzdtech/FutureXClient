using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{
    public static class ObservableCollectionExtention
    {
        public static bool ExistContract<T>(this ObservableCollection<T> oc, T contractKey)
            where T : IContractKey
        {
            return oc.Any(contract => contract.EqualContract(contract));
        }

        public static bool ExistContract<T>(this ObservableCollection<T> oc,
            string exchange, string contract)
            where T : IContractKey
        {
            return oc.Any(c => c.EqualContract(exchange, contract));
        }

        public static T FindContract<T>(this ObservableCollection<T> oc, T contractKey)
            where T : IContractKey
        {
            return oc.FirstOrDefault(contract => contract.EqualContract(contract));
        }

        public static T FindContract<T>(this ObservableCollection<T> oc,
            string exchange, string contract)
            where T : IContractKey
        {
            return oc.FirstOrDefault(c => c.EqualContract(exchange, contract));
        }

        public static IEnumerable<T> FindByContract<T>(this ObservableCollection<T> oc, string contract)
            where T : IContractKey
        {
            return oc.Where(c => c.Contract == contract);
        }

        public static bool ExistOrder<T>(this ObservableCollection<T> oc, ulong orderId)
            where T : OrderVM
        {
            return oc.Any(order => order.OrderID == orderId);
        }

        public static T FindOrder<T>(this ObservableCollection<T> oc, ulong orderId)
            where T : OrderVM
        {
            return oc.FirstOrDefault(order => order.OrderID == orderId);
        }

    }
}
