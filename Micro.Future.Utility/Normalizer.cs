using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Micro.Future.Utility
{
    public static class Normalizer
    {
        public static double EPS = 1e-20;

        public static double Normalize(double value, int precision = 3)
        {
            long rnd = (long)Math.Round(value);
            return Math.Abs(value - rnd) > EPS ? Math.Round(value, precision) : rnd;
        } 
    }
}
