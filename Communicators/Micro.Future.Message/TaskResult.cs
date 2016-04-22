using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Micro.Future.Message
{
    public class TaskResult<TResult, TError>
    {
        public TaskResult(TResult result, TError err)
        {
            Result = result;
            Error = err;
        }
        public TaskResult(TResult result)
        {
            Result = result;
        }
        public TaskResult(TError err)
        {
            Error = err;
        }
        public TResult Result { get; protected set; }
        public TError Error { get; protected set; }
    }
}
