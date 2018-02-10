namespace DShop.Common.IoC
{
    internal interface IDependencyResolver
    {
         TInstance Resolve<TInstance>();
    }
}