using Castle.DynamicProxy;
using System.Threading.Tasks;

namespace BaseInterceptors
{
    /// <summary>
    /// Implements an interceptor that detects method entry and exit.
    /// Whenever a method starts or finishes execution, the overridable OnEntry() and OnExit() methods are called.
    /// OnEntry() and OnExit() are implemented both as a synchronous and as an asynchronous method,
    /// called when sync or async methods are intercepted, respectively.
    /// </summary>
    public abstract class ExecutionInterceptor : IInterceptor
    {
        #region Implement IInterceptor

        public void Intercept(IInvocation invocation)
        {
            var methodInfo = invocation.MethodInvocationTarget;
            if (AsyncHelper.IsAsync(methodInfo))
            {
                Task.Run(() => OnEntryAsync(invocation));
                invocation.Proceed();
                invocation.ReturnValue = InterceptAsync((dynamic)invocation.ReturnValue, invocation);
            }
            else
            {
                OnEntry(invocation);
                invocation.Proceed();
                InterceptSync(invocation);
            }
        }

        private async Task InterceptAsync(Task task, IInvocation invocation)
        {
            await task.ConfigureAwait(false);
            // Do the continuation work for Task
            await OnExitAsync(invocation);
        }

        private async Task<T> InterceptAsync<T>(Task<T> task, IInvocation invocation)
        {
            T result = await task.ConfigureAwait(false);
            // Do the continuation work for Task<T>
            await OnExitAsync(invocation);
            return result;
        }

        private void InterceptSync(IInvocation invocation)
        {
            // Do the continuation work for a sync method
            OnExit(invocation);
        }

        #endregion Implement IInterceptor


        #region Logic

        public abstract void OnEntry(IInvocation invocation);

        public abstract Task OnEntryAsync(IInvocation invocation);

        public abstract void OnExit(IInvocation invocation);

        public abstract Task OnExitAsync(IInvocation invocation);

        #endregion Logic
    }
}
