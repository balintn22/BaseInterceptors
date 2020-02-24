# BaseInterceptors
Provides simple base interceptors for a number of everyday interception scenarios like execution timing, method entry/exit logging or exception handling.

# Concept
Implementing an interceptor should be easy, as easy as providing a single method of what to do in case of a certain code aspect.
This library helps to do that.
Interception scenarios implemented so far:
 - method entry/exit interception
 - method timing interception
 - exception interception

Each scenario is supported by an abstract, base implementation, with the fewest possible overridable methods, i.e. OnCompleted(IInvocation invocation, TimeSpan elapsed) for the timing interceptor.
Each interceptor type has 3 base implementations, i.e. TimingInterceptor, TimingInterceptorSync and TimingInterceptorAsync. The implementation called ...Sync exposes a sinlge, synchronous method to override, ...Async exposes a single, async method to override, ... (without a suffixÖ exposes both a sync and an async method to override. The ...Sync interceptor should be used if the aspect is CPU intensive, ...Async should be used if the aspect is IO intensive - just as picking the sync or async versions of a method. The third (suffix-less) implementation uses sync callbacks for sync intercepted methods and async callbacks for async intercepted methods.

These base interceptors are designed to work in a software envirnment where Castle Windsor is used to manage dependency injection. Interceptors can be applied to classes using the Interceptor attribute.
Another approach (using Castle Windsor component registrations with interceptors) is not yet complete.

# How to Use
If you need an interceptor, chose one of the base interceptors in this package:
 - to handle exceptions, start from ExceptionInterceptor
 - to do something on method entry / exit, start from ExecutionInterceptor
 - to measure method execution time, start from TimingInterceptor
 - if you have a different scenario, feel free to use the the source code of those interceptors to implement your own (and let me know about your scenario!)
 
Let's say, you go with TimingInterceptor.

Now, decide whether you want to implement intercepting code to be sync, async or mixed - use the appropriate implementation of the interceptor you selected above.
Let's say you want to implement your intercepting logic as sync - so you'll derive from TimingInterceptorSync.

Now, create a new class code file and type this much:
public class MyTimingInterceptor _: TimingInterceptorSync_
{
    _override_
}
Visual Studio will show you the overridables (OnCompleted in this case) - all you need to do is implement those methods (all of them).

That's it, you implemented an interceptor!

# Examples
## Example 1
Scenario: there is an application with a class, whose methods' execution time needs to be measured and displayed on the console output. The application uses dependency injection (DI) to create instances of this class. The application targets .Net Core 2.2+ or .Net Framework 4.5+.
Steps to implement the timing aspect:
 1. Add a reference to the BaseInterceptors package.
 2. Within your application add a new class like this:
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
            protected override void OnCompleted(IInvocation invocation, TimeSpan executionTime)
            {
                string methodName = GetMethodName(invocation);
                Console.WriteLine($"Timing interceptor: {methodName} execution time: {executionTime.TotalMilliseconds} msecs");
            }
    
            private string GetMethodName(IInvocation invocation)
            {
                return $"{invocation.InvocationTarget.GetType().Name}.{invocation.Method.Name}()";
            }
        }
    }
 3. Apply the interceptor to the class
    [Interceptor(typeof(LogTimingInterceptor))]
    public class Business : IBusiness
    {
      ...
    }
 4. Add the interceptor to your DI registration code
    container.Register(Component.For<LogTimingInterceptor>().ImplementedBy<LogTimingInterceptor>().Named(nameof(LogTimingInterceptor)).LifestyleTransient());

 ... and you're done. Deriving your interceptor from TimingInterceptorSync means that for both sync and async intercepted methods your OnCompleted() method will be invoked in a synchronous fashion. If - instead of outputting a line to the console - you want to log execution time to a remote service using an async API, derive your interceptor from TimingInterceptorAsync, and override the OnCompletedAsync() method.

## Example 2
Scenario: there is an application with a class, whose methods' execution (entry and exit) needs to be logged to a remote service using an async API. The application uses dependency injection (DI) to create instances of this class. The application targets .Net Core 2.2+ or .Net Framework 4.5+. Assume that there is a logger, registered as a DI dependency (class Logger that implements ILogger).
To implement the logging aspect, steps 1, 3 and 4 are the same as above. In step 2, the interceptor class would look like this:
    using BaseInterceptors;
    using Castle.DynamicProxy;
    using System;
    
    namespace DemoApp
    {
        public class LogExecutionInterceptor : ExecutionInterceptorAsync
        {
            private ILogger _logger;
            
            public LogExecutionInterceptor(ILogger logger)
            {
                _logger = logger;
            }
            
            protected override void OnEntryAsync(IInvocation invocation)
            {
                _logger.LogEntry($"Execution interceptor: Entering {GetMethodName(invocation)}");
            }
    
            protected override void OnExitAsync(IInvocation invocation)
            {
                _logger.LogExit($"Execution interceptor: Exiting {GetMethodName(invocation)}");
            }
    
            private string GetMethodName(IInvocation invocation)
            {
                return $"{invocation.InvocationTarget.GetType().Name}.{invocation.Method.Name}()";
            }
        }
    }
  ... and you're done.
  
## Example 3
Scenario: there is an application with a class, representing an API, where you need to implement top-level exception handling: all exceptions need to be caught, logged, and translated to a status value of 500 in the API method return value.
The application uses dependency injection (DI) to create instances of this class. The application targets .Net Core 2.2+ or .Net Framework 4.5+. Assume that there is a logger, registered as a DI dependency (class Logger that implements ILogger).
To implement the logging aspect, steps 1, 3 and 4 are the same as above. In step 2, the interceptor class would look like this:
    using BaseInterceptors;
    using Castle.DynamicProxy;
    using System;
    
    namespace DemoApp
    {
        public class LogExceptionInterceptor : ExceptionInterceptorSync
        {
            private ILogger _logger;
            
            public LogExecutionInterceptor(ILogger logger)
            {
                _logger = logger;
            }
            
            protected override object OnException(IInvocation invocation, Exception ex)
            {
                var method = GetMethodName(invocation);
                _logger.Error($"{method} failed.\r\nReturn value: {invocation.ReturnValue}.\r\nException: {ex}");
                // Set method invocation return value to something designating an error.
                var result = (ApiStatus)invocation.ReturnValue;
                result.Status = 500;
                return result;
            }
    
            private string GetMethodName(IInvocation invocation)
            {
                return $"{invocation.InvocationTarget.GetType().Name}.{invocation.Method.Name}()";
            }
        }
    }
We chose to derive our interceptor from ExceptionInterceptorSync, because our logging service is synchronous, and setting the result is not IO intensive. If logging was remote, you1d derive your interceptor from ExceptionInterceptorAsync.
