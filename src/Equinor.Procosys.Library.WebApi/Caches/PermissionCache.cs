using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Infrastructure.Caching;
using Equinor.Procosys.Library.WebApi.Authorizations;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Library.WebApi.Caches
{
    public class PermissionCache : IPermissionCache
    {
        private readonly ICacheManager _cacheManager;
        private readonly IPermissionApiService _permissionApiService;
        private readonly IOptionsMonitor<CacheOptions> _options;

        public PermissionCache(
            ICacheManager cacheManager,
            IPermissionApiService permissionApiService,
            IOptionsMonitor<CacheOptions> options)
        {
            _cacheManager = cacheManager;
            _permissionApiService = permissionApiService;
            _options = options;
        }

        public async Task<IList<string>> GetPermissionsForUserAsync(string plantId, Guid userOid)
            => await _cacheManager.GetOrCreate(
                PermissionsCacheKey(plantId, userOid),
                async () => await _permissionApiService.GetPermissionsAsync(plantId),
                CacheDuration.Minutes,
                _options.CurrentValue.PermissionCacheMinutes);

        public void ClearAll(string plantId, Guid userOid)
            => _cacheManager.Remove(PermissionsCacheKey(plantId, userOid));

        private static string PermissionsCacheKey(string plantId, Guid userOid)
        {
            if (userOid == Guid.Empty)
            {
                throw new Exception("Illegal userOid for cache");
            }
            return $"PERMISSIONS_{userOid.ToString().ToUpper()}_{plantId}";
        }
    }
}
