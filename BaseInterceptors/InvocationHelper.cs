using Castle.DynamicProxy;
using System.Threading.Tasks;

namespace BaseInterceptors
{
    public static class InvocationHelper
    {
        public static void SetReturnValue<TValue>(IInvocation invocation, TValue value)
        {
            if (AsyncHelper.IsAsync(invocation.Method))
            {
                var returnValue = (Task<TValue>)invocation.ReturnValue;
                invocation.ReturnValue = returnValue.ContinueWith(
                    t => value,
                    // TODO: Should any one of these picked for general use?
                    // Or should it be configurable by the aspect implementer?
                    // Does it have any effect on the success of setting a return value?

                    //TaskContinuationOptions.AttachedToParent
                    //TaskContinuationOptions.TaskContinuationOptions
                    TaskContinuationOptions.ExecuteSynchronously
                    //TaskContinuationOptions.HideScheduler
                    //TaskContinuationOptions.LazyCancellation
                    //TaskContinuationOptions.LongRunning
                    //TaskContinuationOptions.None
                    //TaskContinuationOptions.NotOnCanceled
                    //TaskContinuationOptions.NotOnFaulted
                    //TaskContinuationOptions.NotOnRanToCompletion
                    //TaskContinuationOptions.OnlyOnCanceled
                    //TaskContinuationOptions.OnlyOnFaulted
                    //TaskContinuationOptions.OnlyOnRanToCompletion
                    //TaskContinuationOptions.PreferFairness
                    //TaskContinuationOptions.RunContinuationsAsynchronously
                );
            }
            else
            {
                invocation.ReturnValue = value;
            }
        }
    }
}
