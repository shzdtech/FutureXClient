using Micro.Future.CustomizedControls;
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
using System.Windows.Controls.Primitives;

namespace Micro.Future.TradeControls
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class RiskControlFrame : UserControl, IUserFrame
    {
        public RiskControlFrame()
        {
            InitializeComponent();
            LoginTaskSource.TrySetResult(true);
        }

        public IEnumerable<MenuItem> FrameMenus
        {
            get
            {
                return Resources["exMenuItems"] as IEnumerable<MenuItem>;
            }
        }

        public TaskCompletionSource<bool> LoginTaskSource
        {
            get;
        } = new TaskCompletionSource<bool>();

        public IEnumerable<StatusBarItem> StatusBarItems
        {
            get
            {
                return Resources["exStatusBarItems"] as IEnumerable<StatusBarItem>;
            }
        }

        public IStatusCollector StatusReporter
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        public Task<bool> LoginAsync(string brokerId, string usernname, string password, string server = null)
        {
            return LoginTaskSource.Task;
        }

        public void OnClosing()
        {

        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
