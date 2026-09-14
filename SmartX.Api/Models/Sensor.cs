namespace SmartX.Api.Models;

public class Sensor
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string DeviceId { get; set; } = string.Empty;

    public string DeploymentLocation { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public bool IsOnline { get; set; } = true;

    public DateTime LastSeen { get; set; } = DateTime.UtcNow;

    public string? AttachmentFileName { get; set; }
}