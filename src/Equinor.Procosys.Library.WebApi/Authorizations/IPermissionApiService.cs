using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.Procosys.Library.WebApi.Authorizations
{
    public interface IPermissionApiService
    {
        Task<IList<string>> GetPermissionsAsync(string plantId);
    }
}
