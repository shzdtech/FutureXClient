using Micro.Future.LocalStorage;
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
        private IList<ColumnObject> _columns;
        public ColumnSettingsWindow(IList<ColumnObject> columns)
        {
            InitializeComponent();
            _columns = columns;
            treeColumns.ItemsSource = columns;
        }
        private void column_checked(object sender, RoutedEventArgs e)
        {

        }

        private void column_unchecked(object sender, RoutedEventArgs e)
        {

        }

    }

    public class ColumnObject : DependencyObject
    {
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
                cobj.Restore();
                //cobj.Save();
            }
            else
            {
                cobj.Hide();
                //cobj.Remove();
            }

            foreach (var c in cobj.Children)
            {
                c.SetValue(IsVisibleProperty, isChecked);
            }
        }


        #endregion

        //从ListView中获取ColumnObject对象
        public static IList<ColumnObject> GetColumns(ListView lv)
        {
            if (lv.View is GridView)
            {
                var collec = ((GridView)lv.View).Columns;
                var listCol = new List<ColumnObject>();
                for (int i = 0; i < collec.Count; i++)
                {
                    listCol.Add(new ColumnObject(collec[i], i));
                }

                return listCol;
            }
            return null;
        }

        public static ColumnObject CreateColumn(GridViewColumn column)
        {
            return new ColumnObject(column, -1);
        }

        #region 属性/字段

        //GridViewColumn集合
        public int OriginalIndex { get; set; }

        private object _originalHeader;
        public object OriginalHeader
        {
            get
            {
                return _originalHeader ?? Column.Header;
            }
            set
            {
                _originalHeader = value;
            }
        }

        public double Width { get; set; }

        public GridViewColumn Column { get; private set; }
        public string ColumnId
        {
            get;
            set;
        }

        public string UserID
        {
            get;
            set;
        }
        #endregion

        public ColumnObject(GridViewColumn column, int originalIdx = -1)
        {
            Column = column;
            OriginalIndex = originalIdx;
        }

        public void Initialize()
        {
            foreach (var child in Children)
            {
                child.Parent = this;
                child.Initialize();
            }
        }

        public void Hide()
        {
            //记住隐藏时列的位置，显示的时候尝试排列在原来位置
            var header = new GridViewColumnHeader();
            header.Visibility = Visibility.Hidden;
            Width = Column.ActualWidth;
            OriginalHeader = Column.Header;
            Column.Header = header;
            Column.Width = 0;
        }

        public void Restore()
        {
            Column.Width = Width;
            Column.Header = OriginalHeader;
        }

        public void Save()
        {

            if (OriginalIndex >= 0)
            {
                ClientDbContext.SaveColumnSettings(UserID, ColumnId, OriginalIndex);
            }
        }

        public void SaveAll(IList<ColumnObject> cols)
        {

            ClientDbContext.DeleteAllColumnSettings(UserID, ColumnId);

            foreach (var col in cols)
            {
                if (!col.Visible)
                {
                    col.Save();
                }
            }
        }

        public void RestoreAll(IList<ColumnObject> cols)
        {

            var hidecols = ClientDbContext.GetColumnSettings(UserID, ColumnId);
            foreach (var col in hidecols)
            {
                if (col.ColumnIdx >= 0 && col.ColumnIdx < cols.Count)
                {
                    cols[col.ColumnIdx].Hide();
                }
            }
        }

        public void Remove()
        {
            if (OriginalIndex >= 0)
            {
                ClientDbContext.DeleteColumnSettings(UserID, ColumnId, OriginalIndex);
            }
        }

        public bool Visible
        {
            get
            {
                bool ret = true;
                var header = OriginalHeader as GridViewColumnHeader;
                if (header != null)
                {
                    ret = header.Visibility == Visibility.Visible;
                }

                return ret;
            }
        }

        public IList<ColumnObject> Children { get; private set; } = new List<ColumnObject>();

        public ColumnObject Parent { get; private set; }
    }
}
