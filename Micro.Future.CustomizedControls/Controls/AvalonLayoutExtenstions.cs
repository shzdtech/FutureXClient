using Micro.Future.CustomizedControls.Controls;
using System;
using Xceed.Wpf.AvalonDock.Layout;

namespace Micro.Future.CustomizedControls
{
    public static class AvalonLayoutExtenstions
    {
        public static LayoutAnchorable AddContent(this LayoutAnchorablePane pane, object content)
        {

            var layoutAnchorable = new LayoutAnchorable();
            layoutAnchorable.ContentId = Guid.NewGuid().ToString();
            layoutAnchorable.Content = content;
            var anchorableCtr = content as ILayoutAnchorableControl;
            if (anchorableCtr != null) anchorableCtr.AnchorablePane = pane;
            pane.Children.Add(layoutAnchorable);

            return layoutAnchorable;
        } 
    }
}
