using System;
using Autofac;

namespace DShop.Common.Builders
{
    public interface IIoCServiceBuilder
    {
        IDatabaseServiceBuilder WithAutofac(Action<ContainerBuilder> registerDependencies);
    }
}