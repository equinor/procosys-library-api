﻿namespace Equinor.Procosys.Library.WebApi.Caches
{
    public class CacheOptions
    {
        public int PermissionCacheMinutes { get; set; } = 20;
        public int PlantCacheMinutes { get; set; } = 20;
    }
}
