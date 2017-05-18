﻿using Micro.Future.CustomizedControls.Controls;
using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using Micro.Future.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class OptionContractRiskGraphCtrl : UserControl

    {
        private List<KeyValuePair<RiskVM, double>> _optionRiskVMList = new List<KeyValuePair<RiskVM, double>>();

        public class StrategyBaseVM
        {
            public string Contract
            {
                get;
                set;
            }
            public string Expiration
            {
                get;
                set;
            }
            public bool RiskGraphEnable
            {
                get;
                set;
            }

        }

        private TraderExHandler _tradeExHandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();
        private OTCOptionTradeHandler _otcOptionTradeHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
        private OTCOptionTradingDeskHandler _otcOptionHandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
        public ObservableCollection<RiskVM> RiskVMCollection
        {
            get;
        } = new ObservableCollection<RiskVM>();
        public RiskBarVM RiskBarVM { get; } = new RiskBarVM();


        public OptionContractRiskGraphCtrl()
        {
            InitializeComponent();
            var portfolioVMCollection = MessageHandlerContainer.DefaultInstance.Get<AbstractOTCHandler>()?.PortfolioVMCollection;
            portfolioCB.ItemsSource = portfolioVMCollection;
            //_tradeExHandler.on += OnRiskParamsReceived;
        }

        private async void portfolioCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (portfolioCB.SelectedValue != null)
            {
                var portfolio = portfolioCB.SelectedValue?.ToString();
                var strategyVMCollection = _otcOptionHandler?.StrategyVMCollection;
                var strategyContractList = strategyVMCollection.Where(s => s.Portfolio == portfolio && !string.IsNullOrEmpty(s.BaseContract))
                    .GroupBy(s => s.BaseContract).Select(c => new StrategyBaseVM { Contract = c.First().BaseContract }).ToList();

                foreach (var vm in strategyContractList)
                {
                    var contractinfo = ClientDbContext.FindContract(vm.Contract);
                    if (contractinfo != null)
                    {
                        vm.Expiration = contractinfo.ExpireDate;
                    }
                }
                expirationLV.ItemsSource = strategyContractList;
                var riskVMlist = await _otcOptionTradeHandler.QueryRiskAsync(portfolio);
                foreach (var vm in riskVMlist)
                {
                    var contractinfo = ClientDbContext.FindContract(vm.Contract);
                    if (contractinfo != null)
                    {
                        //vm.StrikePrice = contractinfo.StrikePrice;
                        //_optionRiskVMList.Add(new KeyValuePair<RiskVM, double>(vm, vm.StrikePrice));
                        //RiskBarVM.add
                    }
                }


            }
        }
        private void OnRiskParamsReceived(RiskVM riskVM)
        {
            int idx;
            if (riskVM != null)
            {
                idx = _optionRiskVMList.FindIndex(c => c.Key.EqualContract(riskVM));
                if (idx > 0)
                {
                    //RiskBarVM.RiskBar[idx] = double.IsNaN(riskVM.Delta) ? 

                }

            }

        }


    }
}
