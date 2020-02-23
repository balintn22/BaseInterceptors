using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace BaseInterceptors
{
    /// <summary>
    /// Implements an interceptor that catches exceptions.
    /// Whenever an intercepted method throws an exepction, the OnException() or OnExceptionAsync()
    /// method are called matching the sync/async type of the intercepted method.
    /// The finally bit could contain business logic, so it shouldn't be part of the error
    /// handler aspect, no support is implemented for that.
    /// </summary>
    public abstract class ExceptionInterceptor : IInterceptor
    {
        #region Implement IInterceptor

        public void Intercept(IInvocation invocation)
        {
            var methodInfo = invocation.MethodInvocationTarget;
            if (AsyncHelper.IsAsync(methodInfo))
            {
                invocation.Proceed();
                invocation.ReturnValue = InterceptAsync((dynamic)invocation.ReturnValue, invocation);
            }
            else
            {
                InterceptSync(invocation);
            }
        }

        private async Task InterceptAsync(Task task, IInvocation invocation)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Do the continuation work for Task
                invocation.ReturnValue = await OnExceptionAsync(invocation, ex);
            }
        }

        private async Task<TResult> InterceptAsync<TResult>(Task<TResult> task, IInvocation invocation)
        {
            try
            {
                TResult result = await task.ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                // Do the continuation work for Task<T>
                return (TResult)(await OnExceptionAsync(invocation, ex));
            }
        }

        private void InterceptSync(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (Exception ex)
            {
                // Do the continuation work for a sync method
                invocation.ReturnValue = OnException(invocation, ex);
            }
        }

        #endregion Implement IInterceptor

        
        #region Logic

        /// <summary>
        /// In a derived class, handles exceptions in intercepted method execution.
        /// The value returned by this method will be set as the return value of the intercepted method.
        /// This method is executed in case the intercepted method is synchronous.
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected abstract object OnException(IInvocation invocation, Exception ex);

        /// <summary>
        /// In a derived class, handles an exception occuring during execution of the intercepted method.
        /// The value returned by this method will be set as the return value of the intercepted method.
        /// This method is executed in case the intercepted method is asynchronous.
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected abstract Task<object> OnExceptionAsync(IInvocation invocation, Exception ex);

        #endregion Logic
    }
}
