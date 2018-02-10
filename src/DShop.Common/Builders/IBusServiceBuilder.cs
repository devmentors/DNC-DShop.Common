using System;
using DShop.Common.Bus;

namespace DShop.Common.Builders
{
    public interface IBusServiceBuilder
    {
         IServiceBuilder WithServiceBus(string settingsSectionName, Action<ISubscribeBus> subscriptions);
    }
}