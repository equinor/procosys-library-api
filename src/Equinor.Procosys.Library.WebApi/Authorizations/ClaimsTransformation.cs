using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Domain;
using Equinor.Procosys.Library.WebApi.Caches;
using Microsoft.AspNetCore.Authentication;

namespace Equinor.Procosys.Library.WebApi.Authorizations
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        private readonly IPlantProvider _plantProvider;
        private readonly IPlantCache _plantCache;
        private readonly IPermissionCache _permissionCache;

        public ClaimsTransformation(IPlantProvider plantProvider, IPlantCache plantCache, IPermissionCache permissionCache)
        {
            _plantProvider = plantProvider;
            _plantCache = plantCache;
            _permissionCache = permissionCache;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var userOid = principal.Claims.TryGetOid();
            if (!userOid.HasValue)
            {
                // not add claims if no oid found (I.e user not authenticated yet)
                return principal;
            }

            var plantId = _plantProvider.Plant;

            if (string.IsNullOrEmpty(plantId))
            {
                // not add plant specific claims if no plant given in request
                return principal;
            }

            if (!await _plantCache.IsValidPlantForUserAsync(plantId, userOid.Value))
            {
                // not add plant specific claims if plant not among plants for user
                return principal;
            }

            if (principal.Claims.All(c => c.Type != ClaimTypes.Role))
            {
                await AddRoleForLibraryPermissionsToPrincipalAsync(principal, plantId, userOid.Value);
            }

            return principal;
        }

        private async Task AddRoleForLibraryPermissionsToPrincipalAsync(ClaimsPrincipal principal, string plantId, Guid userOid)
        {
            var permissions = await _permissionCache.GetPermissionsForUserAsync(plantId, userOid);
            var claimsIdentity = new ClaimsIdentity();

            // add role claim just for "LIBRARY_GENERAL" permissions since we assume these are all we need in Library context
            permissions?.Where(p => p.StartsWith(Permissions.LIBRARY_GENERAL)).ToList().ForEach(
                permission => claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, permission)));
            principal.AddIdentity(claimsIdentity);
        }
    }
}
