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
    }
}
