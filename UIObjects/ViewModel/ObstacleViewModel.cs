using System;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.ObjectModel;
using PBWrapMsgMDA;
using System.Collections.Generic;
using Micro;
using PBMsgTrader;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using PBWrapMsgOG;
using Micro.Future.Util;
using Micro.Future.Message;
using Micro.Future.Message.PBMessageHandler;
using System.Windows.Threading;

namespace Micro.Future.ViewModel
{

    public class PositionViewModel : ViewModelBase
    {
        public PositionViewModel(PBMsgQueryRspPosition rsp, string exchange)
        {
            RawData = rsp;
            Exchange = exchange;

            if (RawData.PosiDirection == "2")
            {
                PosiDirection = "多";
            }
            else if (RawData.PosiDirection == "3")
            {
                PosiDirection = "空";
            }
        }

        public PBMsgQueryRspPosition RawData { get; set; }

        public string Exchange { get; set; }

        public string PosiDirection { get; set; }
    }

    public class FundViewModel : ViewModelBase
    {
        public PBMsgQueryRspFund RawData { get; set; }
        public ObservableCollection<KeyValuePair<string, string>> Values { get; set; }

        private double mAvailable;
        public double Available
        {
            get { return mAvailable; }
            set
            {
                mAvailable = value;
                OnPropertyChanged("Available");
            }
        }

        private string mAccountID;
        public string AccountID
        {
            get { return mAccountID; }
            set
            {
                mAccountID = value;
                OnPropertyChanged("AccountID");
            }
        }

        public FundViewModel()
        {
            Values = new ObservableCollection<KeyValuePair<string, string>>();
        }

        public void Update(PBMsgQueryRspFund rsp)
        {
            RawData = rsp;
            AccountID = RawData.AccountID;

            Values.Clear();

            var dict = PBUtility.ParseProperties(rsp.AllFields, "gb2312");

            foreach (var keyvalue in dict)
            {
                Values.Add(keyvalue);
            }
        }

    }

    public class ExecutionViewModel : ViewModelBase
    {
        public string InstrumentID { get; set; }
        public string OrderRef { get; set; }
        public int BrokerOrderSeq { get; set; }
        public string InsertTime { get; set; }
        public string Exchange { get; set; }
        public string OrderSysID { get; set; }
        public int Sequence { get; set; }
        public string TradeID { get; set; }

        private bool? mFlag;
        public bool? Flag
        {
            get { return mFlag; }
            set
            {
                mFlag = value;
                OnPropertyChanged("Flag");
            }
        }

        private string mOrderStatus;
        public string OrderStatus
        {
            get { return mOrderStatus; }
            set
            {
                mOrderStatus = value;
                OnPropertyChanged("OrderStatus");
            }
        }

        private string mOrderStatusCN;
        public string OrderStatusCN
        {
            get { return mOrderStatusCN; }
            set
            {
                mOrderStatusCN = value;
                OnPropertyChanged("OrderStatusCN");
            }
        }

        private int mVolumeTraded;
        public int VolumeTraded
        {
            get { return mVolumeTraded; }
            set
            {
                mVolumeTraded = value;
                OnPropertyChanged("VolumeTraded");
            }
        }

        private int mVolumeOriginal;
        public int VolumeOriginal
        {
            get { return mVolumeOriginal; }
            set
            {
                mVolumeOriginal = value;
                OnPropertyChanged("VolumeOriginal");
            }
        }

        //剩余数量
        private int mVolumeTotal;
        public int VolumeTotal
        {
            get { return mVolumeTotal; }
            set
            {
                mVolumeTotal = value;
                OnPropertyChanged("VolumeTotal");
            }
        }

        private double mPrice;
        public double Price
        {
            get { return mPrice; }
            set
            {
                mPrice = value;
                OnPropertyChanged("Price");
            }
        }

        public double LimitPrice { get; set; }
        public double StopPrice { get; set; }

