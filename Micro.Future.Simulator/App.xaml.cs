using Micro.Future.Message;
using Micro.Future.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Micro.Future.Simulator
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Config config = new Config("settings.ini");

            var configDict = config.Content["TESTINGSERVER"];

            MessageHandlerContainer.Register<SimMarketDataHandler, SimMarketDataHandler>
               (new SignInOptions
               {
                   FrontServer = configDict["ADDRESS"],
                   UserName = configDict["USERNAME"],
                   Password = configDict["PASSWORD"],
                   ReconnectTimeSpan = TimeSpan.Parse(configDict["RECONN_TIMESPAN"])
               });

            MessageHandlerContainer.DefaultInstance.Refresh();

            base.OnStartup(e);
        }
    }
}
