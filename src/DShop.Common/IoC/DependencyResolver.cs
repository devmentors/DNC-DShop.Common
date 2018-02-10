using Autofac;

namespace DShop.Common.IoC
{
    internal class DependencyResolver : IDependencyResolver
    {
        private readonly ILifetimeScope _lifetimeScope;

        public DependencyResolver(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public TInstance Resolve<TInstance>()
            => _lifetimeScope.Resolve<TInstance>();
    }
}