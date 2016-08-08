using Micro.Future.CustomizedControls.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Micro.Future.CustomizedControls
{
    public static class AvalonLayoutExtenstions
    {
        public static LayoutAnchorable AddContent(this LayoutAnchorablePane pane, object content)
        {

            var layoutAnchorable = new LayoutAnchorable();
            layoutAnchorable.Content = content;
            var anchorableCtr = content as ILayoutAnchorableControl;
            if (anchorableCtr != null) anchorableCtr.AnchorablePane = pane;
            pane.Children.Add(layoutAnchorable);

            return layoutAnchorable;
        } 
    }
}
