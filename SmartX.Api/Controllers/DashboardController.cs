using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Data;
using SmartX.Api.Services;
    
namespace SmartX.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    [HttpGet]
    public IActionResult GetDashboard()
    {
        var readings = SmartXStore.Readings.ToArray();

        return Ok(new
        {
            TotalSensors = SmartXStore.Sensors.Count,

            OnlineSensors = SmartXStore.Sensors.Values
                .Count(x => x.IsOnline),

            TotalReadings = readings.Length,

            Anomalies = readings
                .Count(x => x.IsAnomaly),

            LatestReadings = readings
                .OrderByDescending(x => x.Timestamp)
                .Take(20)
        });
    }
}