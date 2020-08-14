using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Library.WebApi.Caches
{
    public interface IPlantCache
    {
        Task<IList<string>> GetPlantIdsForUserOidAsync(Guid userOid);
        Task<bool> IsValidPlantForUserAsync(string plantId, Guid userOid);
        void Clear(Guid userOid);
    }
}
