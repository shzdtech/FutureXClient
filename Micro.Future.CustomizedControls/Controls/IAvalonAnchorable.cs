using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.AvalonDock.Layout;
using System.Windows;

namespace Micro.Future.Controls
{
    public static class AvalonLayoutExtenstions
    {
        public static LayoutAnchorable AddContent(this LayoutAnchorablePane pane, object content)
        {

            var layoutAnchorable = new LayoutAnchorable();
            layoutAnchorable.Content = content;
            pane.Children.Add(layoutAnchorable);

            return layoutAnchorable;
        } 
    }
}
