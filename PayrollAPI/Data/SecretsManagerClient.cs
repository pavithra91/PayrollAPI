using Amazon.SecretsManager.Model;
using Amazon.SecretsManager;
using Microsoft.Extensions.Caching.Memory;

namespace PayrollAPI.Data
{
    public class SecretsManagerClient
    {
        private readonly IAmazonSecretsManager _amazonSecretsManager;
        private readonly IMemoryCache _cache;

        public SecretsManagerClient(IAmazonSecretsManager amazonSecretsManager, IMemoryCache cache)
        {
            _amazonSecretsManager = amazonSecretsManager;
            _cache = cache;
        }

        public async Task<string> GetSecretValueAsync(string secretId)
        {
            // Check the cache first
            if (_cache.TryGetValue(secretId, out string cachedSecret))
            {
                return cachedSecret;
            }

            // Retrieve from AWS Secrets Manager if not found in cache
            var request = new GetSecretValueRequest { SecretId = secretId };
            var result = await _amazonSecretsManager.GetSecretValueAsync(request);
            var secretValue = result.SecretString;

            // Set the secret in cache with expiration
            _cache.Set(secretId, secretValue, TimeSpan.FromMinutes(15)); // Adjust expiration as needed

            return secretValue;
        }
    }
}
