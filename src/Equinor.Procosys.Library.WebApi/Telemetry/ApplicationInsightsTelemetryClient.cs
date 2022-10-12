using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace Equinor.Procosys.Library.WebApi.Telemetry
{
    public class ApplicationInsightsTelemetryClient : ITelemetryClient
    {
        private readonly TelemetryClient _aiClient;

        public ApplicationInsightsTelemetryClient(TelemetryConfiguration telemetryConfiguration)
        {
            if (telemetryConfiguration == null)
            {
                throw new ArgumentNullException(nameof(telemetryConfiguration));
            }

            var c = telemetryConfiguration.ConnectionString;
            _aiClient = new TelemetryClient(telemetryConfiguration)
            {
                // The InstrumentationKey isn't set through the configuration object. Setting it explicitly works.
                InstrumentationKey = telemetryConfiguration.InstrumentationKey
            };
        }

        public void TrackEvent(string name, Dictionary<string, string> properties)
            => _aiClient.TrackEvent(name, properties);
    }
}
