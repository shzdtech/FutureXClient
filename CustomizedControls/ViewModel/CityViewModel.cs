using System.Linq;
using Micro.Future.UI;

namespace Micro.Future.ViewModel
{
    public class CityViewModel : TreeViewItemViewModel
    {
        //readonly string _city;

        public CityViewModel(string city, StateViewModel parentState)
            : base(parentState, false)
        {
            //_city = city;
            Name = city;
        }

        //public string CityName
        //{
        //    get { return _city; }
        //}

        public void LoadDetails(object DataContext)
        {
            if (IsSelected)
            {
                var query = from info in InstrumentVMList.Instance where info.RawData.InstrumentID == Name select info;

                if (query.Count() ==1)
                {
                    DataContext = query.ElementAt(0).GetDetails().GetEnumerator();
                }
            }
        }
    }
}