        private string mDirection;
        public string Direction
        {
            get { return mDirection; }
            set
            {
                mDirection = value;
                OnPropertyChanged("Direction");
            }
        }

        private string mCombOffsetFlag;
        public string CombOffsetFlag
        {
            get { return mCombOffsetFlag; }
            set
            {
                mCombOffsetFlag = value;
                OnPropertyChanged("CombOffsetFlag");
            }
        }

        public bool IsOrderOrTrade { get; set; }
        public PBOrderStatus Status { get; set; }

        private bool mExpanded;
        public bool Expanded
        {
            get { return mExpanded; }
            set
            {
                mExpanded = value;
                OnPropertyChanged("Expanded");
            }
        }

        public void Update(PBMsgOrderRtn pe, string exhange)
        {
            Flag = false;
            InstrumentID = pe.InstrumentID;
            OrderStatus = pe.OrderStatus;
            OrderStatusCN = pe.StatusMsg.ToString(Encoding.GetEncoding("gb2312"));
            VolumeTraded = pe.VolumeTraded;
            VolumeTotal = pe.VolumeTotal;
            VolumeOriginal = pe.VolumeTotalOriginal;

            if (VolumeTotal == 0)
            {
                Status = PBOrderStatus.ALL_FINISHED;
            }
            else if ((VolumeOriginal > VolumeTraded) && (VolumeTraded > 0))
            {
                Status = PBOrderStatus.PARTLY_FINISHED;
            }
            else
            {
                Status = (PBOrderStatus)pe.IOrderStatus;
            }

            OrderRef = pe.OrderRef;
            BrokerOrderSeq = pe.BrokerOrderSeq;
            InsertTime = pe.InsertTime;
            IsOrderOrTrade = true;

            //Direction = Enum.GetName(typeof(OrderDirection), int.Parse(pe.Direction));
            Direction = ParseDirection(pe.Direction);
            CombOffsetFlag = ParseFlag(pe.CombOffsetFlag);
            Exchange = exhange;
            OrderSysID = pe.OrderSysID;
            LimitPrice = pe.LimitPrice;
            StopPrice = pe.StopPrice;
        }

        public void Update(PBMsgTradeRtn pe, string exchange)
        {
            InstrumentID = pe.InstrumentID;
            InsertTime = pe.TradeTime;
            VolumeTraded = pe.Volume;
            OrderRef = pe.OrderRef;
            BrokerOrderSeq = pe.BrokerOrderSeq;
            IsOrderOrTrade = false;
            Price = pe.Price;
            Direction = ParseDirection(pe.Direction);
            CombOffsetFlag = ParseFlag(pe.OffsetFlag);
            Exchange = exchange;
            TradeID = pe.TradeID;
        }

        private string ParseDirection(string direction)
        {
            if (direction == "0")
            {
                return "买";
            }
            else if (direction == "1")
            {
                return "卖";
            }
            else
            {
                return "";
            }
        }

        private string ParseFlag(string flag)
        {
            if (flag == "0")
            {
                return "开仓";
            }
            else if (flag == "1")
            {
                return "平仓";
            }
            else if (flag == "3")
            {
                return "平今";
            }
            else
            {
                return "";
            }
        }
    }

    public class ExecutionCollection : DispatchObservableCollection<ExecutionViewModel>
    {
        public ExecutionCollection() { }

        public ExecutionCollection(DispatcherObject dispatcherObj) : base(dispatcherObj) { }

        public void Update(PBMsgOrderRtn pe, string exchange)
        {
            bool found = false;

            foreach (ExecutionViewModel child in this)
            {
                if (child.BrokerOrderSeq == pe.BrokerOrderSeq)
                {
                    found = true;
                    child.Update(pe, exchange);
                    Logger.Debug("Update PBMsgOrderRtn");
                    break;
                }
            }

            if (!found)
            {
                ExecutionViewModel evm = new ExecutionViewModel();
                evm.Update(pe, exchange);
                this.Add(evm);
                Logger.Debug("Add PBMsgOrderRtn");
            }

            //MainWindow.MyInstance.executionWindow.Refresh();
        }

