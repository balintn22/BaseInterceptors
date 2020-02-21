using Castle.DynamicProxy;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Interceptors
{
    /// <summary>
    /// Implements an interceptor that measures method execution time.
    /// Whenever a method finishes execution, the overridable OnCompleted() method is called.
    /// OnCompleted() is implemented both as a synchronous and as an asynchronous method,
    /// called when sync or async methods are intercepted, respectively.
    /// </summary>
    public abstract class TimingInterceptorAsync : IInterceptor
    {
        #region Implement IInterceptor

        public void Intercept(IInvocation invocation)
        {
            StartStopwatch();
            invocation.Proceed();
            var methodInfo = invocation.MethodInvocationTarget;
            if (IsAsync(methodInfo) && typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
            {
                invocation.ReturnValue = InterceptAsync((dynamic)invocation.ReturnValue, invocation);
            }
            else
            {   // Do continuation work for sync
                AsyncHelper.RunSync(() => StopStopwatchAsync(invocation));
            }
        }

        private async Task InterceptAsync(Task task, IInvocation invocation)
        {
            await task.ConfigureAwait(false);
            // do the continuation work for Task...
            await StopStopwatchAsync(invocation);
        }

        private async Task<T> InterceptAsync<T>(Task<T> task, IInvocation invocation)
        {
            T result = await task.ConfigureAwait(false);
            // do the continuation work for Task<T>...
            await StopStopwatchAsync(invocation);
            return result;
        }

        private bool IsAsync(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
        }

        #endregion Implement IInterceptor


        #region Logic

        protected Stopwatch Stopwatch;

        public TimingInterceptorAsync()
        {
            Stopwatch = new Stopwatch();
        }

        private void StartStopwatch()
        {
            Stopwatch.Start();
        }

        protected async Task StopStopwatchAsync(IInvocation invocation)
        {
            Stopwatch.Stop();
            await OnCompletedAsync(invocation, Stopwatch.Elapsed).ConfigureAwait(false);
        }

        /// <summary>
        /// <summary>
        /// In a derived class, implements a handler for the on completion event, used when the intercepted method is asynchronous.
        /// </summary>
        /// <param name="invocation">Contains information about the measured method invocation, use it to fetch method name, etc.</param>
        /// <param name="executionTime">Contains the elapsed time between method execution start and end.</param>
        protected virtual async Task OnCompletedAsync(IInvocation invocation, TimeSpan executionTime)
        {
            throw new NotImplementedException();
        }

        #endregion Logic
    }
}
