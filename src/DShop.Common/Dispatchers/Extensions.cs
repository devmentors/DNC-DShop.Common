using Autofac;

namespace DShop.Common.Dispatchers
{
    public static class Extensions
    {
        public static void AddDispatchers(this ContainerBuilder builder)
        {
            builder.RegisterType<CommandDispatcher>().As<ICommandDispatcher>();
            builder.RegisterType<Dispatcher>().As<IDispatcher>();
            builder.RegisterType<QueryDispatcher>().As<IQueryDispatcher>();
        }
    }
}