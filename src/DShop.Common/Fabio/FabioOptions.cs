namespace DShop.Common.Fabio
{
    public class FabioOptions
    {
        public bool Enabled { get; set; }
        public string Url { get; set; }
        public string Service { get; set; }
        public int RequestRetries { get; set; }
    }
}