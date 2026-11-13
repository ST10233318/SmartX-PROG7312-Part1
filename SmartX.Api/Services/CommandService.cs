namespace SmartX.Api.Services;

public sealed record DeviceCommand(Guid Id, string DeviceId, string Action,
    DateTime CreatedAt, string Status);

public sealed class CommandService
{
    private readonly System.Collections.Concurrent.ConcurrentQueue<DeviceCommand> _history = new();
    public DeviceCommand Send(string deviceId, string action)
    {
        var command = new DeviceCommand(Guid.NewGuid(), deviceId, action,
            DateTime.UtcNow, "Queued (simulation)");
        _history.Enqueue(command);
        return command;
    }
    public DeviceCommand[] History(string? deviceId = null) => _history.ToArray()
        .Where(x => deviceId is null || string.Equals(x.DeviceId, deviceId,
            StringComparison.OrdinalIgnoreCase)).OrderByDescending(x => x.CreatedAt).ToArray();
}
