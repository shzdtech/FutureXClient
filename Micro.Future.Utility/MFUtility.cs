using System;
using System.Linq;
using Micro.Future.LocalStorage;





namespace Micro.Future.Utility
{
    public static class MFUtilities
    {
        public static string clientVersion;

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


        //get client version from localStorage
        public static string ClientVersion
        {
            get
            {
                using (var clientDbCtx = new ClientDbContext())
                {
                    var queryClicentInfo = from db in clientDbCtx.ClientInfo
                                           group db by db.Id into g
                                           select new { g.Key, MaxVersion = g.Max(db => db.Version) };

                    int queryCount = queryClicentInfo.Count();

                    if (queryCount == 1)
                    {
                        Console.WriteLine("找到了最大版本号");
                        foreach (var result in queryClicentInfo)
                        {
                            clientVersion = result.MaxVersion;
                            Console.WriteLine(clientVersion);
                        }
                    }
                    return clientVersion;
                }
            }
        }
    }
}