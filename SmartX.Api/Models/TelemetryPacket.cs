namespace SmartX.Api.Models;

public class TelemetryPacket<T>
{
    public string DeviceId { get; set; } = string.Empty;

    public string SensorCategory { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public T Value { get; set; } = default!;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}