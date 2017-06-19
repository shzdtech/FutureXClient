using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{

    public class ValuationParam
    {
        public double Price { get; set; }
        public double Volatitly { get; set; }
    }
    //报价
    public class QueryValuation
    {
        public IDictionary<string, ValuationParam> ContractParams
        {
            get;
        } = new Dictionary<string, ValuationParam>();

        public double? Interest { get; set; }
        public int? DaysRemain { get; set; }
    }
}
