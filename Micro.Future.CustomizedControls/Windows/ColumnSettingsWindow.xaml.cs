using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Micro.Future.UI
{
    /// <summary>
    /// ColumnSettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ColumnSettingsWindow : Window
    {
        public ColumnSettingsWindow(IEnumerable<ColumnObject> columns)
        {
            InitializeComponent();
            treeColumns.ItemsSource = columns;
        }
    }

    public class ColumnObject : DependencyObject
    {
        //not necessary
        //#region 附加属性
        //public static DependencyProperty ItemsSourceFromColumnsProperty =
        //    DependencyProperty.RegisterAttached("ItemsSourceFromColumns", typeof(ListView), typeof(ColumnObject),
        //    new PropertyMetadata(OnItemsSourceFromColumnsChanged));

        //static void OnItemsSourceFromColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    //ColumnObject[] cols = null;

        //    //if (e.NewValue is ListView && ((cols = GetColumns((ListView)e.NewValue)) != null))
        //    //    ((ItemsControl)d).ItemsSource = cols;
        //}

        //public static ListView GetItemsSourceFromColumns(ItemsControl ic)
        //{
        //    return (ListView)ic.GetValue(ItemsSourceFromColumnsProperty);
        //}

        //public static void SetItemsSourceFromColumns(ItemsControl ic, ListView lv)
        //{
        //    ic.SetValue(ItemsSourceFromColumnsProperty, lv);
        //}

        //#endregion

        #region IsVisible 依赖属性

        public static DependencyProperty IsVisibleProperty =
            DependencyProperty.Register("IsVisible", typeof(bool), typeof(ColumnObject),
            new FrameworkPropertyMetadata(true,
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal,
            new PropertyChangedCallback(OnIsVisibleChanged)));

        static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cobj = (ColumnObject)d;
            bool isChecked = (bool)e.NewValue;
            if (isChecked)
            {
                //尝试还原位置，此时有可能由于别的列也被隐藏造成位置无效
                cobj.Column.Width = cobj.Width;
                cobj.Column.Header = cobj.OriginalHeader;
            }
            else
            {
                //记住隐藏时列的位置，显示的时候尝试排列在原来位置
                var header = new GridViewColumnHeader();
                header.Visibility = Visibility.Hidden;
                cobj.Width = cobj.Column.ActualWidth;
                cobj.OriginalHeader = cobj.Column.Header;
                cobj.Column.Header = header;
                cobj.Column.Width = 0;
            }

            foreach (var c in cobj.Children)
            {
                c.SetValue(IsVisibleProperty, isChecked);
            }
        }


        #endregion

        //从ListView中获取ColumnObject对象
        public static ColumnObject[] GetColumns(ListView lv)
        {
            if (lv.View is GridView)
            {
                var collec = ((GridView)lv.View).Columns;
                return collec.Select(col => new ColumnObject(col)).ToArray();
            }
            return null;
        }

        public static ColumnObject CreateColumn(GridViewColumn column)
        {
            return new ColumnObject(column);
        }

        #region 属性/字段

        //GridViewColumn集合
        protected object OriginalHeader { get; set; }
        protected double Width { get; set; }

        public GridViewColumn Column { get; private set; }

        #endregion

        public ColumnObject(GridViewColumn column)
        {
            Column = column;
        }

        public void Initialize()
        {
            foreach (var child in Children)
            {
                child.Parent = this;
                child.Initialize();
            }
        }

        public IList<ColumnObject> Children { get; private set; } = new List<ColumnObject>();
        public ColumnObject Parent { get; private set; }
    }
}
