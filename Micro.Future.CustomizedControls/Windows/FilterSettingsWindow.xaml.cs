﻿using System;
using System.ComponentModel;
using System.Windows;

namespace Micro.Future.Windows
{

    public partial class FilterSettingsWindow : Window
    {
        public event Action<string, string, string, string, string> OnFiltering;

        public FilterSettingsWindow()
        {
            InitializeComponent();
        }

        public string FilterTabTitle
        {
            get
            {
                return titleTxt.Text;
            }
            set { titleTxt.Text = value; }
        }

        public string FilterExchange
        {
            get
            {
                return exchangecombo.Text;
            }
            set { exchangecombo.Text = value; }
        }

        public string FilterUnderlying
        {
            get
            {
                return underlyingTxt.Text;
            }
            set { underlyingTxt.Text = value; }
        }

        public string FilterContract
        {
            get
            {
                return contractTxt.Text;
            }
            set { contractTxt.Text = value; }
        }

        public string FilterPortfolio
        {
            get
            {
                return portfolioTxt.Text;
            }
            set { portfolioTxt.Text = value; }
        }


        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            OnFiltering?.Invoke(FilterTabTitle, FilterExchange, FilterPortfolio, FilterUnderlying, FilterContract);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            Hide();
            e.Cancel = CancelClosing;
            base.OnClosing(e);
        }
        
        public bool CancelClosing
        {
            get; set;
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            titleTxt.Text = "";
            exchangecombo.Text = "";
            underlyingTxt.Text = "";
            contractTxt.Text = "";
            portfolioTxt.Text = "";

        }
    }
}
