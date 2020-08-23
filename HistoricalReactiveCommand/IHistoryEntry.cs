namespace HistoricalReactiveCommand
{
    public interface IHistoryEntry
    {
        object Parameter { get; set; }
        object Result { get; set; }
        string CommandKey { get; }
    }
}

