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
        }
        private Process _serverProcess;
        private void Update_Button_Click(object sender, RoutedEventArgs e)
        {
            AutoUpdater.Start("http://localhost:63321/Client/AutoUpdater.xml");
            _serverProcess = new Process();
            _serverProcess.StartInfo.FileName = @"D:\project\FutureXClient\Micro.Future.ClientUI\bin\Debug\Micro.Future.ClientUI.exe";
            _serverProcess.StartInfo.WorkingDirectory = @"D:\project\FutureXClient\Micro.Future.ClientUI\bin\Debug";
            _serverProcess.Start();
        }
    }
}
