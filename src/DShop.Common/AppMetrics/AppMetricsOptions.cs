namespace DShop.Common.AppMetrics
{
    public class AppMetricsOptions
    {
        public string Database { get; set; }
        public string Uri { get; set; }
        public string Env { get; set; }
        public int Interval { get; set; }
        public bool Enabled { get; set; }
    }
}