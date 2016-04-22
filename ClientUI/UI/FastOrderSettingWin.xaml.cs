using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Micro.Future.ViewModel;

namespace Micro.Future.UI
{
    /// <summary>
    /// FastOrderSettingWin.xaml 的交互逻辑
    /// </summary>
    public partial class FastOrderSettingWin : Window
    {
        private KeyboardOrderViewModel clonedvm;

        public FastOrderSettingWin()
        {
            InitializeComponent();
        }

        public void SetDataContext(KeyboardOrderViewModel viewModel)
        {
            clonedvm = (KeyboardOrderViewModel)viewModel.Clone();
            DataContext = clonedvm;

            switch (clonedvm.CloseChoice)
            {
                case CloseChoiceType.All:
                    radio1.IsChecked = true;
                    break;
                case CloseChoiceType.IgnorePlus:
                    radio2.IsChecked = true;
                    break;
                case CloseChoiceType.PlusOpen:
                    radio3.IsChecked = true;
                    break;
                default:
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (radio1.IsChecked == true)
            {
                clonedvm.CloseChoice = CloseChoiceType.All;
            }

            if (radio2.IsChecked == true)
            {
                clonedvm.CloseChoice = CloseChoiceType.IgnorePlus;
            }

            if (radio3.IsChecked == true)
            {
                clonedvm.CloseChoice = CloseChoiceType.PlusOpen;
            }

            MainWindow.MyInstance.KeyOrderViewModel = clonedvm;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Check2.IsChecked = true;
            Check2.IsEnabled = false;
        }

        private void Check1_Unchecked(object sender, RoutedEventArgs e)
        {
            Check2.IsEnabled = true;
        }
    }
}
