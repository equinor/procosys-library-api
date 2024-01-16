using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.Client;
using MediatR;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.GetFunctionalRoles
{
    public class GetFunctionalRolesByCodesQueryHandler : IRequestHandler<GetFunctionalRolesByCodesQuery, Result<IEnumerable<FunctionalRoleDto>>>
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;

        public GetFunctionalRolesByCodesQueryHandler(
            IBearerTokenApiClient mainApiClient,
            IOptionsMonitor<MainApiOptions> options)
        {
            _apiVersion = options.CurrentValue.ApiVersion;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
            _mainApiClient = mainApiClient;
        }

        public async Task<Result<IEnumerable<FunctionalRoleDto>>> Handle(GetFunctionalRolesByCodesQuery request,
            CancellationToken cancellationToken)
        {
            var functionalRoleCodes = request.Codes.Aggregate("", (current, code) => current + $"&functionalRoleCodes={WebUtility.UrlEncode(code)}");

            var url = $"{_baseAddress}Library/FunctionalRolesByCodes" +
                      $"?plantId={request.Plant}" +
                      functionalRoleCodes +
                      $"&classification={request.Classification}" +
                      $"&api-version={_apiVersion}";

            var mainApiFunctionalRoles =
                await _mainApiClient.QueryAndDeserializeAsync<List<MainApiFunctionalRole>>(url) ??
                new List<MainApiFunctionalRole>();

            var functionalRoleDtos = ConvertToFunctionalRoleDtos(mainApiFunctionalRoles);

            return new SuccessResult<IEnumerable<FunctionalRoleDto>>(functionalRoleDtos);
        }

        private IEnumerable<FunctionalRoleDto> ConvertToFunctionalRoleDtos(IEnumerable<MainApiFunctionalRole> mainApiFunctionalRoles)
        {
            var functionalRoleDtos =
                mainApiFunctionalRoles.Select(fr =>
                    new FunctionalRoleDto(
                        fr.ProCoSysGuid,
                        fr.Code,
                        fr.Description,
                        fr.Email,
                        fr.InformationEmail,
                        fr.UsePersonalEmail,
                        fr.Persons));

            return functionalRoleDtos;
        }
    }
}
