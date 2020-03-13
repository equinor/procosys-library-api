using System;
using Equinor.Procosys.Library.Domain.Time;

namespace Equinor.Procosys.Library.Test.Common
{
    public class ManualTimeProvider : ITimeProvider
    {
        public ManualTimeProvider()
        {
        }

        public ManualTimeProvider(DateTime now)
        {
            if (now.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Time must be UTC");
            }

            UtcNow = now;
        }

        public DateTime UtcNow { get; private set; }

        public void Elapse(TimeSpan elapsedTime) => UtcNow += elapsedTime;

        public void Set(DateTime now)
        {
            if (now.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Time must be UTC");
            }

            UtcNow = now;
        }
    }
}
