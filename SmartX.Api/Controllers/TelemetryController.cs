using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Data;
using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TelemetryController : ControllerBase
{
    [HttpPost("temperature")]
    public IActionResult ReceiveTemperature(
        TelemetryPacket<float> packet)
    {
        if (!SmartXStore.Sensors.ContainsKey(packet.DeviceId))
        {
            return BadRequest("Sensor is not registered.");
        }

        var reading = TelemetryProcessor.Process(packet);

        return Ok(reading);
    }

    [HttpPost("power")]
    public IActionResult ReceivePower(
        TelemetryPacket<int> packet)
    {
        if (!SmartXStore.Sensors.ContainsKey(packet.DeviceId))
        {
            return BadRequest("Sensor is not registered.");
        }

        var reading = TelemetryProcessor.Process(packet);

        return Ok(reading);
    }

    [HttpPost("switch")]
    public IActionResult ReceiveSwitch(
        TelemetryPacket<bool> packet)
    {
        if (!SmartXStore.Sensors.ContainsKey(packet.DeviceId))
        {
            return BadRequest("Sensor is not registered.");
        }

        var reading = TelemetryProcessor.Process(packet);

        return Ok(reading);
    }

    [HttpGet]
    public IActionResult GetTelemetry()
    {
        return Ok(SmartXStore.Readings.ToArray().Reverse());
    }

    [HttpGet("anomalies")]
    public IActionResult GetAnomalies()
    {
        return Ok(
            SmartXStore.Readings
                .Where(x => x.IsAnomaly)
                .OrderByDescending(x => x.Timestamp)
                .ToList());
    }
}