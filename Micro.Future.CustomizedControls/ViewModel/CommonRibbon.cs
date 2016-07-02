using Micro.Future.UI;
using Micro.Future.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micro.Future.ViewModel
{

    public static class CommonRibbon
    {
        #region Data

        private const string HelpFooterTitle = "Press F1 for more help.";
        private static object _lockObject = new object();
        private static Dictionary<string, ControlData> _dataCollection = new Dictionary<string, ControlData>();

        // Store any data that doesnt inherit from ControlData
        private static Dictionary<string, object> _miscData = new Dictionary<string, object>();

        #endregion Data

        public static string ResourceAssembly
        {
            get
            {
                return typeof(ViewModelBase).Assembly.FullName;
            }
        }

        public static void LoadNewQuoteWin()
        {

        }

        public static ControlData NewQuote
        {
            get
            {
                lock (_lockObject)
                {
                    string Str = "添加";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        MenuItemData menuItemData = new MenuItemData()
                        {
                            Label = Str,
                            LargeImage = new Uri("/" + ResourceAssembly + ";component/Images/077_AddFile_48x48_72.png", UriKind.Relative),
                            Command = new DelegateCommand(LoadNewQuoteWin),
                            ToolTipDescription = "添加行情到当前报价组",
                            ToolTipTitle = "报价",
                        };
                        _dataCollection[Str] = menuItemData;
                    }

                    return _dataCollection[Str];
                }
            }
        }
    }
}
