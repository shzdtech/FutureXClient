using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WPFLocalizeExtension.Extensions;

namespace Micro.Future.UI
{
    public static class WPFUtility
    {
        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item 
            DependencyObject parentObject = VisualTreeHelper.GetParent(child); 
            //we've reached the end of the tree 
            if (parentObject == null) return null;
            //check if the parent matches the type we’re looking for 
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

        public static string GetLocalizedString(string key, string resources = "Resources", string assembly = null)
        {
            return GetLocalizedValue<string>(key, resources, assembly);
        }

        public static T GetLocalizedValue<T>(string key, string resources = "Resources", string assembly = null)
        {
            if (assembly == null) assembly = Assembly.GetCallingAssembly().GetName().Name;
            return LocExtension.GetLocalizedValue<T>(assembly + ":" + resources + ":" + key);
        }
    }
}
