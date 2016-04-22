using System.Linq;
using Micro.Future.UI;

namespace Micro.Future.ViewModel
{
    public class StateViewModel : TreeViewItemViewModel
    {
        //readonly string _state;

        //obsolete
        public StateViewModel(string state, RegionViewModel parentRegion)
            : base(parentRegion, true)
        {
            //_state = state;
            Name = state;
            LoadChildren();
        }

        public StateViewModel(string state, RegionViewModel parentRegion, bool byProductClassOrExchange)
            : base(parentRegion, true)
        {
            //_state = state;
            Name = state;
            LoadChildren1(byProductClassOrExchange);
        }

        //public string StateName
        //{
        //    get { return _state; }
        //}

        protected override void LoadChildren()
        {
            RegionViewModel parent = _parent as RegionViewModel;
            string productName = parent.Name;
            var query = from info in InstrumentVMList.Instance where (info.ProductClass == productName) && (info.RawData.ExchangeID == Name) orderby info.InstrumentID  select info.InstrumentID;

            foreach (string city in query)
                base.Children.Add(new CityViewModel(city, this));
        }

        protected void LoadChildren1(bool byProductClassOrExchange)
        {
            RegionViewModel parent = _parent as RegionViewModel;

            if (byProductClassOrExchange)
            {
                string productName = parent.Name;
                var query = from info in InstrumentVMList.Instance where (info.ProductClass == productName) && (info.RawData.ExchangeID == Name) orderby info.InstrumentID select info.InstrumentID;

                foreach (string city in query)
                    base.Children.Add(new CityViewModel(city, this));
            }
            else
            {
                string exchange = parent.Name;
                var query = from info in InstrumentVMList.Instance where (info.RawData.ExchangeID == exchange) && (info.ProductClass == Name) orderby info.InstrumentID select info.InstrumentID;

                foreach (string city in query)
                    base.Children.Add(new CityViewModel(city, this));
            }

        }
    }
}