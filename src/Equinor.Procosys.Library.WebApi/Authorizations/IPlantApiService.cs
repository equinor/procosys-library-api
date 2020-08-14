using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Library.WebApi.Authorizations
{
    public interface IPlantApiService
    {
        Task<List<ProcosysPlant>> GetPlantsAsync();
    }
}
