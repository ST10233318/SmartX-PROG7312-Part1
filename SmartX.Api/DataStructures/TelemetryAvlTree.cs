using SmartX.Api.Models;

namespace SmartX.Api.DataStructures;

// AVL tree keyed by UTC timestamp; duplicate timestamps retain all readings.
public sealed class TelemetryAvlTree
{
    private sealed class Node
    {
        public DateTime Key;
        public List<SensorReading> Values;
        public Node? Left;
        public Node? Right;
        public int Height = 1;
        public Node(SensorReading reading)
        {
            Key = reading.Timestamp.ToUniversalTime();
            Values = new() { reading };
        }
    }
    private Node? _root;
    private static int Height(Node? n) => n?.Height ?? 0;
    private static void Update(Node n) => n.Height = 1 + Math.Max(Height(n.Left), Height(n.Right));
    private static Node RotateRight(Node n)
    {
        var x = n.Left!;
        n.Left = x.Right;
        x.Right = n;
        Update(n); Update(x);
        return x;
    }
    private static Node RotateLeft(Node n)
    {
        var x = n.Right!;
        n.Right = x.Left;
        x.Left = n;
        Update(n); Update(x);
        return x;
    }
    private static Node Insert(Node? node, SensorReading reading)
    {
        if (node is null) return new Node(reading);
        int comparison = reading.Timestamp.ToUniversalTime().CompareTo(node.Key);
        if (comparison == 0) { node.Values.Add(reading); return node; }
        if (comparison < 0) node.Left = Insert(node.Left, reading);
        else node.Right = Insert(node.Right, reading);
        Update(node);
        int balance = Height(node.Left) - Height(node.Right);
        if (balance > 1)
        {
            if (Height(node.Left!.Left) < Height(node.Left.Right)) node.Left = RotateLeft(node.Left);
            return RotateRight(node);
        }
        if (balance < -1)
        {
            if (Height(node.Right!.Right) < Height(node.Right.Left)) node.Right = RotateRight(node.Right);
            return RotateLeft(node);
        }
        return node;
    }
    public void Add(SensorReading reading) => _root = Insert(_root, reading);
    public List<SensorReading> Query(DateTime fromUtc, DateTime toUtc, string? deviceId = null)
    {
        fromUtc = fromUtc.ToUniversalTime();
        toUtc = toUtc.ToUniversalTime();
        if (fromUtc > toUtc) throw new ArgumentException("From must not exceed to.");
        var result = new List<SensorReading>();
        void Walk(Node? node)
        {
            if (node is null) return;
            if (fromUtc < node.Key) Walk(node.Left);
            if (node.Key >= fromUtc && node.Key <= toUtc)
                result.AddRange(node.Values.Where(x => deviceId is null ||
                    string.Equals(x.DeviceId, deviceId, StringComparison.OrdinalIgnoreCase)));
            if (toUtc > node.Key) Walk(node.Right);
        }
        Walk(_root);
        return result;
    }
}
