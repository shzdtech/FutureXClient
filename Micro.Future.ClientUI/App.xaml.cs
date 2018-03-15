using Micro.Future.Business.Handler.Business;
using Micro.Future.Business.Handler.Router;
using Micro.Future.Message;
using Micro.Future.Properties;
using Micro.Future.Utility;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace Micro.Future
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Xceed.Wpf.DataGrid.Licenser.LicenseKey = "DGP62-JKBGJ-GLWE5-PZ2A";
            base.OnStartup(e);

            //DispatcherUnhandledException += AppGlobalDispatcherUnhandledException;


            // Initialize server configuration
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

            if (config.Content.TryGetValue("OTCOPTIONMDSERVER", out configDict))
            {
                MessageHandlerContainer.Register<OTCOptionDataHandler, OTCOptionDataHandler>(GenSignInOption(configDict));
                var mdhandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionDataHandler>();
                OTCMarketDataHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_OTC_OPTION, mdhandler);

                MessageHandlerContainer.Register<OTCOptionTradingDeskHandler, OTCOptionTradingDeskHandler>(GenSignInOption(configDict));
                var tradingdeskhandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradingDeskHandler>();
                TradingDeskHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_OTC_OPTION, tradingdeskhandler);
                TradingDeskHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_OPTIONS, tradingdeskhandler);
                TradingDeskHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_FUTURE, tradingdeskhandler);
            }

            if (config.Content.TryGetValue("OTCOPTIONTDSERVER", out configDict))
            {
                MessageHandlerContainer.Register<OTCOptionTradeHandler, OTCOptionTradeHandler>(GenSignInOption(configDict));

                var tdhandler = MessageHandlerContainer.DefaultInstance.Get<OTCOptionTradeHandler>();
                OTCTradeHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_OPTIONS, tdhandler);
                OTCTradeHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_OTC_OPTION, tdhandler);
                OTCTradeHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_FUTURE, tdhandler);
            }

            if (config.Content.TryGetValue("CTPMDSERVER", out configDict))
            {
                MessageHandlerContainer.Register<MarketDataHandler, MarketDataHandler>(GenSignInOption(configDict));

                var mdhandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
                MarketDataHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_FUTURE, mdhandler);
                MarketDataHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_OPTIONS, mdhandler);
                MarketDataHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_COMBINATION, mdhandler);
                MarketDataHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_SPOT, mdhandler);
            }

            if (config.Content.TryGetValue("CTPTRADESERVER", out configDict))
            {
                MessageHandlerContainer.Register<TraderExHandler, TraderExHandler>(GenSignInOption(configDict));

                var tdhandler = MessageHandlerContainer.DefaultInstance.Get<TraderExHandler>();

                CompositeTradeExHandler.DefaultInstance.RegisterHandler(tdhandler);

                TradeExHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_FUTURE, tdhandler);
                TradeExHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_OPTIONS, tdhandler);
                TradeExHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_COMBINATION, tdhandler);
                TradeExHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_SPOT, tdhandler);
            }

            if (config.Content.TryGetValue("CTPETFMDSERVER", out configDict))
            {
                MessageHandlerContainer.Register<CTPETFMDHandler, CTPETFMDHandler>(GenSignInOption(configDict));
                var mdhandler = MessageHandlerContainer.DefaultInstance.Get<CTPETFMDHandler>();
                MarketDataHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_ETFOPTION, mdhandler);
            }

            if (config.Content.TryGetValue("CTPETFTRADESERVER", out configDict))
            {
                MessageHandlerContainer.Register<CTPETFTraderHandler, CTPETFTraderHandler>(GenSignInOption(configDict));
                var tdhandler = MessageHandlerContainer.DefaultInstance.Get<CTPETFTraderHandler>();

                CompositeTradeExHandler.DefaultInstance.RegisterHandler(tdhandler);

                TradeExHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_ETFOPTION, tdhandler);
            }
            if (config.Content.TryGetValue("CTPSTOCKMDSERVER", out configDict))
            {
                MessageHandlerContainer.Register<CTPSTOCKMDHandler, CTPSTOCKMDHandler>(GenSignInOption(configDict));
                var mdhandler = MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKMDHandler>();
                MarketDataHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_STOCK, mdhandler);
            }

            if (config.Content.TryGetValue("CTPSTOCKTRADESERVER", out configDict))
            {
                MessageHandlerContainer.Register<CTPSTOCKTraderHandler, CTPSTOCKTraderHandler>(GenSignInOption(configDict));
                var tdhandler = MessageHandlerContainer.DefaultInstance.Get<CTPSTOCKTraderHandler>();

                CompositeTradeExHandler.DefaultInstance.RegisterHandler(tdhandler);

                TradeExHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_STOCK, tdhandler);
            }

            if (config.Content.TryGetValue("CTPOPTIONSERVER", out configDict))
            {
                MessageHandlerContainer.Register<CTPOptionDataHandler, CTPOptionDataHandler>(GenSignInOption(configDict));
            }

            if (config.Content.TryGetValue("OTCETFTRADESERVER", out configDict))
            {
                MessageHandlerContainer.Register<OTCETFTradeHandler, OTCETFTradeHandler>(GenSignInOption(configDict));
                var tdhandler = MessageHandlerContainer.DefaultInstance.Get<OTCETFTradeHandler>();
                OTCTradeHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_OTC_ETFOPTION, tdhandler);
                OTCTradeHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_ETFOPTION, tdhandler);

            }
            if (config.Content.TryGetValue("OTCETFMDSERVER", out configDict))
            {
                MessageHandlerContainer.Register<ETFOTCOptionDataHandler, ETFOTCOptionDataHandler>(GenSignInOption(configDict));
                var mdhandler = MessageHandlerContainer.DefaultInstance.Get<ETFOTCOptionDataHandler>();
                OTCMarketDataHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_OTC_ETFOPTION, mdhandler);

                MessageHandlerContainer.Register<OTCETFTradingDeskHandler, OTCETFTradingDeskHandler>(GenSignInOption(configDict));
                var tradingdeskhandler = MessageHandlerContainer.DefaultInstance.Get<OTCETFTradingDeskHandler>();
                TradingDeskHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_ETFOPTION, tradingdeskhandler);
                TradingDeskHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_OTC_ETFOPTION, tradingdeskhandler);
            }

            if (config.Content.TryGetValue("OTCSTOCKTRADESERVER", out configDict))
            {
                MessageHandlerContainer.Register<OTCStockTradeHandler, OTCStockTradeHandler>(GenSignInOption(configDict));
                var tdhandler = MessageHandlerContainer.DefaultInstance.Get<OTCStockTradeHandler>();
                OTCTradeHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_OTC_STOCK, tdhandler);
                OTCTradeHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_STOCK, tdhandler);

            }
            if (config.Content.TryGetValue("OTCSTOCKMDSERVER", out configDict))
            {
                MessageHandlerContainer.Register<StockOTCOptionDataHandler, StockOTCOptionDataHandler>(GenSignInOption(configDict));
                var mdhandler = MessageHandlerContainer.DefaultInstance.Get<StockOTCOptionDataHandler>();
                OTCMarketDataHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_OTC_STOCK, mdhandler);

                MessageHandlerContainer.Register<OTCStockTradingDeskHandler, OTCStockTradingDeskHandler>(GenSignInOption(configDict));
                var tradingdeskhandler = MessageHandlerContainer.DefaultInstance.Get<OTCStockTradingDeskHandler>();
                TradingDeskHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_STOCK, tradingdeskhandler);
                TradingDeskHandlerRouter.DefaultInstance.RegisterHandler(ProductType.PRODUCT_OTC_STOCK, tradingdeskhandler);
            }



            if (config.Content.TryGetValue("CTSMDSERVER", out configDict))
            {
                MessageHandlerContainer.Register<CTSMarketDataHandler, CTSMarketDataHandler>(GenSignInOption(configDict));
            }

            if (config.Content.TryGetValue("CTSTRADESERVER", out configDict))
            {
                MessageHandlerContainer.Register<CTSTradeHandler, CTSTradeHandler>(GenSignInOption(configDict));
            }

            MessageHandlerContainer.DefaultInstance.Refresh();
        }

        private void AppGlobalDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }

        private SignInOptions GenSignInOption(IDictionary<string, string> configDict)
        {
            string brokeId;
            configDict.TryGetValue("BROKERID", out brokeId);
            return new SignInOptions
            {
                FrontServer = configDict["ADDRESS"],
                BrokerID = brokeId,
                ReconnectTimeSpan = TimeSpan.Parse(configDict["RECONN_TIMESPAN"]),
                EncryptPassword = configDict.ContainsKey("HASH_PASSWORD") ? bool.Parse(configDict["HASH_PASSWORD"]) : false
            };
        }
    }
}
