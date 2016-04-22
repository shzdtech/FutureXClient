using Micro.Future.Message.Business;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Message.PBMessageHandler
{
    public class SimpleTable2Map
    {
        public static IDictionary<string, IList<string>> ToMap(SimpleStringTable st)
        {
            Dictionary<string, IList<string>> ret = new Dictionary<string, IList<string>>();

            foreach (var col in st.ColumnsList)
            {
                ret[col.Name] = col.EntryList;
            }
            return ret;
        }

        public static IDictionary<string, IList<double>> ToMap(SimpleDoubleTable st)
        {
            Dictionary<string, IList<double>> ret = new Dictionary<string, IList<double>>();

            foreach (var col in st.ColumnsList)
            {
                ret[col.Name] = col.EntryList;
            }
            return ret;
        }

        public static IDictionary<string, IList<int>> ToMap(SimpleIntTable st)
        {
            Dictionary<string, IList<int>> ret = new Dictionary<string, IList<int>>();

            foreach (var col in st.ColumnsList)
            {
                ret[col.Name] = col.EntryList;
            }
            return ret;
        }
    }

    public class ComplexTable2Map
    {
        public static IDictionary<string, IList<string>> ToStringMap(ComplexTable ct)
        {
            IDictionary<string, IList<string>> ret = null;
            if (ct.HasStringTable)
            {
                ret = SimpleTable2Map.ToMap(ct.StringTable);
            }
            return ret;
        }

        public static IDictionary<string, IList<double>> ToDoubleMap(ComplexTable ct)
        {
            IDictionary<string, IList<double>> ret = null;
            if (ct.HasDoubleTable)
            {
                ret = SimpleTable2Map.ToMap(ct.DoubleTable);
            }
            return ret;
        }

        public static IDictionary<string, IList<int>> ToIntMap(ComplexTable ct)
        {
            IDictionary<string, IList<int>> ret = null;
            if (ct.HasIntTable)
            {
                ret = SimpleTable2Map.ToMap(ct.IntTable);
            }
            return ret;
        }
    }
}
