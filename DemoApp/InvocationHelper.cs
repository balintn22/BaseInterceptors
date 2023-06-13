using Castle.DynamicProxy;

namespace DemoApp
{
    public static class InvocationHelper
    {
        /// <summary>
        /// Gets the name of an invoked method, complete with the type name.
        /// </summary>
        public static string GetFullMethodName(IInvocation invocation) =>
            $"{invocation.InvocationTarget.GetType().Name}.{invocation.Method.Name}()";
    }
}
