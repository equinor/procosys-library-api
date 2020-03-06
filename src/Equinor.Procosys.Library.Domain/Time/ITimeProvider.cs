using System;

namespace Equinor.Procosys.Library.Domain.Time
{
    public interface ITimeProvider
    {
        DateTime UtcNow { get; }
    }
}
