using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PBWrapMsgOG;
using PBMsgTrader;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows;
using PBWrapMsgMDA;
using System.Reflection;
using Micro.Future.Message;

namespace Micro.Future.ViewModel
{
    public class AddOrderViewModel : ViewModelBase
    {
        #region Property
        public AddOrderView View { get; set; }

        private string mSymbolID = "";
        public string SymbolID
        {
            get { return mSymbolID; }
            set
            {
                mSymbolID = value;
                UpdateView();
                OnPropertyChanged("SymbolID");
            }
        }

        private int mSize = 0;
        public int Size
        {
            get { return mSize; }
            set
            {
                mSize = value;
                UpdateView();
                OnPropertyChanged("Size");
            }
        }

        private OrderDirection mOrderDirection = OrderDirection.Buy;
        public OrderDirection OrderDirection
        {
            get { return mOrderDirection; }
            set
            {
                mOrderDirection = value;
                UpdateView();
            }
        }

        private ExecuteType mExecuteType = ExecuteType.Limit;
        public ExecuteType ExecutionType
        {
            get { return mExecuteType; }
            set
            {
                mExecuteType = value;
                UpdateView();
                OnPropertyChanged("ExecutionType");
            }
        }

        private bool mIsTodayEffective = true;
        public bool IsTodayEffective
        {
            get { return mIsTodayEffective; }
            set
            {
                mIsTodayEffective = value;
                UpdateView();
                OnPropertyChanged("IsTodayEffective");
            }
        }

        private double mStopPrice = 0.0;
        public double StopPrice
        {
            get { return mStopPrice; }
            set
            {
                mStopPrice = value;
                UpdateView();
                OnPropertyChanged("StopPrice");
            }
        }

        private double mLimitPrice = 0.0;
        public double LimitPrice
        {
            get { return mLimitPrice; }
            set
            {
                mLimitPrice = value;
                UpdateView();
                OnPropertyChanged("LimitPrice");
            }
        }

        private FutureFlag mFutureFlag = FutureFlag.Open;
        public FutureFlag FutureFlag
        {
            get { return mFutureFlag; }
            set
            {
                mFutureFlag = value;
                UpdateView();
                OnPropertyChanged("FutureFlag");
            }
        }
        #endregion

        #region Commands
        RelayCommand _buyLimitCommand;
        public ICommand BuyLimitCommand
        {
            get
            {
                if (_buyLimitCommand == null)
                {
                    _buyLimitCommand = new RelayCommand(
                        param => MakeOrder(PBWrapMsgOG.OrderDirection.Buy, PBWrapMsgOG.ExecuteType.Limit),
                        param => View.SubmitEnabled
                        );
                }
                return _buyLimitCommand;
            }
        }

        RelayCommand _sellLimitCommand;
        public ICommand SellLimitCommand
        {
            get
            {
                if (_sellLimitCommand == null)
                {
                    _sellLimitCommand = new RelayCommand(
                        param => MakeOrder(PBWrapMsgOG.OrderDirection.Sell, PBWrapMsgOG.ExecuteType.Limit),
                        param => View.SubmitEnabled
                        );
                }
                return _sellLimitCommand;
            }
        }

        RelayCommand _buyMarketCommand;
        public ICommand BuyMarketCommand
        {
            get
            {
                if (_buyMarketCommand == null)
                {
                    _buyMarketCommand = new RelayCommand(
                        param => MakeOrder(PBWrapMsgOG.OrderDirection.Buy, PBWrapMsgOG.ExecuteType.Market),
                        param => CanExecuteMarket
                        );
                }
                return _buyMarketCommand;
            }
        }

        RelayCommand _sellMarketCommand;
        public ICommand SellMarketCommand
        {
            get
            {
                if (_sellMarketCommand == null)
                {
                    _sellMarketCommand = new RelayCommand(
                        param => MakeOrder(PBWrapMsgOG.OrderDirection.Sell, PBWrapMsgOG.ExecuteType.Market),
                        param => CanExecuteMarket
                        );
                }
                return _sellMarketCommand;
            }
        }

        RelayCommand _closeAllCommand;
        public ICommand CloseAllCommand
        {
            get
            {
                if (_closeAllCommand == null)
                {
                    _closeAllCommand = new RelayCommand(
                        param => CloseAllOrders(),
                        param => CanExecuteDefault
                        );
                }
                return _closeAllCommand;
            }
        }
        #endregion

        #region Public Methods
        public AddOrderViewModel(AddOrderView view)
        {
            View = view;
            DisplayName = "AddOrderViewModel";
        }

        //for limit order
        public bool AllInformationIsCollected()
        {
            bool result = true;
            result &= !string.IsNullOrEmpty(mSymbolID);
            result &= mSize > 0;

            switch (mExecuteType)
            {
                case ExecuteType.Market:
                    result &= false;
                    break;
                case ExecuteType.Limit:
                    result &= mLimitPrice > 0.0;
                    break;
                case ExecuteType.Stop:
                    result &= mStopPrice > 0.0;
                    break;
                case ExecuteType.Stoplimit:
                    result &= (mLimitPrice > 0.0) && (mStopPrice > 0.0);
                    break;
                default:
                    result &= false;
                    break;
            }

            result &= mIsTodayEffective;

            return result;
        }

