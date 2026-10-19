using SmartX.Api.Models;
namespace SmartX.Api.Services;
public static class RealtimeBridge
{
    public static Action<SensorReading>? OnReading { get; set; }
}
