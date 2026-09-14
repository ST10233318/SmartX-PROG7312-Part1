namespace SmartX.Api.Models;

public class SensorReading
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string DeviceId { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public double NumericValue { get; set; }

    public string DisplayValue { get; set; } = string.Empty;

    public bool IsAnomaly { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}