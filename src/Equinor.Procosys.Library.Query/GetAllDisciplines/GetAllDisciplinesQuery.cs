using System.Collections.Generic;
using MediatR;
using ServiceResult;

namespace Equinor.Procosys.Library.Query.GetAllDisciplines
{
    public class GetAllDisciplinesQuery : IRequest<Result<IEnumerable<DisciplineDto>>>
    {
        public GetAllDisciplinesQuery(string plant, IEnumerable<string> classifications)
        {
            Plant = plant;
            Classifications = classifications;
        }

        public string Plant { get; }
        public IEnumerable<string> Classifications { get; }
    }
}
