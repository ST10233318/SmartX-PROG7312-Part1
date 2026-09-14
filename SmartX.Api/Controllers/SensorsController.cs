using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Data;
using SmartX.Api.Models;

namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetSensors()
    {
        return Ok(SmartXStore.Sensors.Values);
    }

    [HttpPost("register")]
    public IActionResult RegisterSensor(Sensor sensor)
    {
        if (string.IsNullOrWhiteSpace(sensor.DeviceId))
        {
            return BadRequest("Device ID is required.");
        }

        if (string.IsNullOrWhiteSpace(sensor.DeploymentLocation))
        {
            return BadRequest("Deployment location is required.");
        }

        if (string.IsNullOrWhiteSpace(sensor.Category))
        {
            return BadRequest("Sensor category is required.");
        }

        sensor.LastSeen = DateTime.UtcNow;
        sensor.IsOnline = true;

        SmartXStore.Sensors[sensor.DeviceId] = sensor;

        return Ok(sensor);
    }

    [HttpGet("{deviceId}")]
    public IActionResult GetSensor(string deviceId)
    {
        if (!SmartXStore.Sensors.TryGetValue(deviceId, out var sensor))
        {
            return NotFound("Sensor not found.");
        }

        return Ok(sensor);
    }
}