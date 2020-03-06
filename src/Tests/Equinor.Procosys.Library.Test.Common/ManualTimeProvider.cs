using System;
using System.Collections.Generic;
using System.Text;
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
                throw new ArgumentException("Must be UTC");
            }

            UtcNow = now;
        }

        public DateTime UtcNow { get; private set; }

        public void Elapse(TimeSpan elapsedTime) => UtcNow += elapsedTime;

        public void SetTime(DateTime now) => UtcNow = now;
    }
}
