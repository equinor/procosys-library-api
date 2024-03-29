﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;

namespace Equinor.Procosys.Library.WebApi.Misc
{
    public class MainApiBearerTokenProvider : IBearerTokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _authority;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _mainApiAudience;
        private readonly ILogger<MainApiBearerTokenProvider> _logger;
        private readonly string _secretInfo;

        public MainApiBearerTokenProvider(
            IHttpContextAccessor httpContextAccessor,
            IOptionsMonitor<ApiOptions> apiOptions,
            IOptionsMonitor<MainApiOptions> mainApiOptions,
            ILogger<MainApiBearerTokenProvider> logger)
        {
            _httpContextAccessor = httpContextAccessor;

            _authority = apiOptions.CurrentValue.Authority;
            _clientId = apiOptions.CurrentValue.Audience;
            _clientSecret = apiOptions.CurrentValue.ClientSecret;

            _mainApiAudience = mainApiOptions.CurrentValue.Audience;
            _secretInfo = $"{_clientSecret.Substring(0, 2)}***{_clientSecret.Substring(_clientSecret.Length - 1, 1)}";
            _logger = logger;
        }

        public async Task<string> GetBearerTokenAsync()
        {
            _logger.LogInformation($"Getting client credentials using {_secretInfo} for {_clientId}");

            var app = ConfidentialClientApplicationBuilder.Create(_clientId)
                .WithClientSecret(_clientSecret)
                .WithAuthority(_authority)
                .WithLegacyCacheCompatibility(false)
                .Build();

            app.AddInMemoryTokenCache();

            var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var userToken = authorizationHeader.ToString().Split(' ')[1];
            var userAssertion = new UserAssertion(userToken);
            var scopes = new List<string> { _mainApiAudience + "/.default" };

            var authenticationResult = await app.AcquireTokenOnBehalfOf(scopes, userAssertion)
                .ExecuteAsync()
                .ConfigureAwait(false);

            return authenticationResult?.AccessToken;
        }
    }
}
