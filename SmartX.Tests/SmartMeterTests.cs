using SmartX.Api.Models;

namespace SmartX.Tests;

public class SmartMeterTests
{
    [Fact]
    public void TwoMetersCanBeAdded()
    {
        var meter1 =
            new SmartMeterValue(100);

        var meter2 =
            new SmartMeterValue(200);

        var result =
            meter1 + meter2;

        Assert.Equal(300, result.Value);
    }

    [Fact]
    public void MeterComparisonWorks()
    {
        var meter1 =
            new SmartMeterValue(500);

        var meter2 =
            new SmartMeterValue(300);

        Assert.True(meter1 > meter2);
    }
}