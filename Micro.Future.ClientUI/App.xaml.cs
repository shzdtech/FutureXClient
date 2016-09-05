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

            var configDict = config.Content["OTCSERVER"];
            MessageHandlerContainer.Register<AbstractOTCHandler, OTCContractHandler>
                (new SignInOptions {
                    FrontServer = configDict["ADDRESS"],
                    ReconnectTimeSpan = TimeSpan.Parse(configDict["RECONN_TIMESPAN"])
                });

            configDict = config.Content["OTCOPTIONSERVER"];
            MessageHandlerContainer.Register<OTCOptionHandler, OTCOptionHandler>
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

            configDict = config.Content["CTPOPTIONSERVER"];
            MessageHandlerContainer.Register<CTPOptionDataHandler, CTPOptionDataHandler>
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
