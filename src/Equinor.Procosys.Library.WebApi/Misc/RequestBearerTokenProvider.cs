using Equinor.Procosys.Library.Query.Client;
using Microsoft.AspNetCore.Http;

namespace Equinor.Procosys.Library.WebApi.Misc
{
    public class RequestBearerTokenProvider : IBearerTokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestBearerTokenProvider(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

        public string GetBearerToken()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            return authorizationHeader.ToString().Split(' ')[1];
        }
    }
}
