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
        private IDictionary<string, Queue<MarketData>> _simDataDict = new Dictionary<string, Queue<MarketData>>();
        private IDictionary<string, Queue<MarketDataOpt>> _simOptDataDict = new Dictionary<string, Queue<MarketDataOpt>>();
        private Timer _timer;
        private uint _counter;
        private bool _sending;

        public MainWindow()
        {
            InitializeComponent();

            SimMarketDataHandler.Instance.RegisterMessageWrapper(_simSignIner.MessageWrapper);

            _simSignIner.OnLogged += LoginStatus.OnLogged;
            _simSignIner.MessageWrapper.MessageClient.OnDisconnected += LoginStatus.OnDisconnected;
            _simSignIner.SignIn();

            LoginStatus.OnConnButtonClick += LoginStatus_OnConnButtonClick;

            Task.Run(()=>LoadData());
            
        }

        private void LoginStatus_OnConnButtonClick(object sender, EventArgs e)
        {
            if (!_simSignIner.MessageWrapper.HasSignIn)
                _simSignIner.SignIn();
        }

        private void LoadData()
        {
            try
            {
                Dispatcher.Invoke(() => buttonSwitch.IsEnabled = false);
                var ctx = new SimulatorDbContext();
                Dispatcher.Invoke(() => statisticsTB.Text = string.Format("Loading MarketData"));
                LoadSimMarketData(ctx.MarketData, _simDataDict);
                Dispatcher.Invoke(() => statisticsTB.Text = string.Format("Loading MarketDataOpt"));
                LoadSimMarketData(ctx.MarketDataOpt, _simOptDataDict);
                Dispatcher.Invoke(() => statisticsTB.Text = string.Format("Sim Data Loaded"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Dispatcher.Invoke(() => buttonSwitch.IsEnabled = true);
            }
        }

        void LoadSimMarketData(IEnumerable<MarketData> mdList, IDictionary<string, Queue<MarketData>> mdQueues)
        {
            try
            {
                mdQueues.Clear();
                foreach (var md in mdList)
                {
                    Queue<MarketData> mdQueue;
                    if (!mdQueues.TryGetValue(md.Contract, out mdQueue))
                    {
                        mdQueue = new Queue<MarketData>();
                        mdQueues[md.Contract] = mdQueue;
                    }
                    mdQueue.Enqueue(md);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
            }
        }

        void LoadSimMarketData(IEnumerable<MarketDataOpt> mdList, IDictionary<string, Queue<MarketDataOpt>> mdQueues)
        {
            try
            {
                mdQueues.Clear();
                foreach (var md in mdList)
                {
                    Queue<MarketDataOpt> mdQueue;
                    if (!mdQueues.TryGetValue(md.Contract, out mdQueue))
                    {
                        mdQueue = new Queue<MarketDataOpt>();
                        mdQueues[md.Contract] = mdQueue;
                    }
                    mdQueue.Enqueue(md);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
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

            foreach (var pair in _simOptDataDict)
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
