using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.Procosys.Library.Infrastructure.Caching;
using Equinor.Procosys.Library.WebApi.Authorizations;
using Microsoft.Extensions.Options;

namespace Equinor.Procosys.Library.WebApi.Caches
{
    public class PlantCache : IPlantCache
    {
        private readonly ICacheManager _cacheManager;
        private readonly IPlantApiService _plantApiService;
        private readonly IOptionsMonitor<CacheOptions> _options;

        public PlantCache(
            ICacheManager cacheManager, 
            IPlantApiService plantApiService, 
            IOptionsMonitor<CacheOptions> options)
        {
            _cacheManager = cacheManager;
            _plantApiService = plantApiService;
            _options = options;
        }

        public async Task<IList<string>> GetPlantIdsForUserOidAsync(Guid userOid)
            => await _cacheManager.GetOrCreate(
                PlantsCacheKey(userOid),
                async () =>
                {
                    var plants = await _plantApiService.GetPlantsAsync();
                    return plants?.Select(p => p.Id).ToList();
                },
                CacheDuration.Minutes,
                _options.CurrentValue.PlantCacheMinutes);

        public async Task<bool> IsValidPlantForUserAsync(string plantId, Guid userOid)
        {
            var plantIds = await GetPlantIdsForUserOidAsync(userOid);
            return plantIds!= null && plantIds.Contains(plantId);
        }

        public void Clear(Guid userOid) => _cacheManager.Remove(PlantsCacheKey(userOid));

        private string PlantsCacheKey(Guid userOid)
            => $"PLANTS_{userOid.ToString().ToUpper()}";
    }
}
