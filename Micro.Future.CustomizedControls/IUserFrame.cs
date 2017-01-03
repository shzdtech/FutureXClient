using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Micro.Future.CustomizedControls
{
    public interface IUserFrame
    {
        string Title { get; }
        IEnumerable<MenuItem> FrameMenus { get; }

        IEnumerable<StatusBarItem> StatusBarItems { get; }

        IStatusCollector StatusReporter { get; set; }

        TaskCompletionSource<bool> LoginTaskSource { get; }

        Task<bool> LoginAsync(string brokerId, string usernname, string password, string server = null);
    }
}
