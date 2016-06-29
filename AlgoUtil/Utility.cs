using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Util
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

            var strArray = findStr.Split(seperator);
            foreach (var str in strArray)
                if (thisString.IndexOf(str.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0)
                    return true;

            return false;
        }

        public static bool ContainsAny(this string thisString, string findStr)
        {
            return ContainsAny(thisString, findStr, ';', ',');
        }
    }
}
