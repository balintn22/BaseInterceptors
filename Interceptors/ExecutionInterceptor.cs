using Castle.DynamicProxy;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Interceptors
{
    public class ExecutionInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            OnEntry(invocation);
            invocation.Proceed();
            var methodInfo = invocation.MethodInvocationTarget;
            if (IsAsync(methodInfo) && typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
            {
                invocation.ReturnValue = InterceptAsync((dynamic)invocation.ReturnValue, invocation);
            }
            else
            {   // Do continuation work for sync
                OnExit(invocation);
            }
        }

        private async Task InterceptAsync(Task task, IInvocation invocation)
        {
            await task.ConfigureAwait(false);
            // do the continuation work for Task...
            OnExit(invocation);
        }

        private async Task<T> InterceptAsync<T>(Task<T> task, IInvocation invocation)
        {
            T result = await task.ConfigureAwait(false);
            // do the continuation work for Task<T>...
            OnExit(invocation);
            return result;
        }

        private bool IsAsync(MethodInfo methodInfo)
        {
            return methodInfo.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null;
        }

        #region Logic

        public ExecutionInterceptor()
        {
        }

        protected virtual void OnEntry(IInvocation invocation)
        {
        }

        protected virtual void OnExit(IInvocation invocation)
        {
        }

        #endregion Logic
    }
}
