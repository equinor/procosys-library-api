using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Equinor.Procosys.Library.WebApi.Misc
{
    public class MainApiBearerTokenProvider : IBearerTokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _authority;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _mainApiClientId;

        public MainApiBearerTokenProvider(
            IHttpContextAccessor httpContextAccessor,
            IOptionsMonitor<ApiOptions> apiOptions,
            IOptionsMonitor<MainApiOptions> mainApiOptions)
        {
            _httpContextAccessor = httpContextAccessor;

            _authority = apiOptions.CurrentValue.Authority;
            _clientId = apiOptions.CurrentValue.Audience;
            _clientSecret = apiOptions.CurrentValue.ClientSecret;

            _mainApiClientId = mainApiOptions.CurrentValue.Audience;
        }

        public async Task<string> GetBearerTokenAsync()
        {
            var clientCred = new ClientCredential(_clientId, _clientSecret);

            var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var userToken = authorizationHeader.ToString().Split(' ')[1];
            var userAssertion = new UserAssertion(userToken);

            var authContext = new AuthenticationContext(_authority);
            var authenticationResult = await authContext.AcquireTokenAsync(_mainApiClientId, clientCred, userAssertion);
            return authenticationResult?.AccessToken;
        }
    }
}
