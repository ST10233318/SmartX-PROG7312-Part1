using SmartX.Api.DataStructures;
using SmartX.Api.Models;
namespace SmartX.Api.Services;
public sealed class TelemetryHistoryService
{
    public List<SensorReading> Query(DateTime from, DateTime to, string? sensor)
    { lock (HistoryIndex.Sync) return HistoryIndex.Tree.Query(from, to, sensor); }
}
