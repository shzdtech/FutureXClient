using Micro.Future.LocalStorage;
using Micro.Future.LocalStorage.DataObject;
using Micro.Future.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Micro.Future.Simulator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private AbstractSignInManager _simSignIner = new PBSignInManager(MessageHandlerContainer.GetSignInOptions<SimMarketDataHandler>());
        private IDictionary<string, Queue<MarketDataDO>> _simDataDict = new Dictionary<string, Queue<MarketDataDO>>();
        private Timer _timer;
        private uint _counter;
        private bool _sending;

        public MainWindow()
        {
            InitializeComponent();

            SimMarketDataHandler.Instance.RegisterMessageWrapper(_simSignIner.MessageWrapper);

            _simSignIner.OnLogged += LoginStatus.OnLogged;
            _simSignIner.OnLoginError += LoginStatus.OnDisconnected;

            _simSignIner.SignIn();

            LoadSimMarketData();
        }

        void LoadSimMarketData()
        {
            using (var ctx = new SimulatorDbContext())
            {
                try
                {
                    var mdList = ctx.MarketData;

                    foreach (var md in mdList)
                    {
                        Queue<MarketDataDO> mdQueue;
                        if (!_simDataDict.TryGetValue(md.Contract, out mdQueue))
                        {
                            mdQueue = new Queue<MarketDataDO>();
                            _simDataDict[md.Contract] = mdQueue;
                        }
                        mdQueue.Enqueue(md);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.ToString());
                }
            }
        }

        private void buttonSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (!_sending)
            {
                _sending = true;
                buttonSwitch.Content = "Sending";
                int interval = int.Parse(textBoxInterval.Text) * 1000;
                _timer = new Timer(SendingSimDataCallback, null, interval, interval);
            }
            else
            {
                _sending = false;
                _timer?.Dispose();
                buttonSwitch.Content = "Start";
            }
        }

        private void SendingSimDataCallback(object state)
        {
            foreach (var pair in _simDataDict)
            {
                var queue = pair.Value;
                var mdo = queue.Dequeue();
                queue.Enqueue(mdo);
                SimMarketDataHandler.Instance.SendSimMarketData(mdo);
            }

            Dispatcher.Invoke(() => statisticsTB.Text = string.Format("We have sent sim data for {0} times.", ++_counter));
        }

        private void LoginStatus_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _simSignIner.SignIn();
        }
    }
}