        public PBMsgTrader.PBMsgOrderInsert CreateOrder(FutureFlag futureFlag)
        {
            PBMsgOrderInsert pb = new PBMsgOrderInsert();

            //pb.SetMsgId((int)MsgIdOrder.ID_ORDER_REQ_NEW);
            pb.InstrumentID = mSymbolID;
            pb.VolumeTotalOriginal = mSize;

            switch (mOrderDirection)
            {
                case OrderDirection.Buy:
                    pb.Direction = "0";
                    break;
                case OrderDirection.Sell:
                    pb.Direction = "1";
                    break;
                case OrderDirection.Unknown:
                    break;
                default:
                    break;
            }

            switch (mExecuteType)
            {
                case ExecuteType.Market:
                    pb.OrderPriceType = ("1");
                    pb.ContingentCondition = ("1");
                    pb.TimeCondition = ("1");
                    LimitPrice = 0.0;
                    break;
                case ExecuteType.Limit:
                    pb.OrderPriceType = ("2");
                    pb.ContingentCondition = ("1");
                    pb.TimeCondition = ("3");
                    break;
                //case ExecuteType.STOP:
                //    pb.SetContingentCondition("2");
                //    pb.SetOrderPriceType("4");
                //    break;
                //case ExecuteType.STOPLIMIT:
                //    pb.SetOrderPriceType("2");
                //    pb.SetContingentCondition("2");
                //    break;
                default:
                    pb.OrderPriceType = ("4");
                    pb.ContingentCondition = ("1");
                    break;
            }

            pb.StopPrice = (StopPrice);
            pb.LimitPrice = (LimitPrice);

            //开平
            switch (futureFlag)
            {
                case FutureFlag.Open:
                    pb.CombOffsetFlag=("0");
                    break;
                case FutureFlag.Close:
                    pb.CombOffsetFlag=("1");
                    break;
                case FutureFlag.Closetoday:
                    pb.CombOffsetFlag=("3");
                    break;
                default:
                    pb.CombOffsetFlag=("0");
                    break;
            }

            return pb;
        }

        /// <summary>
        /// Returns true if symbolID isn't empty
        /// </summary>
        bool CanExecuteDefault
        {
            get { return !String.IsNullOrEmpty(mSymbolID); }
        }

        bool CanExecuteMarket
        {
            get { return (!String.IsNullOrEmpty(mSymbolID)) && (mSize > 0); }
        }

        #endregion // Public Methods

        private void UpdateView()
        {
            View.SubmitEnabled = AllInformationIsCollected();
        }

        //only for fast order
        private void MakeOrder(OrderDirection direction, ExecuteType iexecuteType)
        {
            //View.CollectCurrentSettings();
            //{
            OrderDirection = direction;
            ExecutionType = iexecuteType;
            PBMsgTrader.PBMsgOrderInsert pb = CreateOrder(mFutureFlag);
            View.SendOrder(pb);
            //}
            //else
            //{
            //    MessageBox.Show("请检查是否选中报价或量等", "错误");
            //}
        }

        //only for one button order
        //note: the only difference id not to collect basic info
        public void DirectMakeOrder(OrderDirection direction, ExecuteType iexecuteType)
        {
            OrderDirection = direction;
            ExecutionType = iexecuteType;
            PBMsgTrader.PBMsgOrderInsert pb = CreateOrder(mFutureFlag);
            View.SendOrder(pb);
        }

        private void CloseAllOrders()
        {
            //View.CollectCurrentSettings();
            //FutureFlag = PBWrapMsgOG.FutureFlag.CLOSE;
            ClosePosition(true);
            ClosePosition(false);
        }

        private void ClosePosition(bool longOrShort)
        {
            int position = 0;//; TradeHandler.Instance.GetPosition(SymbolID, longOrShort);
            if (position > 0)
            {
                Size = position;
                if (longOrShort)
                {
                    OrderDirection = PBWrapMsgOG.OrderDirection.Sell;
                }
                else
                {
                    OrderDirection = PBWrapMsgOG.OrderDirection.Buy;
                }

                PBMsgTrader.PBMsgOrderInsert pb = CreateOrder(FutureFlag.Close);
                View.SendOrder(pb);
            }
        }
    }

    public interface AddOrderView
    {
        //note:only for UI button not one key order
        bool SubmitEnabled { set; get; }
        void SendOrder(PBMsgTrader.PBMsgOrderInsert pb);
    }

    public class MockAddOrderView : AddOrderView
    {
        public bool submitEnabled;
        public bool SubmitEnabled
        {
            get
            {
                return submitEnabled;
            }
            set
            {
                submitEnabled = value;
                submitEnabledCount++;
            }
        }
        public int submitEnabledCount;

        public void SendOrder(PBMsgTrader.PBMsgOrderInsert pb)
        {
        }
    }
}
