using SmartX.Api.Data;
using SmartX.Api.Models;

namespace SmartX.Api.Services;

public static class TelemetryProcessor
{
    public static SensorReading Process(TelemetryPacket<float> packet)
    {
        bool anomaly = packet.Value < -20 || packet.Value > 50;

        return SaveReading(
            packet.DeviceId,
            packet.SensorCategory,
            packet.Location,
            packet.Value,
            $"{packet.Value:F2} °C",
            anomaly,
            packet.Timestamp);
    }

    public static SensorReading Process(TelemetryPacket<int> packet)
    {
        bool anomaly = packet.Value < 0 || packet.Value > 10000;

        return SaveReading(
            packet.DeviceId,
            packet.SensorCategory,
            packet.Location,
            packet.Value,
            $"{packet.Value} W",
            anomaly,
            packet.Timestamp);
    }

    public static SensorReading Process(TelemetryPacket<bool> packet)
    {
        return SaveReading(
            packet.DeviceId,
            packet.SensorCategory,
            packet.Location,
            packet.Value ? 1 : 0,
            packet.Value ? "ON" : "OFF",
            false,
            packet.Timestamp);
    }

    private static SensorReading SaveReading(
        string deviceId,
        string category,
        string location,
        double numericValue,
        string displayValue,
        bool anomaly,
        DateTime timestamp)
    {
        var reading = new SensorReading
        {
            DeviceId = deviceId,
            Category = category,
            Location = location,
            NumericValue = numericValue,
            DisplayValue = displayValue,
            IsAnomaly = anomaly,
            Timestamp = timestamp
        };

        SmartXStore.Readings.Enqueue(reading);

        if (SmartXStore.Sensors.TryGetValue(deviceId, out var sensor))
        {
            sensor.LastSeen = DateTime.UtcNow;
            sensor.IsOnline = true;
        }

        return reading;
    }
}