using System.Collections.Generic;

namespace DShop.Common.Bus
{
    internal sealed class ServiceBusOptions
    {
        public string QueueName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public List<string> Hostnames { get; set; }
    }
}