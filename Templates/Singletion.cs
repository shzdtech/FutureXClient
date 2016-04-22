using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Template
{
    public abstract class Singleton<T> where T:new()
    {
        private static readonly T _instance = new T();

        public static T Instance
        {
            get
            {
                return _instance;
            }
        }

    }
}
