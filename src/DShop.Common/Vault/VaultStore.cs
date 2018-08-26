using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp.V1.AuthMethods.UserPass;

namespace DShop.Common.Vault
{
    public class VaultStore : IVaultStore
    {
        private readonly IOptions<VaultOptions> _options;

        public VaultStore(IOptions<VaultOptions> options)
        {
            _options = options;
            LoadEnvironmentVariables();
        }

        public async Task<T> GetDefaultAsync<T>()
            => await GetAsync<T>(_options.Value.Key);

        public async Task<T> GetAsync<T>(string key)
        {
            var settings = new VaultClientSettings(_options.Value.Url, GetAuthMethod());
            var client = new VaultClient(settings);
            try
            {
                var secret = await client.V1.Secrets.KeyValue.V2.ReadSecretAsync(key);

                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(secret.Data.Data));

            }
            catch (Exception exception)
            {
                throw new VaultException($"Getting Vault secret for key: '{key}' caused an error. " +
                    $"{exception.Message}", exception, key);
            }
        }

        private IAuthMethodInfo GetAuthMethod()
        {
            switch(_options.Value.AuthType?.ToLowerInvariant())
            {
                case "token": return new TokenAuthMethodInfo(_options.Value.Token);
                case "userpass": return new UserPassAuthMethodInfo(_options.Value.Username, _options.Value.Password);
            }

            throw new VaultAuthTypeNotSupportedException($"Vault auth type: '{_options.Value.AuthType}' is not supported.",
                _options.Value.AuthType);
        }

        private void LoadEnvironmentVariables()
        {
            _options.Value.Url = GetEnvironmentVariableValue("VAULT_URL") ?? _options.Value.Url;
            _options.Value.Key = GetEnvironmentVariableValue("VAULT_KEY") ?? _options.Value.Key;
            _options.Value.AuthType = GetEnvironmentVariableValue("VAULT_AUTH_TYPE") ?? _options.Value.AuthType;
            _options.Value.Token = GetEnvironmentVariableValue("VAULT_TOKEN") ?? _options.Value.Token;
            _options.Value.Username = GetEnvironmentVariableValue("VAULT_USERNAME") ?? _options.Value.Username;
            _options.Value.Password = GetEnvironmentVariableValue("VAULT_PASSWORD") ?? _options.Value.Password;
        }

        private static string GetEnvironmentVariableValue(string key)
        {
            var value = Environment.GetEnvironmentVariable(key);

            return string.IsNullOrWhiteSpace(value) ? null : value;
        }
    }
}