using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.Client;
using MediatR;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.GetTagFunctions
{
    public class GetTagFunctionsQueryHandler : IRequestHandler<GetTagFunctionsQuery, Result<IEnumerable<TagFunctionDto>>>
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;

        public GetTagFunctionsQueryHandler(
            IBearerTokenApiClient mainApiClient,
            IOptionsMonitor<MainApiOptions> options)
        {
            _apiVersion = options.CurrentValue.ApiVersion;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
            _mainApiClient = mainApiClient;
        }

        public async Task<Result<IEnumerable<TagFunctionDto>>> Handle(GetTagFunctionsQuery request, CancellationToken cancellationToken)
        {
            var url = $"{ _baseAddress}Library/TagFunctions" +
                $"?plantId={request.Plant}" +
                $"&registerCode={request.RegisterCode}" +
                $"&api-version={_apiVersion}";

            var mainApiTagFunctions = await _mainApiClient.QueryAndDeserializeAsync<List<MainApiTagFunction>>(url) ?? new List<MainApiTagFunction>();
            var tagFunctionDtos = mainApiTagFunctions.Select(a => new TagFunctionDto(a.Code, a.Description));
            return new SuccessResult<IEnumerable<TagFunctionDto>>(tagFunctionDtos);
        }
    }
}
