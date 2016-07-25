using Micro.Future.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.AvalonDock.Layout;

namespace Micro.Future.UI
{
    /// <summary>
    /// ClientStrategyFrame.xaml 的交互逻辑
    /// </summary>
    public partial class ClientStrategyFrame : UserControl, IAvalonAnchorable
    {
        private ClientStrategyFrame()
        {
            InitializeComponent();
        }

        public LayoutContent LayoutContent
        {
            get;
            set;
        }

        private static ClientStrategyFrame clientStrategyFrame = null;

        public static ClientStrategyFrame getClientStrategyFrame()
        {
            if (clientStrategyFrame == null) clientStrategyFrame = new ClientStrategyFrame();
            return clientStrategyFrame;
        }

        private void dockingManager_Unloaded(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("您已关闭当前高端策略窗口，您可以重新点击交易行情再次打开");
            ClientMainWindowOTC.isClientStrategyFrameLoaded = false;
        }
    }
}
