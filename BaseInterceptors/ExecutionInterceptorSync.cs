using Castle.DynamicProxy;
using System.Threading.Tasks;

namespace BaseInterceptors
{
    /// <summary>
    /// Implements an interceptor that detects method entry and exit.
    /// Whenever a method starts or finishes execution, the overridable OnEntry() and OnExit() methods are called.
    /// OnEntry() and OnExit() are implemented as synchronous methods,
    /// regardless of the intercepted method.
    /// </summary>
    public abstract class ExecutionInterceptorSync : IInterceptor
    {
        #region Implement IInterceptor

        public void Intercept(IInvocation invocation)
        {
            var methodInfo = invocation.MethodInvocationTarget;
            OnEntry(invocation);
            invocation.Proceed();
            if (AsyncHelper.IsAsync(methodInfo))
                invocation.ReturnValue = InterceptAsync((dynamic)invocation.ReturnValue, invocation);
            else
                InterceptSync(invocation);
        }

        private async Task InterceptAsync(Task task, IInvocation invocation)
        {
            await task.ConfigureAwait(false);
            // Do the continuation work for Task
            OnExit(invocation);
        }

        private async Task<T> InterceptAsync<T>(Task<T> task, IInvocation invocation)
        {
            T result = await task.ConfigureAwait(false);
            // Do the continuation work for Task<T>
            OnExit(invocation);
            return result;
        }

        private void InterceptSync(IInvocation invocation)
        {
            // Do the continuation work for a sync method
            OnExit(invocation);
        }

        #endregion Implement IInterceptor

        
        #region Logic

        protected abstract void OnEntry(IInvocation invocation);

        protected abstract void OnExit(IInvocation invocation);

        #endregion Logic
    }
}
