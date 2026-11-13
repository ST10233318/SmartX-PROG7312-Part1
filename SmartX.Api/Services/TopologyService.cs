using SmartX.Api.Data;
using SmartX.Api.DataStructures;

namespace SmartX.Api.Services;

public sealed class TopologyService
{
    private readonly object _sync = new();
    private readonly NetworkGraph _graph = new();
    public TopologyService() { _graph.AddNode("Gateway"); }
    public void AddNode(string id) { lock (_sync) _graph.AddNode(id); }
    public void AddConnection(string from, string to, double weight)
    { lock (_sync) _graph.AddConnection(from, to, weight); }
    public object Snapshot(string gateway)
    {
        lock (_sync)
        {
            var nodes = _graph.GetNodes().OrderBy(x => x).ToArray();
            var edges = nodes.SelectMany(a => _graph.GetNeighbours(a)
                .Where(p => string.Compare(a, p.Key, StringComparison.OrdinalIgnoreCase) < 0)
                .Select(p => new MstEdge(a, p.Key, p.Value))).ToArray();
            var mst = _graph.MinimumSpanningTree(gateway);
            var connected = _graph.BreadthFirstSearch(gateway);
            return new { nodes, edges, mst, connected,
                offline = nodes.Except(connected, StringComparer.OrdinalIgnoreCase).ToArray(),
                sensorHealth = SmartXStore.Sensors.Values.Select(s => new
                { s.DeviceId, s.IsOnline, s.LastSeen }).ToArray() };
        }
    }
    public List<string> Route(string gateway, string destination)
    { lock (_sync) return _graph.GetMstRoute(gateway, destination); }
}