        public void Update(PBMsgTradeRtn pe, string exchange)
        {
            var query = from row in this where pe.TradeID == row.TradeID select row;
            if (query.ToList().Count == 0)
            {
                ExecutionViewModel evm = new ExecutionViewModel();
                evm.Update(pe, exchange);
                this.Add(evm);

                //MainWindow.MyInstance.executionWindow.Refresh();
                Logger.Debug("Add Trade execution");
            }
        }
    }

    public sealed class InstrumentVMList : List<InstrumentViewModel>
    {
        private static readonly InstrumentVMList _instance = new InstrumentVMList();
        public static InstrumentVMList Instance
        {
            get
            {
                return _instance;
            }
        }
    }

    [Serializable]
    public class InstrumentViewModel
    {
        [XmlIgnore]
        public PBMsgQueryRspInstrumentInfo RawData { get; set; }

        public string ProductClass { get; set; }
        public string InstrumentID { get; set; }

        public IDictionary<string, string> GetDetails()
        {
            var dict = PBUtility.ParseProperties(RawData.AllFields, "gb2312");

            dict["ProductClass"] = ProductClass;

            return dict;
        }

        public void Adjust()
        {
            InstrumentID = RawData.InstrumentID;

            switch (RawData.ProductID.ToUpper())
            {
                case "AG":
                case "AU":
                    ProductClass = "贵金属";
                    break;
                case "JM":
                case "J":
                case "V":
                case "L":
                case "RU":
                case "P":
                case "TA":
                case "FG":
                case "ME":
                case "BU":
                case "FB":
                case "BB":
                    ProductClass = "工业材料";
                    break;
                case "A":
                case "B":
                case "C":
                case "Y":
                case "M":
                case "RM":
                case "OI":
                case "RS":
                case "PM":
                case "WH":
                case "RI":
                case "JR":
                    ProductClass = "谷物与油籽";
                    break;
                case "RB":
                case "AL":
                case "PB":
                case "CU":
                case "WR":
                case "ZN":
                case "I":
                    ProductClass = "金属";
                    break;
                case "FU":
                case "TC":
                    ProductClass = "能源";
                    break;
                case "SR":
                case "CF":
                case "JD":
                    ProductClass = "软产品与家畜";
                    break;
                case "IF":
                case "TF":
                    ProductClass = "市场指数";
                    break;
                default:
                    ProductClass = "未知";
                    break;
            }
        }
    }


    public class QuoteGroup
    {
        public bool Predefined { get; set; }
        public string Name { get; set; }

        //actually product id and the only instrument id
        public List<InstrumentViewModel> Quotes { get; set; }

        public QuoteGroup()
        {
            Quotes = new List<InstrumentViewModel>();
        }

        public void UpdatePreDefinedGroup(InstrumentViewModel qi)
        {
            if (Predefined)
            {
                foreach (InstrumentViewModel instrument in Quotes)
                {
                    //get product id
                    var query = from info in InstrumentVMList.Instance where info.InstrumentID == instrument.InstrumentID select info.RawData.ProductID;

                    //if the same product id
                    if (query.ToArray()[0] == qi.RawData.ProductID)
                    {
                        //nearest contract?
                        if (string.Compare(instrument.InstrumentID, qi.InstrumentID) > 0)
                        {
                            //replace
                            instrument.InstrumentID = qi.InstrumentID;
                        }

                        return;
                    }
                }

                //first of the same product id
                Quotes.Add(new InstrumentViewModel() { InstrumentID = qi.InstrumentID, RawData = qi.RawData, ProductClass = qi.ProductClass });
            }
        }

        public void AddSymbol(string symbole)
        {
            if (!Predefined)
            {
                bool found = false;
                foreach (InstrumentViewModel instrument in Quotes)
                {
                    if (instrument.InstrumentID == symbole)
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    Quotes.Add(new InstrumentViewModel() { InstrumentID = symbole });
                }
            }
        }
    }
}
