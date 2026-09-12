namespace SmartX.Client.Models;

public class DashboardViewModel
{
    public int TotalSensors { get; set; }

    public int OnlineSensors { get; set; }

    public int TotalReadings { get; set; }

    public int Anomalies { get; set; }

    public List<TelemetryViewModel> LatestReadings { get; set; }
        = new();
}

public class TelemetryViewModel
{
    public string DeviceId { get; set; } = "";

    public string Category { get; set; } = "";

    public string Location { get; set; } = "";

    public double NumericValue { get; set; }

    public string DisplayValue { get; set; } = "";

    public bool IsAnomaly { get; set; }

    public DateTime Timestamp { get; set; }
}