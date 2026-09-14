using System.Collections.Concurrent;
using SmartX.Api.Models;

namespace SmartX.Api.Data;

public static class SmartXStore
{
    public static ConcurrentDictionary<string, Sensor> Sensors { get; }
        = new();

    public static ConcurrentQueue<SensorReading> Readings { get; }
        = new();
}