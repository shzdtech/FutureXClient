﻿using System;
using System.Linq;
using Micro.Future.LocalStorage;
using System.Reflection;

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
        public static Version ClientVersion
        {
            get
            {
                return Assembly.GetEntryAssembly().GetName().Version;
            }
        }

        public static string GetSyncVersion(string item)
        {
            using (var clientDbCtx = new ClientDbContext())
            {
                return clientDbCtx.GetSyncVersion(item);
            }
        }

        public static DateTime SetSyncVersion(string item, string version)
        {
            using (var clientDbCtx = new ClientDbContext())
            {
                var syncTm = clientDbCtx.SetSyncVersion(item, version);

                clientDbCtx.SaveChanges();

                return syncTm;
            }
        }
    }
}