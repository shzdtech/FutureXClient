namespace System.Threading.Tasks
{
    public class TimeoutTaskCompletionSource<T> : TaskCompletionSource<T>
    {
        public const int DEFAULT_TIMEOUT = 10000;
        public TimeoutTaskCompletionSource(int timeout = DEFAULT_TIMEOUT)
        {
            if (timeout > 0)
                Tasks.Task.Run(()=>
                {
                    int ret = Tasks.Task.WaitAny(new[] { Task }, timeout);
                    if (ret < 0)
                    {
                        TrySetException(new TimeoutException());
                    }
                });
        }
    }
}
