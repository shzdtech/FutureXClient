using System;
using System.ComponentModel;
using System.Windows;

namespace Micro.Future.Windows
{

    public partial class VolModelSettingsWindow : Window
    {
        public event Action<string> OnNaming;

        public VolModelSettingsWindow()
        {
            InitializeComponent();
        }

        public string VolModelTabTitle
        {
            get
            {
                return titleTxt.Text;
            }
            set { titleTxt.Text = value; }
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            OnNaming?.Invoke(VolModelTabTitle);
        }

        
    }
}
