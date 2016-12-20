using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Threading.Tasks
{
    public static class TaskExtensions
    {
        //10000ms
        public const int DEFAULT_TIMEOUT = 10000; 
        public static Task<bool> WaitAsync(this Task task, TimeSpan timeout)
        {
            return Task.Run(() => { return task.Wait(timeout); });
        }

        public static Task<bool> WaitAsync(this Task task, int timeout_millisecs = DEFAULT_TIMEOUT)
        {
            return Task.Run(() => { return task.Wait(timeout_millisecs); });
        }
    }
}
