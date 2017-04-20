using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Micro.Future.UI
{
    public class PanesStyleSelector : StyleSelector
    {
        public Style DocumentStyle
        {
            get;
            set;
        }
        //public interface IDocument : ILayoutItem
        //{
        //    IUndoRedoManager UndoRedoManager { get; }
        //}
        //public override Style SelectStyle(object item, DependencyObject container)
        //{
        //    if (item is IDocument)
        //        return DocumentStyle;
        //    return base.SelectStyle(item, container);
        //}
    }
}
