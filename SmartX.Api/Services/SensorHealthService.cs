using SmartX.Api.Data;

namespace SmartX.Api.Services;

public static class SensorHealthService
{
    public static void RefreshSensorHealth()
    {
        foreach (var sensor
            in SmartXStore.Sensors.Values)
        {
            var secondsSinceLastSeen =
                (DateTime.UtcNow - sensor.LastSeen)
                .TotalSeconds;

            sensor.IsOnline =
                secondsSinceLastSeen <= 60;
        }
    }
}