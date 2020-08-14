using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.Client;
using MediatR;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.GetAllDisciplines
{
    public class GetAllDisciplinesQueryHandler : IRequestHandler<GetAllDisciplinesQuery, Result<IEnumerable<DisciplineDto>>>
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;

        public GetAllDisciplinesQueryHandler(
            IBearerTokenApiClient mainApiClient,
            IOptionsMonitor<MainApiOptions> options)
        {
            _apiVersion = options.CurrentValue.ApiVersion;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
            _mainApiClient = mainApiClient;
        }

        public async Task<Result<IEnumerable<DisciplineDto>>> Handle(GetAllDisciplinesQuery request, CancellationToken cancellationToken)
        {
            var url = $"{ _baseAddress}Library/Disciplines" +
                $"?plantId={request.Plant}" +
                string.Join("", request.Classifications
                    .Where(c => c!= null)
                    .Select(c => $"&classifications={c.ToUpper()}")) +
                $"&api-version={_apiVersion}";

            var mainApiDisciplines = await _mainApiClient.QueryAndDeserializeAsync<List<MainApiDiscipline>>(url) ?? new List<MainApiDiscipline>();
            var disciplineDtos = mainApiDisciplines.Select(discipline => new DisciplineDto(discipline.Code, discipline.Description));
            return new SuccessResult<IEnumerable<DisciplineDto>>(disciplineDtos);
        }
    }
}
