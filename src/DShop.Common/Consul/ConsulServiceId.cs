using System;

namespace DShop.Common.Consul
{
    public class ConsulServiceId : IServiceId
    {
        private static readonly string _id = $"{Guid.NewGuid():N}";

        public string Id => _id;
    }
}