using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Api.Data;

public static class MockDataSeeder
{
    public static void Seed()
    {
        RegisterSensors();

        for (int i = 0; i < 10; i++)
        {
            var temperature = new TelemetryPacket<float>
            {
                DeviceId = "ESP32-TEMP-001",
                SensorCategory = "Environmental",
                Location = "Hydroponic Farm - Zone A",
                Value = 20 + i * 0.5f,
                Timestamp = DateTime.UtcNow.AddMinutes(-i)
            };

            TelemetryProcessor.Process(temperature);
        }

        var temperatureAnomaly =
            new TelemetryPacket<float>
            {
                DeviceId = "ESP32-TEMP-001",
                SensorCategory = "Environmental",
                Location = "Hydroponic Farm - Zone A",
                Value = 82f
            };

        TelemetryProcessor.Process(
            temperatureAnomaly);

        for (int i = 0; i < 10; i++)
        {
            var power = new TelemetryPacket<int>
            {
                DeviceId = "ESP32-POWER-001",
                SensorCategory = "Power Consumption",
                Location = "Smart Grid - Node 1",
                Value = 400 + (i * 25),
                Timestamp = DateTime.UtcNow.AddMinutes(-i)
            };

            TelemetryProcessor.Process(power);
        }

        var powerAnomaly =
            new TelemetryPacket<int>
            {
                DeviceId = "ESP32-POWER-001",
                SensorCategory = "Power Consumption",
                Location = "Smart Grid - Node 1",
                Value = 12500
            };

        TelemetryProcessor.Process(powerAnomaly);

        TelemetryProcessor.Process(
            new TelemetryPacket<bool>
            {
                DeviceId = "ESP32-SWITCH-001",
                SensorCategory = "Actuator",
                Location = "Farm Valve A",
                Value = true
            });
    }

    private static void RegisterSensors()
    {
        SmartXStore.Sensors.TryAdd(
            "ESP32-TEMP-001",
            new Sensor
            {
                DeviceId = "ESP32-TEMP-001",
                DeploymentLocation =
                    "Hydroponic Farm - Zone A",
                Category = "Environmental"
            });

        SmartXStore.Sensors.TryAdd(
            "ESP32-POWER-001",
            new Sensor
            {
                DeviceId = "ESP32-POWER-001",
                DeploymentLocation =
                    "Smart Grid - Node 1",
                Category = "Power Consumption"
            });

        SmartXStore.Sensors.TryAdd(
            "ESP32-SWITCH-001",
            new Sensor
            {
                DeviceId = "ESP32-SWITCH-001",
                DeploymentLocation =
                    "Farm Valve A",
                Category = "Actuator"
            });
    }
}