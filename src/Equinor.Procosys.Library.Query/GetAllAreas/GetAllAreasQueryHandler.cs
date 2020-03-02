using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.Client;
using MediatR;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.GetAllAreas
{
    public class GetAllAreasQueryHandler : IRequestHandler<GetAllAreasQuery, Result<IEnumerable<AreaDto>>>
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;

        public GetAllAreasQueryHandler(
            IBearerTokenApiClient mainApiClient,
            IOptionsMonitor<MainApiOptions> options)
        {
            _apiVersion = options.CurrentValue.ApiVersion;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
            _mainApiClient = mainApiClient;
        }

        public async Task<Result<IEnumerable<AreaDto>>> Handle(GetAllAreasQuery request, CancellationToken cancellationToken)
        {
            var url = $"{ _baseAddress}Library/Areas" +
                $"?plantId={request.Plant}" +
                $"&api-version={_apiVersion}";

            var mainApiAreas = await _mainApiClient.QueryAndDeserialize<List<MainApiArea>>(url) ?? new List<MainApiArea>();
            var areaDtos = mainApiAreas.Select(a => new AreaDto(a.Code, a.Description));
            return new SuccessResult<IEnumerable<AreaDto>>(areaDtos);
        }
    }
}
