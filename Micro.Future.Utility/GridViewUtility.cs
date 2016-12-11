using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;

namespace Micro.Future.Utility
{
    public class GridViewUtility
    {
        public static void Sort(GridViewColumn clickedColumn, ItemCollection itemCollection)
        {
            if (clickedColumn != null)
            {
                //Get binding property of clicked column
                string bindingProperty = (clickedColumn.DisplayMemberBinding as Binding).Path.Path;
                SortDescriptionCollection sdc = itemCollection.SortDescriptions;
                ListSortDirection sortDirection = ListSortDirection.Ascending;
                if (sdc.Count > 0)
                {
                    SortDescription sd = sdc[0];
                    sortDirection = sd.Direction == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                    sdc.Clear();
                }
                sdc.Add(new SortDescription(bindingProperty, sortDirection));
            }
        }
    }
}
