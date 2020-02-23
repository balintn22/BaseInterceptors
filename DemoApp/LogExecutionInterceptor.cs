using BaseInterceptors;
using Castle.DynamicProxy;
using System;

namespace DemoApp
{
    public class LogExecutionInterceptor : ExecutionInterceptorSync
    {
        protected override void OnEntry(IInvocation invocation)
        {
            Console.WriteLine($"Execution interceptor: Entering {GetMethodName(invocation)}");
        }

        protected override void OnExit(IInvocation invocation)
        {
            Console.WriteLine($"Execution interceptor: Exiting {GetMethodName(invocation)}");
        }

        private string GetMethodName(IInvocation invocation)
        {
            return $"{invocation.InvocationTarget.GetType().Name}.{invocation.Method.Name}()";
        }
    }
}
