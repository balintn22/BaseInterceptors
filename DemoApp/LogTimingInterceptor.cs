using BaseInterceptors;
using Castle.DynamicProxy;
using System;

namespace DemoApp
{
    /// <summary>
    /// Implements a TimingInterceptor, that - when a method execution is finished -
    /// logs execution time to the console.
    /// </summary>
    public class LogTimingInterceptor : TimingInterceptorSync
    {
        public override void OnCompleted(IInvocation invocation, TimeSpan executionTime)
        {
            Console.WriteLine($"Timing interceptor: {InvocationHelper.GetFullMethodName(invocation)} " +
                $"execution time: {executionTime.TotalMilliseconds} msecs");
        }
    }
}
