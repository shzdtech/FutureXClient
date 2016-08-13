using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.LocalStorage.DataObject
{
    public class ContractInfo
    {
        ///交易所代码
        public string Exchange { get; set; }
        ///合约代码
        public string Contract { get; set; }
        ///合约名称
        public string Name { get; set; }
        ///产品代码
        public string ProductID { get; set; }
        ///产品类型
        public int ProductType { get; set; }
        ///交割年份
        public int DeliveryYear { get; set; }
        ///交割月
        public int DeliveryMonth { get; set; }
        ///市价单最大下单量
        public int MaxMarketOrderVolume { get; set; }
        ///市价单最小下单量
        public int MinMarketOrderVolume { get; set; }
        ///限价单最大下单量
        public int MaxLimitOrderVolume { get; set; }
        ///限价单最小下单量
        public int MinLimitOrderVolume { get; set; }
        ///合约数量乘数
        public int VolumeMultiple { get; set; }
        ///最小变动价位
        public double PriceTick { get; set; }
        ///创建日
        public string CreateDate { get; set; }
        ///上市日
        public string OpenDate { get; set; }
        ///到期日
        public string ExpireDate { get; set; }
        ///开始交割日
        public string StartDelivDate { get; set; }
        ///结束交割日
        public string EndDelivDate { get; set; }
        ///合约生命周期状态
        public int LifePhase { get; set; }
        ///当前是否交易
        public bool IsTrading { get; set; }
        ///持仓类型
        public int PositionType { get; set; }
        ///持仓日期类型
        public int PositionDateType { get; set; }
        ///多头保证金率
        public double LongMarginRatio { get; set; }
        ///空头保证金率
        public double ShortMarginRatio { get; set; }
        ///是否使用大额单边保证金算法
        public string UnderlyingExchange { get; set; }
        public string UnderlyingContract { get; set; }
    }

}
