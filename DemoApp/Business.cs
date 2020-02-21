using Castle.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DemoApp
{
    /// <summary>
    /// Sample interface with both sync and async methods and also actions and funcs.
    /// </summary>
    public interface IBusiness
    {
        void Do(int msec);
        int Count(int msec);
        Task DoAsync(int msec);
        Task<int> CountAsync(int msec);
    }

    // The order of interceptor execution is not guaranteed by the order of these
    // attributes. To set interceptor order, see https://github.com/castleproject/Windsor/blob/master/docs/registering-interceptors-and-proxyoptions.md#registering-interceptors-and-proxyoptions
    [Interceptor(nameof(LogTimingInterceptor))]
    [Interceptor(nameof(LogExecutionInterceptor))]
    public class Business : IBusiness
    {
        public void Do(int msec)
        {
            ValidateArguments(msec);
            Thread.Sleep(msec);
        }

        public int Count(int msec)
        {
            ValidateArguments(msec);
            Thread.Sleep(msec);
            return msec;
        }

        public async Task DoAsync(int msec)
        {
            ValidateArguments(msec);
            await Task.Delay(msec);
        }

        public async Task<int> CountAsync(int msec)
        {
            ValidateArguments(msec);
            await Task.Delay(msec);
            return msec;
        }

        private void ValidateArguments(int msec)
        {
            if (msec < 0)
                throw new ArgumentException("msec must be a non-negative value.");
        }
    }
}
