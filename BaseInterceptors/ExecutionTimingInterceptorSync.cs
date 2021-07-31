using Castle.DynamicProxy;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BaseInterceptors
{
    /// <summary>
    /// Implements an interceptor that detects method entry and exit,
    /// creates a stopwatch on entry, an passes it to the OnExit method.
    /// Whenever a method starts or finishes execution, the overridable OnEntry() and OnExit() methods are called.
    /// OnEntry() and OnExit() are implemented as synchronous methods,
    /// regardless of the intercepted method.
    /// </summary>
    public abstract class ExecutionTimingInterceptorSync : IInterceptor
    {
        #region Implement IInterceptor

        public void Intercept(IInvocation invocation)
        {
            var methodInfo = invocation.MethodInvocationTarget;
            var stopwatch = Stopwatch.StartNew();
            OnEntry(invocation);
            invocation.Proceed();
            if (AsyncHelper.IsAsync(methodInfo))
                invocation.ReturnValue = InterceptAsync((dynamic)invocation.ReturnValue, invocation, stopwatch);
            else
                InterceptSync(invocation, stopwatch);
        }

        private async Task InterceptAsync(Task task, IInvocation invocation, Stopwatch stopwatch)
        {
            await task.ConfigureAwait(false);
            // Do the continuation work for Task
            OnExit(invocation, stopwatch);
        }

        private async Task<T> InterceptAsync<T>(Task<T> task, IInvocation invocation, Stopwatch stopwatch)
        {
            T result = await task.ConfigureAwait(false);
            // Do the continuation work for Task<T>
            OnExit(invocation, stopwatch);
            return result;
        }

        private void InterceptSync(IInvocation invocation, Stopwatch stopwatch)
        {
            // Do the continuation work for a sync method
            OnExit(invocation, stopwatch);
        }

        #endregion Implement IInterceptor

        
        #region Logic

        protected abstract void OnEntry(IInvocation invocation);

        protected abstract void OnExit(IInvocation invocation, Stopwatch stopwatch);

        #endregion Logic
    }
}
