using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Data;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/commands")]
public sealed class CommandsController(CommandService commands) : ControllerBase
{
    public sealed record CommandRequest(string DeviceId, string Action);
    [HttpPost]
    public IActionResult Send(CommandRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.DeviceId) ||
            string.IsNullOrWhiteSpace(request.Action)) return BadRequest("Device and action are required.");
        if (!SmartXStore.Sensors.ContainsKey(request.DeviceId)) return NotFound("Unknown sensor.");
        return Accepted(commands.Send(request.DeviceId, request.Action));
    }
    [HttpGet]
    public IActionResult History([FromQuery] string? deviceId = null) => Ok(commands.History(deviceId));
}
