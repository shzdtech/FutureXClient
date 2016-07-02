using System.Linq;
using Micro.Future.UI;

namespace Micro.Future.ViewModel
{
    public class RegionViewModel : TreeViewItemViewModel
    {
       // readonly string _region;

        public RegionViewModel(string region, bool byProductClassOrExchange) 
            : base(null, false)
        {
            //_region = region;
            Name = region;
            LoadChildren1(byProductClassOrExchange);
        }

        //obsolete
        public RegionViewModel(string region)
            : base(null, false)
        {
            //_region = region;
            Name = region;
            LoadChildren();
        }

        //public string RegionName
        //{
        //    get { return _region; }
        //}

        protected override void LoadChildren()
        {
            var query = from info in InstrumentVMList.Instance where info.ProductClass == Name select info.RawData.ExchangeID;

            foreach (string state in query.Distinct().ToList())
                base.Children.Add(new StateViewModel(state, this));
        }

        protected void LoadChildren1(bool byProductClassOrExchange)
        {
            if (byProductClassOrExchange)
            {
                var query = from info in InstrumentVMList.Instance where info.ProductClass == Name select info.RawData.ExchangeID;

                foreach (string state in query.Distinct().ToList())
                    base.Children.Add(new StateViewModel(state, this, byProductClassOrExchange));
            }
            else
            {
                var query = from info in InstrumentVMList.Instance where info.RawData.ExchangeID == Name select info.ProductClass;

                foreach (string state in query.Distinct().ToList())
                    base.Children.Add(new StateViewModel(state, this, byProductClassOrExchange));
            }
        }
    }
}