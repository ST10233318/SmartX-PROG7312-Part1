using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/history")]
public sealed class HistoryController(TelemetryHistoryService history) : ControllerBase
{
    [HttpGet]
    public IActionResult Query([FromQuery] DateTime from, [FromQuery] DateTime to,
        [FromQuery] string? deviceId = null)
    {
        try { return Ok(history.Query(from, to, deviceId)); }
        catch (ArgumentException e) { return BadRequest(e.Message); }
    }
}
