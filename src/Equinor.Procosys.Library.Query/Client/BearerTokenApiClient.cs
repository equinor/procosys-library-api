using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Library.Query.Client
{
    public class BearerTokenApiClient : IBearerTokenApiClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IBearerTokenProvider _bearerTokenProvider;
        private readonly ILogger<BearerTokenApiClient> _logger;

        public BearerTokenApiClient(
            IHttpClientFactory httpClientFactory,
            IBearerTokenProvider bearerTokenProvider,
            ILogger<BearerTokenApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _bearerTokenProvider = bearerTokenProvider;
            _logger = logger;
    }

        public async Task<T> QueryAndDeserializeAsync<T>(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException(nameof(url));
            }

            if (url.Length > 2000)
            {
                throw new ArgumentException("url exceed max 2000 characters", nameof(url));
            }

            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await _bearerTokenProvider.GetBearerTokenAsync());

            var stopWatch = Stopwatch.StartNew();
            var response = await httpClient.GetAsync(url);
            stopWatch.Stop();

            var msg = $"{stopWatch.Elapsed.TotalMilliseconds}ms elapsed when requesting '{url}'. Status: {response.StatusCode}";
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogWarning(msg);
                    return default;
                }
                _logger.LogError(msg);
                throw new Exception($"Requesting '{url}' was unsuccessful. Status={response.StatusCode}");
            }

            _logger.LogInformation(msg);
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(jsonResult);
            return result;
        }
    }
}
