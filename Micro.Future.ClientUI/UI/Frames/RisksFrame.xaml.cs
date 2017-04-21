using System.Collections.Generic;
using System.Windows;
using Micro.Future.Message;
using System.ComponentModel;
using System.Windows.Controls;
using Micro.Future.CustomizedControls;
using System;
using System.Windows.Controls.Primitives;
using Micro.Future.Resources.Localization;
using Micro.Future.UI;
using Micro.Future.Utility;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.CustomizedControls.Windows;
using System.Threading.Tasks;
using System.Threading;
using Micro.Future.LocalStorage;
using Micro.Future.Windows;

namespace Micro.Future.UI
{
    /// <summary>
    /// xaml 的交互逻辑
    /// </summary>
    public partial class RisksFrame : UserControl, IUserFrame
    {
        public RisksFrame()
        {
            InitializeComponent();
            Initialize();
        }

        public string Title
        {
            get
            {
                return frameMenu.Header.ToString();
            }
        }

        public IStatusCollector StatusReporter
        {
            get; set;
        }

        public TaskCompletionSource<bool> LoginTaskSource
        {
            get;
        } = new TaskCompletionSource<bool>();

        public IEnumerable<MenuItem> FrameMenus
        {
            get
            {
                return Resources["exMenuItems"] as IEnumerable<MenuItem>;
            }
        }

        public IEnumerable<StatusBarItem> StatusBarItems
        {
            get
            {
                return Resources["exStatusBarItems"] as IEnumerable<StatusBarItem>;
            }
        }

        public void Initialize()
        {
            //portfolioCtl.AnchorablePane = portfolioselectPane;
            marketDataLV.AnchorablePane = quotePane;
            //greeksControl.AnchorablePane = greeksPane;
            positionsWindow.AnchorablePane = positionPane;
            tradeWindow.AnchorablePane = tradePane;
            var marketdataHandler = MessageHandlerContainer.DefaultInstance.Get<MarketDataHandler>();
            marketDataLV.MarketDataHandler = marketdataHandler;

        }

        public Task<bool> LoginAsync(string brokerId, string usernname, string password, string server = null)
        {
            throw new NotImplementedException();
        }
    }
}
