using Castle.DynamicProxy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace BaseInterceptors.Test
{
    [TestClass]
    public class ExecutionInterceptorTests
    {
        const string RETURN_VALUE_SET_BY_METHOD = "return value set by method";
        const string RETURN_VALUE_SET_BY_ASPECT = "return value set by aspect";

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
                return RETURN_VALUE_SET_BY_METHOD;
            }

            public async Task<string> MethodAsync()
            {
                await Task.Delay(100);
                return RETURN_VALUE_SET_BY_METHOD;
            }
        }

        public class InterceptorUnderTest : ExecutionInterceptorSync
        {
            protected override void OnEntry(IInvocation invocation)
            {
                // Do nothing
            }

            protected override void OnExit(IInvocation invocation)
            {
                InvocationHelper.SetReturnValue(invocation, RETURN_VALUE_SET_BY_ASPECT);
            }
        }

        [TestMethod]
        public void InterceptSyncMethod_Test_IfOnCompletedSetsReturnValue_ThatValueShouldBeRturned()
        {
            var result = new ProxyGenerator().CreateInterfaceProxyWithTarget<IMy>(new My(), new InterceptorUnderTest());
            result.Should().NotBeNull();
            Assert.IsNotNull(result);

            var returnValue = result.Method();

            returnValue.Should().Be(RETURN_VALUE_SET_BY_ASPECT);
        }

        // TODO: This test is failing. Need to find a reliable way of setting the result of an intercepted async call.
        [TestMethod]
        public async Task InterceptAsyncMethod_Test_IfOnCompletedSetsReturnValue_ThatValueShouldBeRturned()
        {
            var result = new ProxyGenerator().CreateInterfaceProxyWithTarget<IMy>(new My(), new InterceptorUnderTest());
            result.Should().NotBeNull();
            Assert.IsNotNull(result);

            var returnValue = await result.MethodAsync();

            returnValue.Should().Be(RETURN_VALUE_SET_BY_ASPECT);
        }
    }
}
