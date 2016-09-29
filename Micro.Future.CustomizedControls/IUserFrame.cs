using System;
using System.Collections.Generic;
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

        void LoginAsync(string usernname, string password, string server = null);
    }
}
