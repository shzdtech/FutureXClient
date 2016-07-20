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
using System;
using System.Windows.Controls.Ribbon;
using Micro.Future.Message;
using Micro.Future.Utility;
using Micro.Future.Properties;
using System.Threading;
using Xceed.Wpf.AvalonDock.Layout;
using System.Collections.Generic;


namespace Micro.Future.UI
{
    /// <summary>
    /// CustomPagexaml.xaml 的交互逻辑
    /// </summary>
    public partial class CustomPagexaml : Page
    {

        private const string CST_CONTROL_ASSEMBLY = "Micro.Future.Resources.Localization";
        private const string RESOURCE_FILE = "Resources";

        public CustomPagexaml()
        {
            InitializeComponent();
        }


        private void _ctpTradeSignIner_OnLogged(IUserInfo obj)
        {
            Thread.Sleep(2000);
            clientFundLV.ReloadData();
            Thread.Sleep(2000);
            positionsWindow.ReloadData();
            Thread.Sleep(2000);
            tradeWindow.ReloadData();
            Thread.Sleep(2000);
            executionWindow.ReloadData();
        }

        private void MenuItem_Click_Contract(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var quoteGrpVw = new ClientQuoteGroupView() { LayoutContent = ancable };
            ancable.Content = quoteGrpVw;
            ancable.Title = WPFUtility.GetLocalizedString("Optional", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            quotePane.Children.Add(ancable);
        }

        private void MenuItem_Click_ZhongJin(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var quoteGrpVw = new ClientQuoteGroupView() { LayoutContent = ancable };
            quoteGrpVw.Filter("CFFEX", "", "");
            ancable.Content = quoteGrpVw;
            ancable.Title = WPFUtility.GetLocalizedString("CFFEX", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            quotePane.Children.Add(ancable);
        }

        private void MenuItem_Click_ShangHai(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var quoteGrpVw = new ClientQuoteGroupView() { LayoutContent = ancable };
            quoteGrpVw.Filter("SHFE", "", "");
            ancable.Content = quoteGrpVw;
            ancable.Title = WPFUtility.GetLocalizedString("SHFE", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            quotePane.Children.Add(ancable);
        }

        private void MenuItem_Click_DaLian(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var quoteGrpVw = new ClientQuoteGroupView() { LayoutContent = ancable };
            quoteGrpVw.Filter("DCE", "", "");
            ancable.Content = quoteGrpVw;
            ancable.Title = WPFUtility.GetLocalizedString("DCE", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            quotePane.Children.Add(ancable);
        }

        private void MenuItem_Click_ZhengZhou(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var quoteGrpVw = new ClientQuoteGroupView() { LayoutContent = ancable };
            quoteGrpVw.Filter("CZCE", "", "");
            ancable.Content = quoteGrpVw;
            ancable.Title = WPFUtility.GetLocalizedString("CZCE", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            quotePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Execution(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var executionWin = new ClientExecutionWindow() { LayoutContent = ancable };
            ancable.Content = executionWin;
            ancable.Title = WPFUtility.GetLocalizedString("AllExecution", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            executionPane.Children.Add(ancable);
        }

        private void MenuItem_Click_Opening(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var executionWin = new ClientExecutionWindow() { LayoutContent = ancable };
            ancable.Content = executionWin;
            executionWin.FilterByStatus(new List<OrderStatus> { OrderStatus.OPENNING, OrderStatus.PARTIAL_TRADED, OrderStatus.PARTIAL_TRADING });
            ancable.Title = WPFUtility.GetLocalizedString("Opening", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            executionPane.Children.Add(ancable);
        }

        private void MenuItem_Click_Traded(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var executionWin = new ClientExecutionWindow() { LayoutContent = ancable };
            ancable.Content = executionWin;
            executionWin.FilterByStatus(new List<OrderStatus> { OrderStatus.ALL_TRADED });
            ancable.Title = WPFUtility.GetLocalizedString("Traded", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            executionPane.Children.Add(ancable);
        }

        private void MenuItem_Click_Trade(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var tradeWin = new ClientTradeWindow() { LayoutContent = ancable };
            ancable.Content = tradeWin;
            ancable.Title = WPFUtility.GetLocalizedString("AllTraded", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Open(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var tradeWin = new ClientTradeWindow() { LayoutContent = ancable };
            ancable.Content = tradeWin;
            tradeWin.FilterByStatus(new List<OrderOffsetType> { OrderOffsetType.OPEN });
            ancable.Title = WPFUtility.GetLocalizedString("Open", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Close(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var tradeWin = new ClientTradeWindow() { LayoutContent = ancable };
            ancable.Content = tradeWin;
            tradeWin.FilterByStatus(new List<OrderOffsetType> { OrderOffsetType.CLOSE });
            ancable.Title = WPFUtility.GetLocalizedString("Close", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            tradePane.Children.Add(ancable);
        }

        private void MenuItem_Click_Position(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable ancable = new LayoutAnchorable();
            var positionWin = new ClientPositionWindow() { LayoutContent = ancable };
            ancable.Content = positionWin;
            ancable.Title = WPFUtility.GetLocalizedString("Position", RESOURCE_FILE, CST_CONTROL_ASSEMBLY);
            positionPane.Children.Add(ancable);
        }


        private void FastOrder_Click(object sender, RoutedEventArgs e)
        {
            FastOrderWin fastOrderWindow = new FastOrderWin();
            fastOrderWindow.Show();
            otcMarketDataLV.OnQuoteSelected += fastOrderWindow.OnQuoteSelected;
            positionsWindow.OnPositionSelected += fastOrderWindow.OnPositionSelected;
        }

    }
}
