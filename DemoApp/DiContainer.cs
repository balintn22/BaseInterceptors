using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace DemoApp
{
    public static class DiContainer
    {
        private static WindsorContainer _container;

        public static void Initialize()
        {
            _container = new WindsorContainer();

            // Register interceptors
            _container.Register(Component.For<LogExceptionInterceptor>().ImplementedBy<LogExceptionInterceptor>().Named(nameof(LogExceptionInterceptor)).LifestyleTransient());
            _container.Register(Component.For<LogExecutionInterceptor>().ImplementedBy<LogExecutionInterceptor>().Named(nameof(LogExecutionInterceptor)).LifestyleTransient());
            _container.Register(Component.For<LogTimingInterceptor>().ImplementedBy<LogTimingInterceptor>().Named(nameof(LogTimingInterceptor)).LifestyleTransient());

            // Register types
            _container.Register(Component.For<IBusiness>().ImplementedBy<Business>());
            // TODO: Castle.Core documentation suggests this method to register multiple interceptors
            // for execution in a specific order. However, this doesn't work with my current interception
            // implementation: invocation.MethodInvocationTarget that is used to distinguish between
            // sync/async intercepted methods - returns null.
            // Register a component with interceptors.
            // See https://github.com/castleproject/Windsor/blob/master/docs/registering-interceptors-and-proxyoptions.md#registering-interceptors-and-proxyoptions
            //_container.Register(
            //    Component.For<IBusiness>()
            //        .Interceptors(
            //            InterceptorReference.ForType<LogExceptionInterceptor>(),
            //            InterceptorReference.ForType<LogExecutionInterceptor>(),
            //            InterceptorReference.ForType<LogTimingInterceptor>()
            //        ).Last,
            //    Component.For<LogExceptionInterceptor>()
            //);
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}
