using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Data;
using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers;
[ApiController]
[Route("api/realtime")]
public class RealtimeController(RealtimeEngine engine) : ControllerBase
{
    [HttpGet("state")]
    public IActionResult State([FromQuery] string? category, [FromQuery] bool alertsOnly = false, [FromQuery] string? search = null)
        => Ok(engine.Snapshot(category, alertsOnly, search));

    [HttpPost("commands")]
    public IActionResult Command([FromBody] CommandRequest request)
    {
        try { return Ok(engine.Issue(request.DeviceId, request.Action)); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }
    [HttpPost("undo")]
    public IActionResult Undo() => engine.Undo() is { } cmd ? Ok(cmd) : NotFound(new { error = "No command to undo." });
    [HttpPost("process-next")]
    public IActionResult ProcessNext() => Ok(engine.ProcessNext());
    [HttpDelete("alerts/{deviceId}")]
    public IActionResult ClearAlert(string deviceId) => engine.ClearAlert(deviceId) ? NoContent() : NotFound();
    [HttpPost("simulate")]
    public IActionResult Simulate([FromBody] SimulatedReading input)
    {
        if (!SmartXStore.Sensors.ContainsKey(input.DeviceId)) return BadRequest(new { error = "Register device first." });
        if (string.IsNullOrWhiteSpace(input.Category)) return BadRequest(new { error = "Category is required." });
        var reading = new SensorReading { DeviceId = input.DeviceId, Category = input.Category, NumericValue = input.Value,
            DisplayValue = input.Value.ToString("F2"), Location = "Simulated", IsAnomaly = input.Critical ||
            (input.Category.Contains("moisture", StringComparison.OrdinalIgnoreCase) && input.Value < 20), Timestamp = DateTime.UtcNow };
        SmartXStore.Readings.Enqueue(reading);
        engine.Accept(reading);
        if (SmartXStore.Sensors.TryGetValue(input.DeviceId, out var sensor)) { sensor.LastSeen = DateTime.UtcNow; sensor.IsOnline = true; }
        return Accepted(reading);
    }
}
public record CommandRequest(string DeviceId, string Action);
public record SimulatedReading(string DeviceId, string Category, double Value, bool Critical = false);
