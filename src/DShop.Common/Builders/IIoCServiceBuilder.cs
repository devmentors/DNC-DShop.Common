using System;
using Autofac;

namespace DShop.Common.Builders
{
    public interface IIoCServiceBuilder
    {
        IDatabaseServiceBuilder WithRegistration(Action<ContainerBuilder> registerDependencies);
    }
}