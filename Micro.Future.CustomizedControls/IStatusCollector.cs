using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.CustomizedControls
{
    public interface IStatusCollector
    {
        void ReportStatus(string statusMsg);
    }
}
