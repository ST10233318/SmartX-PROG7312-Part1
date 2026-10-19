using SmartX.Api.Data;
using SmartX.Api.Models;

namespace SmartX.Api.Services;

// Single process, in-memory demonstration store. All collection operations are synchronized.
public sealed class RealtimeEngine
{
    private readonly object _gate = new();
    private readonly Queue<SensorReading> _normal = new();
    private readonly PriorityQueue<SensorReading, int> _urgent = new();
    private readonly Stack<DeviceCommand> _undo = new();
    private readonly Dictionary<string, DeviceCommand> _lastCommand = new(StringComparer.OrdinalIgnoreCase);
    private readonly SortedDictionary<DateTime, SensorReading> _timeline = new();
    private readonly HashSet<string> _activeAlerts = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<DeviceCommand> _history = new();
    private readonly Dictionary<string, int> _searches = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, int> _actions = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _states = new(StringComparer.OrdinalIgnoreCase);
    public void Accept(SensorReading reading)
    {
        lock (_gate)
        {
            if (reading.IsAnomaly || (reading.Category.Contains("moisture", StringComparison.OrdinalIgnoreCase) && reading.NumericValue < 20))
            {
                _urgent.Enqueue(reading, reading.IsAnomaly ? 0 : 1);
                _activeAlerts.Add(reading.DeviceId);
            }
            else _normal.Enqueue(reading);
            var key = reading.Timestamp.ToUniversalTime();
            while (_timeline.ContainsKey(key)) key = key.AddTicks(1);
            _timeline[key] = reading;
            while (_timeline.Count > 500) _timeline.Remove(_timeline.First().Key);
        }
    }
    public object Snapshot(string? category = null, bool alertsOnly = false, string? search = null)
    {
        lock (_gate)
        {
            if (!string.IsNullOrWhiteSpace(search))
                _searches[search] = _searches.GetValueOrDefault(search) + 1;
            var devices = SmartXStore.Sensors.Values
                .Where(d => string.IsNullOrWhiteSpace(category) || d.Category.Contains(category, StringComparison.OrdinalIgnoreCase))
                .Where(d => !alertsOnly || _activeAlerts.Contains(d.DeviceId))
                .Where(d => string.IsNullOrWhiteSpace(search) || d.DeviceId.Contains(search, StringComparison.OrdinalIgnoreCase) || d.Category.Contains(search, StringComparison.OrdinalIgnoreCase))
                .Select(d => new { d.DeviceId, d.Category, d.DeploymentLocation, d.IsOnline, d.LastSeen, Alert = _activeAlerts.Contains(d.DeviceId), State = _states.GetValueOrDefault(d.DeviceId, "unknown") }).ToArray();
            return new { devices, timeline = _timeline.Values.Reverse().Take(80).ToArray(), commands = _history.TakeLast(40).Reverse().ToArray(), alerts = _activeAlerts.ToArray(), normalPending = _normal.Count, urgentPending = _urgent.Count, suggestions = SuggestionsInternal() };
        }
    }
    public object ProcessNext()
    {
        lock (_gate)
        {
            if (_urgent.TryDequeue(out var priority, out _)) return new { queue = "priority", reading = priority };
            if (_normal.TryDequeue(out var normal)) return new { queue = "fifo", reading = normal };
            return new { queue = "empty", reading = (SensorReading?)null };
        }
    }
    public DeviceCommand Issue(string deviceId, string action)
    {
        lock (_gate)
        {
            if (!SmartXStore.Sensors.ContainsKey(deviceId)) throw new ArgumentException("Unknown device.");
            if (string.IsNullOrWhiteSpace(action) || action.Length > 100) throw new ArgumentException("Action must be 1–100 characters.");
            var previous = _states.GetValueOrDefault(deviceId, "unknown");
            var command = new DeviceCommand(Guid.NewGuid(), deviceId, action, previous, DateTime.UtcNow, false);
            _undo.Push(command); _history.Add(command); _states[deviceId] = action;
            var key = deviceId + ":" + action;
            _actions[key] = _actions.GetValueOrDefault(key) + 1;
            _lastCommand[deviceId] = command;
            return command;
        }
    }
    public DeviceCommand? Undo()
    {
        lock (_gate)
        {
            if (!_undo.TryPop(out var previous)) return null;
            _states[previous.DeviceId] = previous.PreviousState;
            var reverted = new DeviceCommand(Guid.NewGuid(), previous.DeviceId, previous.PreviousState, previous.Action, DateTime.UtcNow, true);
            _history.Add(reverted); _lastCommand[previous.DeviceId] = reverted;
            return reverted;
        }
    }
    public bool ClearAlert(string deviceId) { lock (_gate) return _activeAlerts.Remove(deviceId); }
    private string[] SuggestionsInternal()
    {
        var suggestions = new List<string>();
        foreach (var id in _activeAlerts.Take(5)) suggestions.Add($"Inspect {id}: active abnormal reading detected.");
        foreach (var entry in _actions.Where(x => x.Value >= 2).OrderByDescending(x => x.Value).Take(3))
            suggestions.Add($"Frequently used command: {entry.Key} ({entry.Value} times). Consider repeating if appropriate.");
        foreach (var entry in _searches.Where(x => x.Value >= 3).OrderByDescending(x => x.Value).Take(2))
            suggestions.Add($"Frequently searched device/category: {entry.Key}. Review its recent telemetry.");
        return suggestions.Count == 0 ? new[] { "No patterns yet. Submit telemetry and issue commands to generate insights." } : suggestions.ToArray();
    }
}
public record DeviceCommand(Guid Id, string DeviceId, string Action, string PreviousState, DateTime Timestamp, bool IsUndo);
