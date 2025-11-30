namespace AutoOS.Views.Settings.Scheduling.Models;

public class CpuSet
{
    public uint Id { get; set; }
    public byte CoreIndex { get; set; }
    public byte LogicalProcessorIndex { get; set; }
    public byte LastLevelCacheIndex { get; set; }
    public byte EfficiencyClass { get; set; }
    public byte NumaNodeIndex { get; set; }
}

