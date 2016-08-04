using Micro.Future.Message;
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
            MessageHandlerContainer.Register<OTCMDTradingDeskHandler, OTCMDTradingDeskHandler>();
            MessageHandlerContainer.Register<MarketDataHandler, MarketDataHandler>();
            MessageHandlerContainer.Register<TraderExHandler, TraderExHandler>();
            MessageHandlerContainer.DefaultInstance.Refresh();

            base.OnStartup(e);
        }


    }
}
