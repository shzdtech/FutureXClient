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
using AutoUpdaterDotNET;
using System.Diagnostics;
using System.IO;
using System.Reflection;
//using System.Windows.Forms;

namespace Micro.Future.AutoUpdate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AutoUpdater.CheckForUpdateEvent += AutoUpdaterOnCheckForUpdateEvent;
            AutoUpdater.Start("http://localhost:63321/Client/AutoUpdater.xml");
        }


        private void LauchMainApp()
        {
            var workingDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var mainApp = new Process();
            mainApp.StartInfo.UseShellExecute = true;
            mainApp.StartInfo.FileName = System.IO.Path.Combine(workingDir, "Micro.Future.ClientUI.exe");
            mainApp.StartInfo.WorkingDirectory = workingDir;
            mainApp.Start();
        }

        private void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args != null)
            {
                if (args.IsUpdateAvailable)
                {
                    AutoUpdater.CheckForUpdateEvent -= AutoUpdaterOnCheckForUpdateEvent;
                    AutoUpdater.Start("http://localhost:63321/Client/AutoUpdater.xml");
                    LauchMainApp();
                }
                else
                {
                    LauchMainApp();
                }
            }
            else
            {
                LauchMainApp();
            }
        }

    }
}
