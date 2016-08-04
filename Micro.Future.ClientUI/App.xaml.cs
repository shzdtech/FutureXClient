using Micro.Future.Message;
using Micro.Future.Properties;
using Micro.Future.Utility;
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

            MessageHandlerContainer.Register<AbstractOTCMarketDataHandler, OTCMDClientHandler>
                (new SignInOptions { FrontServer = config.Content["OTCCLIENTSERVER"]["ADDRESS"] });

            MessageHandlerContainer.Register<OTCMDTradingDeskHandler, OTCMDTradingDeskHandler>
                (new SignInOptions { FrontServer = config.Content["OTCTDSERVER"]["ADDRESS"] });

            MessageHandlerContainer.Register<MarketDataHandler, MarketDataHandler>
                (new SignInOptions { FrontServer = config.Content["CTPMDSERVER"]["ADDRESS"] });

            MessageHandlerContainer.Register<TraderExHandler, TraderExHandler>
                (new SignInOptions { FrontServer = config.Content["CTPTRADESERVER"]["ADDRESS"] });

            MessageHandlerContainer.DefaultInstance.Refresh();

            base.OnStartup(e);
        }

    }
}
