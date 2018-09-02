namespace DShop.Common.Metrics
{
    public class MetricsOptions
    {
        public bool Enabled { get; set; }
        public bool PrometheusEnabled { get; set; }
        public string InfluxUrl { get; set; }
        public string Database { get; set; }
        public string Env { get; set; }
        public int Interval { get; set; }
    }
}