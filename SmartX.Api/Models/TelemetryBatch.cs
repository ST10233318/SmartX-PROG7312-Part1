namespace SmartX.Api.Models;

public class TelemetryBatch
{
    public double[,] HistoricalMatrix { get; set; }

    public double[][] HistoricalBatches { get; set; }

    public TelemetryBatch()
    {
        HistoricalMatrix = new double[,]
        {
            { 21.5, 22.1, 22.8 },
            { 450, 470, 490 },
            { 18.2, 19.1, 20.3 }
        };

        HistoricalBatches = new double[][]
        {
            new double[] { 21.5, 22.1, 22.8 },
            new double[] { 450, 470, 490 },
            new double[] { 18.2, 19.1, 20.3 }
        };
    }
}