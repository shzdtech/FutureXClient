using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Business.Handler.ViewModel
{
    public class ContractVM
    {
        private string contractName;
        public string ContractName
        {
            get { return contractName; }
            set { this.contractName = value; }
        }
    }
}
