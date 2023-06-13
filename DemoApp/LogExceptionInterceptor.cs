using BaseInterceptors;
using Castle.DynamicProxy;
using System;

namespace DemoApp
{
    public class LogExceptionInterceptor : ExceptionInterceptorSync
    {
        public override object OnException(IInvocation invocation, Exception ex)
        {
            Console.WriteLine($"Exception interceptor: {InvocationHelper.GetFullMethodName(invocation)} failed.\r\n" +
                $"Return value: {invocation.ReturnValue}.\r\nException: {ex}");
            // Set method invocation return value to something designating an error.
            return -1;
        }
    }
}
