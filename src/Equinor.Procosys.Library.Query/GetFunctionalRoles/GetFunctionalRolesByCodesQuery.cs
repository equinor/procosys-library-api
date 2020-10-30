using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.GetFunctionalRoles
{
    public class GetFunctionalRolesByCodesQuery : IRequest<Result<IEnumerable<FunctionalRoleDto>>>
    {
        public GetFunctionalRolesByCodesQuery(string plant, List<string> functionalRoleCodes)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            Codes = functionalRoleCodes;
        }

        public string Plant { get; }
        public List<string> Codes { get; }
    }
}
