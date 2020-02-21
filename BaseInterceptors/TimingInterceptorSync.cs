using Castle.DynamicProxy;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace BaseInterceptors
{
    /// <summary>
    /// Implements an interceptor that measures method execution time.
    /// Whenever a method finishes execution, the overridable OnCompleted() method is called.
    /// OnCompleted() is implemented as a synchronous method, regardless of the intercepted method.
    /// </summary>
    public abstract class TimingInterceptorSync : IInterceptor
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
                StopStopwatch(invocation);
            }
        }

        private async Task InterceptAsync(Task task, IInvocation invocation)
        {
            await task.ConfigureAwait(false);
            // do the continuation work for Task...
            StopStopwatch(invocation);
        }

        private async Task<T> InterceptAsync<T>(Task<T> task, IInvocation invocation)
        {
            T result = await task.ConfigureAwait(false);
            // do the continuation work for Task<T>...
            StopStopwatch(invocation);
            return result;
        }

        private bool IsAsync(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
        }

        #endregion Implement IInterceptor


        #region Logic

        protected Stopwatch Stopwatch;

        public TimingInterceptorSync()
        {
            Stopwatch = new Stopwatch();
        }

        private void StartStopwatch()
        {
            Stopwatch.Start();
        }

        protected void StopStopwatch(IInvocation invocation)
        {
            Stopwatch.Stop();
            OnCompleted(invocation, Stopwatch.Elapsed);
        }

        /// <summary>
        /// In a derived class, implements a handler for the on completion event.
        /// </summary>
        /// <param name="invocation">Contains information about the measured method invocation, use it to fetch method name, etc.</param>
        /// <param name="executionTime">Contains the elapsed time between method execution start and end.</param>
        protected abstract void OnCompleted(IInvocation invocation, TimeSpan executionTime);

        #endregion Logic
    }
}
