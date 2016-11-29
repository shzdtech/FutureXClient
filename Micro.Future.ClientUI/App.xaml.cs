using Micro.Future.Business.Handler.Business;
using Micro.Future.Message;
using Micro.Future.Properties;
using Micro.Future.Utility;
using System;
using System.Collections.Generic;
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

            Dictionary<string, string> configDict;
            if (config.Content.TryGetValue("ACCOUNTSERVER", out configDict))
            {
                MessageHandlerContainer.Register<AccountHandler, AccountHandler>(GenSignInOption(configDict));
            }

            if (config.Content.TryGetValue("OTCSERVER", out configDict))
            {
                MessageHandlerContainer.Register<AbstractOTCHandler, OTCContractHandler>(GenSignInOption(configDict));
            }

            if (config.Content.TryGetValue("OTCOPTIONSERVER", out configDict))
            {
                MessageHandlerContainer.Register<OTCOptionHandler, OTCOptionHandler>(GenSignInOption(configDict));
            }

            if (config.Content.TryGetValue("CTPMDSERVER", out configDict))
            {
                MessageHandlerContainer.Register<MarketDataHandler, MarketDataHandler>(GenSignInOption(configDict));
            }

            if (config.Content.TryGetValue("CTPTRADESERVER", out configDict))
            {
                MessageHandlerContainer.Register<TraderExHandler, TraderExHandler>(GenSignInOption(configDict));
            }

            if (config.Content.TryGetValue("CTPOPTIONSERVER", out configDict))
            {
                configDict = config.Content["CTPOPTIONSERVER"];
                MessageHandlerContainer.Register<CTPOptionDataHandler, CTPOptionDataHandler>(GenSignInOption(configDict));
            }

            configDict = config.Content["CTSMDSERVER"];
            MessageHandlerContainer.Register<CTSMarketDataHandler, CTSMarketDataHandler>
               (new SignInOptions
               {
                   FrontServer = configDict["ADDRESS"],
                   ReconnectTimeSpan = TimeSpan.Parse(configDict["RECONN_TIMESPAN"])
               });

            configDict = config.Content["CTSTRADESERVER"];
            MessageHandlerContainer.Register<CTSTradeHandler, CTSTradeHandler>
               (new SignInOptions
               {
                   FrontServer = configDict["ADDRESS"],
                   ReconnectTimeSpan = TimeSpan.Parse(configDict["RECONN_TIMESPAN"])
               });

            MessageHandlerContainer.DefaultInstance.Refresh();

            base.OnStartup(e);
        }

        private SignInOptions GenSignInOption(IDictionary<string, string> configDict)
        {
            return new SignInOptions
            {
                FrontServer = configDict["ADDRESS"],
                ReconnectTimeSpan = TimeSpan.Parse(configDict["RECONN_TIMESPAN"]),
                EncryptPassword = configDict.ContainsKey("HASH_PASSWORD") ? bool.Parse(configDict["HASH_PASSWORD"]) : false
            };
        }
    }
}
