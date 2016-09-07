using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxyPlot
{
    public class CustomOxyMarkers
    {
        public static readonly ScreenPoint[] LUTriangle = new[] { new ScreenPoint(0, 1), new ScreenPoint(-1, -1), new ScreenPoint(0, -1) };
        public static readonly ScreenPoint[] RUTriangle = new[] { new ScreenPoint(0, 1), new ScreenPoint(0, -1), new ScreenPoint(1, -1), };
        public static readonly ScreenPoint[] LDTriangle = new[] { new ScreenPoint(-1, 1), new ScreenPoint(0, 1), new ScreenPoint(0, -1) };
        public static readonly ScreenPoint[] RDTriangle = new[] { new ScreenPoint(0, 1), new ScreenPoint(1, 1), new ScreenPoint(0, -1), };
        public static readonly ScreenPoint[] LRectangle = new[] { new ScreenPoint(-1, 1), new ScreenPoint(-1, -1), new ScreenPoint(0, -1), new ScreenPoint(0, 1) };
        public static readonly ScreenPoint[] RRectangle = new[] { new ScreenPoint(0, 1), new ScreenPoint(0, -1), new ScreenPoint(1, -1), new ScreenPoint(1, 1), };
    }
}
