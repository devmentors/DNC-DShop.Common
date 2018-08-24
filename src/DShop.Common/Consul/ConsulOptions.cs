namespace DShop.Common.Consul
{
    public class ConsulOptions
    {
        public string Endpoint { get; set; }
        public string Service { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public bool PingEnabled { get; set; }
        public string PingEndpoint { get; set; }
        public int PingInterval { get; set; }
        public int DeregisterInterval { get; set; }
    }
}