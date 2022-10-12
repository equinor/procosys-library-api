using System.Collections.Generic;

namespace Equinor.Procosys.Library.WebApi.Telemetry
{
    public interface ITelemetryClient
    {
        void TrackEvent(string name, Dictionary<string, string> properties = null);
    }
}
