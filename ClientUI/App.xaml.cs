﻿using Micro.Future.Message;
using Micro.Future.Constant;
using Micro.Future.Message.PBMessageHandler;
using Micro.Future.Properties;
using Micro.Future.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Micro.Future
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MessageHandlerContainer.Register<AbstractOTCMarketDataHandler, OTCMDClientHandler>();
            MessageHandlerContainer.Register<MarketDataHandler, MarketDataHandler>();
            MessageHandlerContainer.Register<TraderExHandler, TraderExHandler>();
            MessageHandlerContainer.DefaultInstance.Refresh();

            Config cfg = new Config(Settings.Default.ConfigFile);

            base.OnStartup(e);
        }
    }
}
