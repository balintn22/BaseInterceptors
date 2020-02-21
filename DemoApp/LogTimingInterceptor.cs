using Castle.DynamicProxy;
using Interceptors;
using System;

namespace DemoApp
{
    /// <summary>
    /// Implements a TimingInterceptor, that - when a method execution is finished -
    /// logs execution time to the console.
    /// </summary>
    public class LogTimingInterceptor : TimingInterceptorSync
    {
        protected override void OnCompleted(IInvocation invocation, TimeSpan executionTime)
        {
            string methodName = GetMethodName(invocation);
            Console.WriteLine($"{methodName} execution time: {executionTime.TotalMilliseconds} msecs");
        }

        private string GetMethodName(IInvocation invocation)
        {
            return $"{invocation.InvocationTarget.GetType().Name}.{invocation.Method.Name}()";
        }
    }
}
