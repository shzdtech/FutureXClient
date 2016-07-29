using Micro.Future.Message;
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
        //add by 马小帅, to control the Trade Window which to be show up.
        //public static bool TradeIn = true;
        //public static bool TradeOut = false;

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
