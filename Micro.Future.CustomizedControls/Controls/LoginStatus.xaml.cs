﻿using Micro.Future.Utility;
using Micro.Future.ViewModel;
using System;
using System.Media;
using System.Windows.Controls;
using System.Windows.Input;

namespace Micro.Future.CustomizedControls
{
    /// <summary>
    /// LoginStatus.xaml 的交互逻辑
    /// </summary>
    public partial class LoginStatus : UserControl
    {
        public event EventHandler OnConnButtonClick;

        private string _lastErrorMsg;

        private ImageVM _loginStatusVM = new ImageVM();

        public LoginStatus()
        {
            InitializeComponent();

            statusIcon.DataContext = _loginStatusVM;

            Connected = false;

            MouseUp += LoginStatus_MouseUp;
        }

        private void LoginStatus_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OnConnButtonClick?.Invoke(sender, e);
        }

        public SystemSound DisconnectSound
        {
            get;
            set;
        } = SystemSounds.Exclamation;


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

                var componentUri = MFUtilities.GenComponentUri(this.GetType());

                if (value)
                {
                    promptLabel.Content = ConnectedPrompt;
                    _loginStatusVM.SourceUri = componentUri + "/Images/connected_48x48.png";
                    _loginStatusVM.SetImage(_loginStatusVM.SourceUri);
                }
                else
                {
                    promptLabel.Content = _lastErrorMsg != null ? _lastErrorMsg : DisconnectedPrompt;
                    _loginStatusVM.SourceUri = componentUri + "/Images/disconnected_48x48.png";
                    _loginStatusVM.SetImage(_loginStatusVM.SourceUri);
                }
            }
        }

        public void OnLogged(object obj)
        {
            Connected = true;
        }

        public void OnDisconnected(object obj)
        {
            if (obj != null)
            {
                var ex = obj as Exception;
                _lastErrorMsg = ex != null ? ex.Message : null;

                Connected = false;
                DisconnectSound?.Play();
            }
        }
    }
}
