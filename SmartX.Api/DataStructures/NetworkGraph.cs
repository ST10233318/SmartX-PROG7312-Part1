namespace SmartX.Api.DataStructures;

public sealed record MstEdge(string From, string To, double Weight);
public sealed record MstResult(List<MstEdge> Edges, double TotalWeight, List<string> UnreachableNodes);

public sealed class NetworkGraph
{
    private readonly Dictionary<string, Dictionary<string, double>> _adjacency =
        new(StringComparer.OrdinalIgnoreCase);

    public void AddNode(string nodeId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nodeId);
        if (!_adjacency.ContainsKey(nodeId))
            _adjacency[nodeId] = new(StringComparer.OrdinalIgnoreCase);
    }

    public void AddConnection(string nodeA, string nodeB, double weight)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nodeA);
        ArgumentException.ThrowIfNullOrWhiteSpace(nodeB);
        if (string.Equals(nodeA, nodeB, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("A node cannot connect to itself.");
        if (!double.IsFinite(weight) || weight <= 0)
            throw new ArgumentOutOfRangeException(nameof(weight));
        AddNode(nodeA);
        AddNode(nodeB);
        _adjacency[nodeA][nodeB] = weight;
        _adjacency[nodeB][nodeA] = weight;
    }

    public IReadOnlyCollection<string> GetNodes() => _adjacency.Keys.ToArray();
    public IReadOnlyDictionary<string, double> GetNeighbours(string nodeId) =>
        _adjacency.TryGetValue(nodeId, out var neighbours)
            ? new Dictionary<string, double>(neighbours, StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

    public List<string> BreadthFirstSearch(string startNode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(startNode);
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var result = new List<string>();
        if (!_adjacency.ContainsKey(startNode)) return result;
        var queue = new Queue<string>();
        queue.Enqueue(startNode);
        visited.Add(startNode);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            result.Add(current);
            foreach (var neighbour in _adjacency[current].Keys)
                if (visited.Add(neighbour)) queue.Enqueue(neighbour);
        }
        return result;
    }

    public MstResult MinimumSpanningTree(string startNode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(startNode);
        if (!_adjacency.ContainsKey(startNode))
            throw new ArgumentException("The starting node does not exist.", nameof(startNode));
        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { startNode };
        var selected = new List<MstEdge>();
        var queue = new PriorityQueue<MstEdge, double>();
        void EnqueueEdges(string node)
        {
            foreach (var pair in _adjacency[node])
                if (!visited.Contains(pair.Key))
                    queue.Enqueue(new MstEdge(node, pair.Key, pair.Value), pair.Value);
        }
        EnqueueEdges(startNode);
        double total = 0;
        while (queue.Count > 0)
        {
            var edge = queue.Dequeue();
            if (!visited.Add(edge.To)) continue;
            selected.Add(edge);
            total += edge.Weight;
            EnqueueEdges(edge.To);
        }
        return new MstResult(selected, total,
            _adjacency.Keys.Where(x => !visited.Contains(x)).ToList());
    }

    // The MST forms a tree; recover the unique route from the gateway to a node.
    public List<string> GetMstRoute(string gateway, string destination)
    {
        var mst = MinimumSpanningTree(gateway);
        var tree = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var edge in mst.Edges)
        {
            if (!tree.ContainsKey(edge.From)) tree[edge.From] = new();
            if (!tree.ContainsKey(edge.To)) tree[edge.To] = new();
            tree[edge.From].Add(edge.To);
            tree[edge.To].Add(edge.From);
        }
        var parent = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase) { [gateway] = null };
        var queue = new Queue<string>();
        queue.Enqueue(gateway);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (string.Equals(current, destination, StringComparison.OrdinalIgnoreCase))
            {
                var path = new List<string>();
                string? cursor = current;
                while (cursor is not null) { path.Add(cursor); cursor = parent[cursor]; }
                path.Reverse();
                return path;
            }
            if (!tree.TryGetValue(current, out var neighbours)) continue;
            foreach (var next in neighbours)
                if (!parent.ContainsKey(next)) { parent[next] = current; queue.Enqueue(next); }
        }
        return new();
    }
}
