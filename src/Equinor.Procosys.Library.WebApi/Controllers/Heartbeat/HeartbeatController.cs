using System.ComponentModel.DataAnnotations;
using Equinor.Procosys.Library.Domain.Time;
using Equinor.Procosys.Library.WebApi.Telemetry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Library.WebApi.Controllers.Heartbeat
{
    [ApiController]
    [Route("Heartbeat")]
    public class HeartbeatController : ControllerBase
    {
        private readonly ILogger<HeartbeatController> _logger;
        private readonly ITelemetryClient _telemetryClient;

        public HeartbeatController(ILogger<HeartbeatController> logger, ITelemetryClient telemetryClient)
        {
            _logger = logger;
            _telemetryClient = telemetryClient;
        }

        [AllowAnonymous]
        [HttpGet("IsAlive")]
        public IActionResult IsAlive()
        {
            var timestampString = $"{TimeService.UtcNow:yyyy-MM-dd HH:mm:ss} UTC";
            _logger.LogInformation($"The application is running at {timestampString}");
            _telemetryClient.TrackEvent("Heartbeat");
            return new JsonResult(new
            {
                IsAlive = true,
                TimeStamp = timestampString
            });
        }

        [HttpPost("LogInfo")]
        public void LogInfo([Required] string info)
            => _logger.LogInformation($"Information: {info}");

        [HttpPost("LogWarning")]
        public void LogWarning([Required] string warning)
            => _logger.LogWarning($"Warning: {warning}");

        [HttpPost("LogError")]
        public void LogError([Required] string error)
            => _logger.LogError($"Error: {error}");

        [HttpPost("LogException")]
        public void LogException([Required] string error)
            => _logger.LogError(new System.Exception(error), $"Exception: {error}");
    }
}
