using Castle.DynamicProxy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace BaseInterceptors.Test
{
    // This test was implemented based on https://github.com/castleproject/Core/issues/107
    // to understand how to set return values in async interceptors.
    [TestClass]
    public class ExampleTests
    {
        public interface IMy
        {
            Task<string> MethodAsync();
        }

        public class My : IMy
        {
            public async Task<string> MethodAsync()
            {
                await Task.Delay(100);
                throw new Exception("Something failed in target method");
            }
        }

        public class InterceptorUnderTest : IInterceptor
        {
            // Sample from https://github.com/castleproject/Core/issues/107
            //public void Intercept(IInvocation invocation)
            //{
            //    invocation.Proceed();
            //    var returnValue = (Task<string>)invocation.ReturnValue;
            //    returnValue.ContinueWith(t =>
            //    {
            //        Console.WriteLine(t.Exception.Message);
            //        invocation.ReturnValue = Task.FromResult("Oops");
            //    }, TaskContinuationOptions.OnlyOnFaulted);
            //}

            public void Intercept(IInvocation invocation)
            {
                invocation.Proceed();
                var returnValue = (Task<string>)invocation.ReturnValue;
                invocation.ReturnValue = returnValue.ContinueWith(
                    t =>
                    {
                        Console.WriteLine(t.Exception.Message);
                        return "Oops";
                    },
                    TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        [TestMethod]
        public async Task AyncTestCase_Happy_ReturnValueShouldMatchValueSetByInterceptor()
        {
            var result = new ProxyGenerator().CreateInterfaceProxyWithTarget<IMy>(new My(), new InterceptorUnderTest());
            Assert.IsNotNull(result);

            var returnValue = await result.MethodAsync();
            Assert.AreEqual("Oops", returnValue);
        }
    }
}
