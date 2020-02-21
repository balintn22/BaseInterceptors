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
            _container.Register(Component.For<LogExecutionInterceptor>().ImplementedBy<LogExecutionInterceptor>().Named(nameof(LogExecutionInterceptor)).LifestyleTransient());
            _container.Register(Component.For<LogTimingInterceptor>().ImplementedBy<LogTimingInterceptor>().Named(nameof(LogTimingInterceptor)).LifestyleTransient());

            // Register types
            _container.Register(Component.For<IBusiness>().ImplementedBy<Business>());
        }

        public static T Resolve<T>()
        {
            return _container.Resolve<T>();
        }
    }
}
