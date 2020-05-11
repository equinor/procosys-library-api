using System;
using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.GetTagFunctions
{
    public class GetTagFunctionsQuery : IRequest<Result<IEnumerable<TagFunctionDto>>>
    {
        public GetTagFunctionsQuery(string plant, string registerCode)
        {
            Plant = plant ?? throw new ArgumentNullException(nameof(plant));
            RegisterCode = registerCode;
        }

        public string Plant { get; }
        public string RegisterCode { get; }
    }
}
