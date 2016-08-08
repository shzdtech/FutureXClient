using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;

namespace Micro.Future.CustomizedControls.Controls
{
    public interface ILayoutAnchorableControl
    {
        LayoutAnchorablePane AnchorablePane { get; set; }
    }
}
