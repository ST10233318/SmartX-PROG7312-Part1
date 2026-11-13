using SmartX.Api.DataStructures;
namespace SmartX.Api.Services;
public static class HistoryIndex
{
    public static readonly TelemetryAvlTree Tree = new();
    public static readonly object Sync = new();
}
