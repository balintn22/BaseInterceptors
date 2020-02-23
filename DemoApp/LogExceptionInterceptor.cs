using BaseInterceptors;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoApp
{
    public class LogExceptionInterceptor : ExceptionInterceptorSync
    {
        protected override object OnException(IInvocation invocation, Exception ex)
        {
            var method = GetMethodName(invocation);
            Console.WriteLine($"Exception interceptor: {method} failed.\r\nReturn value: {invocation.ReturnValue}.\r\nException: {ex}");
            // Set method invocation return value to something designating an error.
            return -1;
        }

        private string GetMethodName(IInvocation invocation)
        {
            return $"{invocation.InvocationTarget.GetType().Name}.{invocation.Method.Name}()";
        }
    }
}
