﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel 
{
    public class HedgeVM : ViewModelBase
    {
        private string _hedgeName;
        public string HedgeName
        {
            get { return _hedgeName; }
            set
            {
                _hedgeName = value;
                OnPropertyChanged("HedgeName");
            }
        }
        private string _portfolio;
        public string Portfolio
        {
            get { return _portfolio; }
            set
            {
                _portfolio = value;
                OnPropertyChanged("Portfolio");
            }
        }
        private bool _hedge;
        public bool Hedge
        {
            get { return _hedge; }
            set
            {
                _hedge = value;
                OnPropertyChanged("Hedge");
            }
        }
        private string _exchange;
        public string Exchange
        {
            get { return _exchange; }
            set
            {
                _exchange = value;
                OnPropertyChanged("Exchange");
            }
        }
        private string _underlying;
        public string Underlying
        {
            get { return _underlying; }
            set
            {
                _underlying = value;
                OnPropertyChanged("Underlying");
            }
        }
        private string _contract;
        public string Contract
        {
            get { return _contract; }
            set
            {
                _contract = value;
                OnPropertyChanged("Contract");
            }
        }
        public ObservableCollection<PricingContractParamVM> HedgeContracts
        {
            get;
        } = new ObservableCollection<PricingContractParamVM>();

    }
}
