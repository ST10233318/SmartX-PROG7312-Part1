using SmartX.Api.DataStructures;
using SmartX.Api.Models;
using Xunit;
namespace SmartX.Tests;
public class TelemetryAvlTreeTests
{
    [Fact] public void RangeQueryFiltersByTimeAndSensor()
    {
        var tree=new TelemetryAvlTree();var now=DateTime.UtcNow;
        for(int i=0;i<100;i++) tree.Add(new SensorReading { DeviceId=i%2==0?"A":"B",Timestamp=now.AddMinutes(i) });
        var rows=tree.Query(now.AddMinutes(10),now.AddMinutes(20),"A");
        Assert.Equal(6,rows.Count);
    }
    [Fact] public void DuplicateTimestampRetainsReadings()
    {
        var tree=new TelemetryAvlTree();var now=DateTime.UtcNow;
        tree.Add(new SensorReading { DeviceId="A",Timestamp=now });
        tree.Add(new SensorReading { DeviceId="B",Timestamp=now });
        Assert.Equal(2,tree.Query(now,now).Count);
    }
    [Fact] public void InvalidRangeIsRejected()
    {var tree=new TelemetryAvlTree();Assert.Throws<ArgumentException>(()=>tree.Query(DateTime.UtcNow,DateTime.UtcNow.AddDays(-1)));}
}
