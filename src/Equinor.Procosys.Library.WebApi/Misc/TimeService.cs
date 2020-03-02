using System;
using Equinor.Procosys.Library.Domain;

namespace Equinor.Procosys.Library.WebApi.Misc
{
    public class TimeService : ITimeService
    {
        public DateTime GetCurrentTimeUtc() => DateTime.UtcNow;
    }
}
