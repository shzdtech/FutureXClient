using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Micro.Future.LocalStorage;
using System.Linq;
using System.Text;
using Micro.Future.Utility;
using System.Collections.ObjectModel;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;





namespace Micro.Future.Utility
{
    public static class Utility
    {
        public static string GenComponentUri(Type type)
        {
            return "/" + type.Assembly.FullName + ";component";
        }

        public static bool ContainsAny(this string thisString, string findStr, params char[] seperator)
        {
            if (string.IsNullOrWhiteSpace(findStr))
                return true;

            var strArray = findStr.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            foreach (var str in strArray)
                if (!string.IsNullOrWhiteSpace(str) &&
                    thisString.IndexOf(str.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return true;

            return false;
        }

        public static bool ContainsAny(this string thisString, string findStr)
        {
            return ContainsAny(thisString, findStr, ';', ',');
        }


        // to show Client version
        public static void setCurrentVersion()
        {

        }

        private static void getClientVersion()
        {
            using (var clientDBCtx = new ClientDbContext())
            {

            }
        }
    }
}
