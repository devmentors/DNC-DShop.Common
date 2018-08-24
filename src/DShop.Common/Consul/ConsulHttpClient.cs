using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DShop.Common.Consul
{
    public class ConsulHttpClient
    {
        private HttpClient _client;

        public ConsulHttpClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<T> GetAsync<T>(string requestUri)
        {
            var uri = requestUri.StartsWith("http://") ? requestUri : $"http://{requestUri}";
            var response = await _client.GetAsync(uri);
            if (!response.IsSuccessStatusCode)
            {
                return default(T);
            }

            var content = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}