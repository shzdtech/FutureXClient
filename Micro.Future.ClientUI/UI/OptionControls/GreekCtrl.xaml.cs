using Micro.Future.Utility;
using Micro.Future.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class GreekCtrl : UserControl
    {
        public GreekCtrl()
        {
            InitializeComponent();

        }
        
        public void BindingToSource(ObservableCollection<RiskVM> source)
        {
            GreekListView.ItemsSource = null;
            GreekListView.ItemsSource = source;
            source.Add(new RiskVM { Contract = "a", Underlying = "1", Delta = 1, Gamma = 1, Vega = 1, Theta = 1 });
            source.Add(new RiskVM { Contract = "a", Underlying = "1", Delta = 1, Gamma = 1, Vega = 1, Theta = 1 });
            source.Add(new RiskVM { Contract = "b", Underlying = "2", Delta = 1, Gamma = 1, Vega = 1, Theta = 1 });
            var cvs = CollectionViewSource.GetDefaultView(source);
            if (cvs != null)
            {
                cvs.GroupDescriptions.Add(new PropertyGroupDescription("Underlying"));
            }
        }

        private void GreekListView_Click(object sender, RoutedEventArgs e)
        {
            var head = e.OriginalSource as GridViewColumnHeader;
            if (head != null)
            {
                GridViewUtility.Sort(head.Column, GreekListView.Items);
            }
        }

    }
}
