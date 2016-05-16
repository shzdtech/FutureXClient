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
        public static bool Exist<T>(this ObservableCollection<T> oc, Func<T, bool> func)
        {
            for (int i = 0; i < oc.Count; i++)
            {
                if (func(oc[i])) return true;
            }
            return false;
        }

        public static T Find<T>(this ObservableCollection<T> oc, Func<T, bool> func)
        {
            for (int i = 0; i < oc.Count; i++)
            {
                if (func(oc[i]))
                    return oc[i];
            }

            return default(T);
        }

        public static bool ExistContract<T>(this ObservableCollection<T> oc, T contractKey)
            where T : IContractKey
        {
            return Exist<T>(oc, (contract) => contract.EqualContract(contract));
        }

        public static bool ExistContract<T>(this ObservableCollection<T> oc, 
            string exchange, string contract)
            where T : IContractKey
        {
            return Exist<T>(oc, (obj) => obj.EqualContract(exchange, contract));
        }

        public static T FindContract<T>(this ObservableCollection<T> oc, T contractKey)
            where T : IContractKey
        {
            return  Find<T>(oc, (contract)=> contract.EqualContract(contract));
        }

        public static T FindContract<T>(this ObservableCollection<T> oc,
            string exchange, string contract)
            where T : IContractKey
        {
            return Find<T>(oc, (obj) => obj.EqualContract(exchange, contract));
        }

        public static bool ExistOrder<T>(this ObservableCollection<T> oc, ulong orderId)
            where T : OrderVM
        {
            return Exist<T>(oc, (order) => order.OrderID == orderId);
        }

        public static T FindOrder<T>(this ObservableCollection<T> oc, ulong orderId)
            where T : OrderVM
        {
            return Find<T>(oc, (order) => order.OrderID == orderId);
        }

    }
}
