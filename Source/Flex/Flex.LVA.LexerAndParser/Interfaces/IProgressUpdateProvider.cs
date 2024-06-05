
namespace Flex.LVA.Core.Interfaces;

using Flex.LVA.Shared;

public class ProgressData
{
    public ProgressData(ILogGraph theLogGraph, Range theRange, int theProcessedDataLength, int theTotalDataLength)
    {
        this.LogGraph = theLogGraph;
        this.ProcessedDataLength = theProcessedDataLength;
        this.TotalDataLength = theTotalDataLength;
        this.LineRange = theRange;
    }

    public int TotalDataLength { get; }

    public int ProcessedDataLength { get; }

    public float ProgressPercentage => (this.ProcessedDataLength / (float)this.TotalDataLength) * 100.0f;

    public ILogGraph LogGraph { get; }

    public Range LineRange { get; }
}

public interface IProgressUpdateProvider
{
    event EventHandler<ILogHeader> HeaderReady;

    event EventHandler<ProgressData> LogChunkReady;

    event EventHandler JobFinished;
}