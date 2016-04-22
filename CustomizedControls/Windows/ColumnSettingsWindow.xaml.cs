using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
        public ColumnSettingsWindow(ColumnObject[] columns)
        {
            InitializeComponent();
            columnsList.ItemsSource = columns;
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
            new PropertyMetadata(true, OnIsVisibleChanged));

        static void OnIsVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cobj = (ColumnObject)d;
            if ((bool)e.NewValue)
            {
                //尝试还原位置，此时有可能由于别的列也被隐藏造成位置无效
                if (cobj.index < 0 || cobj.index > cobj.collec.Count)
                    cobj.index = cobj.collec.Count - 1;
                cobj.collec.Insert(cobj.index, cobj.Column);
            }
            else
            {
                //记住隐藏时列的位置，显示的时候尝试排列在原来位置
                cobj.index = cobj.collec.IndexOf(cobj.Column);
                if (cobj.index != -1)
                {
                    cobj.collec.RemoveAt(cobj.index);
                }
            }
        }

        #endregion

        #region 静态成员

        //从ListView中获取ColumnObject对象
        public static ColumnObject[] GetColumns(ListView lv)
        {
            if (lv.View is GridView)
            {
                var collec = ((GridView)lv.View).Columns;
                return collec.Select(col => new ColumnObject(col, collec)).ToArray();
            }
            return null;
        }

        #endregion

        #region 属性/字段

        //GridViewColumn集合
        GridViewColumnCollection collec;
        int index;
        public GridViewColumn Column { get; private set; }

        #endregion

        public ColumnObject(GridViewColumn column, GridViewColumnCollection collec)
        {
            Column = column;
            this.collec = collec;
        }

    }
}
