﻿using System;
using System.Windows;
using Micro.Future.Util;
using Micro.Future.Message;
using System.Collections;
using System.Security.Cryptography;
using Micro.Future.ViewModel;
using System.Windows.Input;
using System.Windows.Controls;

namespace Micro.Future.UI
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class FastOrderWin : Window
    {
        private string _currentContract;

        private bool submitEnabled;
        public bool SubmitEnabled
        {
            get
            {
                return submitEnabled;
            }
            set
            {
                submitEnabled = value;
            }
        }

        public void OnQuoteSelected(QuoteViewModel quoteVM)
        {
            if (quoteVM != null)
            {
                _currentContract = quoteVM.Contract;
                stackPanelPrices.DataContext = quoteVM;
                OrderVM.Contract = quoteVM.Contract;

            }
        }

        public void OnPositionSelected(PositionVM positionVM)
        {
            if (positionVM != null)
            {
                OrderVM.Contract = positionVM.Contract;
                OrderVM.OffsetFlag = OrderOffsetType.CLOSE;

                if (positionVM.Direction == PositionDirectionType.PD_SHORT)
                {
                    OrderVM.Direction = DirectionType.BUY;
                }
                else
                {
                    OrderVM.Direction = DirectionType.SELL;
                }
            }
        }

        public OrderVM OrderVM
        {
            get;
            private set;
        }

        public FastOrderWin()
        {
            OrderVM = new OrderVM();
            InitializeComponent();
            DataContext = OrderVM;
        }

        private OrderVM _orderVM = new OrderVM();

        private void labelupperprice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LimitTxt.Value = double.Parse(LabelUpperPrice.Content.ToString());
        }

        private void labellowerprice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LimitTxt.Value = double.Parse(LabelLowerPrice.Content.ToString());
        }

        private void FastOrderContract_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_currentContract != null && FastOrderContract.Text != _currentContract)
                stackPanelPrices.DataContext = null;
            //LabelUpperPrice.Content = string.Empty;
            //LabelBidPrice.Content = string.Empty;
            //LabelAskPrice.Content = string.Empty;
            //LabelLowerPrice.Content = string.Empty;
        }

        private void labelbidprice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LimitTxt.Value = double.Parse(LabelBidPrice.Content.ToString());
        }

        private void labelaskprice_MouseDown(object sender, MouseButtonEventArgs e)
        {
            LimitTxt.Value = double.Parse(LabelAskPrice.Content.ToString());
        }
    }
}