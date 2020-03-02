using System;

namespace Equinor.Procosys.Library.Domain
{
    public interface ITimeService
    {
        DateTime GetCurrentTimeUtc();
    }
}
