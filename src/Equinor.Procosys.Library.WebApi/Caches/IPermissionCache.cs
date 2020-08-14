using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Library.WebApi.Caches
{
    public interface IPermissionCache
    {
        Task<IList<string>> GetPermissionsForUserAsync(string plantId, Guid userOid);
        void ClearAll(string plantId, Guid userOid);
    }
}
