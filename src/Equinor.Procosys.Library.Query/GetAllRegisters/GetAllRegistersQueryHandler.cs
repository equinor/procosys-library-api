using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Query.Client;
using MediatR;
using Microsoft.Extensions.Options;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.GetAllRegisters
{
    public class GetAllRegistersQueryHandler : IRequestHandler<GetAllRegistersQuery, Result<IEnumerable<RegisterDto>>>
    {
        private readonly string _apiVersion;
        private readonly Uri _baseAddress;
        private readonly IBearerTokenApiClient _mainApiClient;

        public GetAllRegistersQueryHandler(
            IBearerTokenApiClient mainApiClient,
            IOptionsMonitor<MainApiOptions> options)
        {
            _apiVersion = options.CurrentValue.ApiVersion;
            _baseAddress = new Uri(options.CurrentValue.BaseAddress);
            _mainApiClient = mainApiClient;
        }

        public async Task<Result<IEnumerable<RegisterDto>>> Handle(GetAllRegistersQuery request, CancellationToken cancellationToken)
        {
            var url = $"{ _baseAddress}Library/Registers" +
                $"?plantId={request.Plant}" +
                $"&api-version={_apiVersion}";

            var mainApiRegisters = await _mainApiClient.QueryAndDeserializeAsync<List<MainApiRegister>>(url) ?? new List<MainApiRegister>();
            var registerDtos = mainApiRegisters.Select(a => new RegisterDto(a.Code, a.Description));
            return new SuccessResult<IEnumerable<RegisterDto>>(registerDtos);
        }
    }
}
