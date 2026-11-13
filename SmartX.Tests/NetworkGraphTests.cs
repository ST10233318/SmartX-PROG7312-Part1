using SmartX.Api.DataStructures;
using Xunit;
namespace SmartX.Tests;
public class NetworkGraphTests
{
    [Fact] public void BfsFindsConnectedNodes()
    {
        var g = new NetworkGraph(); g.AddConnection("Gateway","A",2); g.AddConnection("A","B",1); g.AddNode("Offline");
        Assert.Equal(3,g.BreadthFirstSearch("Gateway").Count);
        Assert.DoesNotContain("Offline",g.BreadthFirstSearch("Gateway"));
    }
    [Fact] public void PrimSelectsMinimumWeight()
    {
        var g=new NetworkGraph();g.AddConnection("Gateway","A",2);g.AddConnection("Gateway","B",5);
        g.AddConnection("A","B",1);g.AddConnection("B","C",3);g.AddConnection("A","C",7);
        var result=g.MinimumSpanningTree("Gateway");
        Assert.Equal(6,result.TotalWeight);Assert.Equal(3,result.Edges.Count);
        Assert.Equal(new[]{"Gateway","A","B","C"},g.GetMstRoute("Gateway","C"));
    }
    [Fact] public void MstReportsDisconnected()
    {
        var g=new NetworkGraph();g.AddConnection("Gateway","A",2);g.AddNode("Offline");
        Assert.Contains("Offline",g.MinimumSpanningTree("Gateway").UnreachableNodes);
    }
    [Fact] public void MstRejectsUnknownGateway()
    { var g=new NetworkGraph();g.AddNode("Gateway");Assert.Throws<ArgumentException>(()=>g.MinimumSpanningTree("Unknown")); }
    [Fact] public void SingleNodeHasEmptyMst()
    { var g=new NetworkGraph();g.AddNode("Gateway");Assert.Empty(g.MinimumSpanningTree("Gateway").Edges); }
    [Theory][InlineData(-1)][InlineData(0)][InlineData(double.PositiveInfinity)]
    public void RejectsInvalidWeights(double weight)
    { var g=new NetworkGraph();Assert.Throws<ArgumentOutOfRangeException>(()=>g.AddConnection("A","B",weight)); }
}
