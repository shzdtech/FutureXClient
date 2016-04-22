using Micro.Future.Util;
using Micro.Future.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Micro.Future.Controls
{
    /// <summary>
    /// LoginStatus.xaml 的交互逻辑
    /// </summary>
    public partial class LoginStatus : UserControl
    {
        public event EventHandler OnConnButtonClick;

        private ImageVM _loginStatusVM = new ImageVM();
        public LoginStatus()
        {
            InitializeComponent();

            statusIcon.DataContext = _loginStatusVM;

            Connected = false;

            statusIcon.MouseLeftButtonUp += StatusIcon_MouseLeftButtonUp;
        }

        private void StatusIcon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (OnConnButtonClick != null)
                OnConnButtonClick(sender, e);
        }

        public object ConnectedPrompt
        {
            get;
            set;
        }

        object _disconnPrompt;
        public object DisconnectedPrompt
        {
            get
            {
                return _disconnPrompt;
            }
            set
            {
                _disconnPrompt = value;
            }
        }

        public object Prompt
        {
            set { promptLabel.Content = value; }
        }

        private bool _connected;
        public bool Connected
        {
            get
            {
                return _connected;
            }
            set
            {
                _connected = value;
                statusIcon.IsEnabled = !value;

                var componentUri = Utility.GenComponentUri(this.GetType());

                if (value)
                {
                    promptLabel.Content = ConnectedPrompt;
                    _loginStatusVM.SourceUri = componentUri + "/Images/connected_48x48.png";
                    _loginStatusVM.SetImage(_loginStatusVM.SourceUri);
                }
                else
                {
                    promptLabel.Content = DisconnectedPrompt;
                    _loginStatusVM.SourceUri = componentUri + "/Images/disconnected_48x48.png";
                    _loginStatusVM.SetImage(_loginStatusVM.SourceUri);
                }
            }
        }

        public void OnLogged(object obj)
        {
            Dispatcher.Invoke(
                () => { Connected = true; });
        }
        public void OnDisconnected(object obj)
        {
            if(obj != null)
            {
                Dispatcher.Invoke(
                () => { Connected = false; });
            }
        }
    }
}
