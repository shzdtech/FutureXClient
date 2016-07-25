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
using System.Data;
using Xceed.Wpf.AvalonDock.Layout;
using System.Windows.Controls.Ribbon;
using System.Xml.Serialization;
using System.IO;
using Micro.Future.ViewModel;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using Micro.Future.Message;
using Micro.Future.Utility;
using Micro.Future.Properties;
using Micro.Future.Controls;

namespace Micro.Future.UI
{
    /// <summary>
    /// ClientTradeFrame.xaml 的交互逻辑
    /// </summary>
    public partial class ClientTradeFrame : UserControl, IAvalonAnchorable
    {
        public LayoutContent LayoutContent { get; set; }

        private ClientTradeFrame()
        {
            InitializeComponent();
        }

        private static ClientTradeFrame clientTradeFrame = null;

        public static ClientTradeFrame getClientTradeFrame() { if (clientTradeFrame == null) clientTradeFrame = new ClientTradeFrame(); return clientTradeFrame; }
        



    }
}
