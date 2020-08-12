using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.Client;
using MediatR;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.GetAllResponsibles
{
    public class GetAllResponsiblesQueryHandler : IRequestHandler<GetAllResponsiblesQuery, Result<IEnumerable<ResponsibleDto>>>
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;

        public GetAllResponsiblesQueryHandler(
            IBearerTokenApiClient mainApiClient,
            IOptionsMonitor<MainApiOptions> options)
        {
            _apiVersion = options.CurrentValue.ApiVersion;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
            _mainApiClient = mainApiClient;
        }

        public async Task<Result<IEnumerable<ResponsibleDto>>> Handle(GetAllResponsiblesQuery request, CancellationToken cancellationToken)
        {
            var url = $"{ _baseAddress}Library/Responsibles" +
                      $"?plantId={request.Plant}" +
                      $"&api-version={_apiVersion}";

            var mainApiResponsibles = await _mainApiClient.QueryAndDeserializeAsync<List<MainApiResponsible>>(url) ?? new List<MainApiResponsible>();
            var responsibleDtos = mainApiResponsibles.Select(a => new ResponsibleDto(a.Code, a.Description));
            return new SuccessResult<IEnumerable<ResponsibleDto>>(responsibleDtos);
        }
    }
}
