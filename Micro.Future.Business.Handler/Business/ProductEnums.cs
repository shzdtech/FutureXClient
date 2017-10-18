using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public enum ProductType
    {
        PRODUCT_UNSPECIFIED = -1,
        ///期货
        PRODUCT_FUTURE = 0,
        ///期货期权
        PRODUCT_OPTIONS = 1,
        ///组合
        PRODUCT_COMBINATION = 2,
        ///即期
        PRODUCT_SPOT = 3,
        ///期转现
        PRODUCT_EFP = 4,
        ///现货期权
        PRODUCT_SPOTOPTION = 5,
        ///个股期权
        PRODUCT_ETFOPTION = 6,
        ///证券
        PRODUCT_STOCK = 7,
        ///OTC合约
        PRODUCT_OTC = 8,
        ///OTC期权
        PRODUCT_OTC_OPTION = 9,
        ///OTC个股期权
        PRODUCT_OTC_ETFOPTION = 10,
        ///OTC证券
        PRODUCT_OTC_STOCK = 11,
        ///
        PRODUCT_UPPERBOUND,
    };

    public enum ContractType
    {
        CONTRACTTYPE_UNSPECIFIED = 0,
        CONTRACTTYPE_FUTURE = 1,
        CONTRACTTYPE_CALL_OPTION = 2,
        CONTRACTTYPE_PUT_OPTION = 3,
        CONTRACTTYPE_SPREAD = 4,
        CONTRACTTYPE_BUTTERFLY = 5,
        CONTRACTTYPE_INDEX_2_LEGS = 6,
        CONTRACTTYPE_INDEX_4_LEGS = 7,
        CONTRACTTYPE_CASHSPOT = 8,
    };
}
