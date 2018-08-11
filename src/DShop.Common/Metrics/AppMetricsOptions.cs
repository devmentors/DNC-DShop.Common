namespace DShop.Common.Metrics
{
    public class MetricsOptions
    {
        public string Database { get; set; }
        public string Uri { get; set; }
        public string Env { get; set; }
        public int Interval { get; set; }
        public bool Enabled { get; set; }
    }
}