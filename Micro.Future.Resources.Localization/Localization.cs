using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Resources.Localization
{
    public class LocalizationInfo
    {
        public static readonly string AssemblyName = Assembly.GetExecutingAssembly().FullName;

        public const string ResourceFile = "Resources";
    }
}
