using Autofac;

namespace DShop.Common.IoC
{
    internal class AutofacDependencyResolver : IDependencyResolver
    {
        private readonly ILifetimeScope _lifetimeScope;

        public AutofacDependencyResolver(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public TInstance Resolve<TInstance>()
            => _lifetimeScope.Resolve<TInstance>();
    }
}