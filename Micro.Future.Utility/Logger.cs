using System;
using System.Text;
using System.IO;
using log4net;
using System.Reflection;

namespace Micro.Future.Utility
{
    //wrapper for log by Lucas, Jun 5th, 2013
    public class Logger
    {
        private Logger()
        {
        }

        private static readonly ILog loginfo = LogManager.GetLogger("log");

        //invoke before any log
        public static void StartLog(string config)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(config));
        }

        public static void Info(string info)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(info);
            }
        }

        public static void Debug(string info)
        {
            if (loginfo.IsDebugEnabled)
            {
                loginfo.Debug(info);
            }
        }

        public static void Warn(string info)
        {
            if (loginfo.IsWarnEnabled)
            {
                loginfo.Warn(info);
            }
        }

        public static void Error(string info)
        {
            if (loginfo.IsErrorEnabled)
            {
                loginfo.Error(info);
            }
        }


        public static void Dump<T>(T obj)
        {
            Type t = typeof(T);
            StringBuilder sb = new StringBuilder();
            PropertyInfo[] properties = t.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                sb.Append("[" + property.Name + "]" + property.GetValue(obj, null).ToString() + ", ");
            }

            Debug(sb.ToString());
        }
    }
}
