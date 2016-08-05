using Micro.Future.Message;
using Micro.Future.Properties;
using Micro.Future.Utility;
using System;
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
            Config config = new Config(Settings.Default.ConfigFile);

            var configDict = config.Content["OTCCLIENTSERVER"];
            MessageHandlerContainer.Register<AbstractOTCMarketDataHandler, OTCMDClientHandler>
                (new SignInOptions {
                    FrontServer = configDict["ADDRESS"],
                    ReconnectTimeSpan = TimeSpan.Parse(configDict["RECONN_TIMESPAN"])
                });

            configDict = config.Content["OTCTDSERVER"];
            MessageHandlerContainer.Register<OTCMDTradingDeskHandler, OTCMDTradingDeskHandler>
                (new SignInOptions
                {
                    FrontServer = configDict["ADDRESS"],
                    ReconnectTimeSpan = TimeSpan.Parse(configDict["RECONN_TIMESPAN"])
                });

            configDict = config.Content["CTPMDSERVER"];
            MessageHandlerContainer.Register<MarketDataHandler, MarketDataHandler>
                (new SignInOptions
                {
                    FrontServer = configDict["ADDRESS"],
                    ReconnectTimeSpan = TimeSpan.Parse(configDict["RECONN_TIMESPAN"])
                });

            configDict = config.Content["CTPTRADESERVER"];
            MessageHandlerContainer.Register<TraderExHandler, TraderExHandler>
               (new SignInOptions
               {
                   FrontServer = configDict["ADDRESS"],
                   ReconnectTimeSpan = TimeSpan.Parse(configDict["RECONN_TIMESPAN"])
               });

            MessageHandlerContainer.DefaultInstance.Refresh();

            base.OnStartup(e);
        }

    }
}
