using Micro.Future.ViewModel;
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

namespace Micro.Future.UI
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class OptionModelCtrl : UserControl
    {
        private VolCurvCtrl _volCurvCtrl = new VolCurvCtrl();
        private OpMarketData _opMarketData = new OpMarketData();
        public OptionModelCtrl()
        {
            InitializeComponent();
            OpMarketControl.underlyingContractCB1.SelectionChanged += UnderlyingContractCB1_SelectionChanged;        }

        private void UnderlyingContractCB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ModelParamsVM modelParams = new ModelParamsVM();
            var uc = OpMarketControl.underlyingContractCB1.SelectedItem;
            _volCurvCtrl.SelectOption(uc.ToString());
            //var modelparamsVM = await OTCHandler?.QueryModelParamsAsync(modelParams.Model);
            //VolatilityPanel = modelParams.Params;
        }
    }
}
