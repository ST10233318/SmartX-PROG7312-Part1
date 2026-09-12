using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Tests;

public class TelemetryTests
{
    [Fact]
    public void TemperatureAboveThresholdIsAnomaly()
    {
        var packet = new TelemetryPacket<float>
        {
            DeviceId = "TEST-001",
            SensorCategory = "Environmental",
            Location = "Test Zone",
            Value = 80
        };

        var result =
            TelemetryProcessor.Process(packet);

        Assert.True(result.IsAnomaly);
    }

    [Fact]
    public void NormalTemperatureIsNotAnomaly()
    {
        var packet = new TelemetryPacket<float>
        {
            DeviceId = "TEST-002",
            SensorCategory = "Environmental",
            Location = "Test Zone",
            Value = 25
        };

        var result =
            TelemetryProcessor.Process(packet);

        Assert.False(result.IsAnomaly);
    }
}