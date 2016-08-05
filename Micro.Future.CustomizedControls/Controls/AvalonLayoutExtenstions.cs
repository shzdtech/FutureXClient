using Xceed.Wpf.AvalonDock.Layout;

namespace Micro.Future.CustomizedControls
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
