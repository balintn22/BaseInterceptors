using Castle.DynamicProxy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BaseInterceptors.Test
{
    [TestClass]
    public class TimingInterceptorTests
    {
        const string MESSAGE = "Aye OK";
        // Nah, this means we can only run one test at a time.
        private static TimeSpan _executionTime;
        private const int DELAY_MS = 100;

        public interface IMy
        {
            string Method();
            Task<string> MethodAsync();
        }

        public class My : IMy
        {
            public string Method()
            {
                Thread.Sleep(100);
                return MESSAGE;
            }

            public async Task<string> MethodAsync()
            {
                await Task.Delay(100);
                return MESSAGE;
            }
        }

        public class InterceptorUnderTest : TimingInterceptorSync
        {
            public override void OnCompleted(IInvocation invocation, TimeSpan executionTime)
            {
                _executionTime = executionTime;
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _executionTime = TimeSpan.Zero;
        }

        [TestMethod]
        public void InterceptSyncMethod_Test_ReturnValueShouldMatchValueSetByInterceptor()
        {
            var result = new ProxyGenerator().CreateInterfaceProxyWithTarget<IMy>(new My(), new InterceptorUnderTest());
            result.Should().NotBeNull();
            Assert.IsNotNull(result);

            var returnValue = result.Method();

            _executionTime.TotalMilliseconds.Should().BeGreaterThan(DELAY_MS);
        }

        [TestMethod]
        public async Task InterceptAsyncMethod_Test_ReturnValueShouldMatchValueSetByInterceptor()
        {
            var result = new ProxyGenerator().CreateInterfaceProxyWithTarget<IMy>(new My(), new InterceptorUnderTest());
            result.Should().NotBeNull();
            Assert.IsNotNull(result);

            var returnValue = await result.MethodAsync();

            _executionTime.TotalMilliseconds.Should().BeGreaterThan(DELAY_MS);
        }
    }
}
