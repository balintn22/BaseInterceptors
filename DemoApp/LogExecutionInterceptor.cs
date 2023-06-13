using BaseInterceptors;
using Castle.DynamicProxy;
using System;

namespace DemoApp
{
    public class LogExecutionInterceptor : ExecutionInterceptorSync
    {
        public override void OnEntry(IInvocation invocation)
        {
            Console.WriteLine($"Execution interceptor: Entering {InvocationHelper.GetFullMethodName(invocation)}");
        }

        public override void OnExit(IInvocation invocation)
        {
            Console.WriteLine($"Execution interceptor: Exiting {InvocationHelper.GetFullMethodName(invocation)}");
        }
    }
}
