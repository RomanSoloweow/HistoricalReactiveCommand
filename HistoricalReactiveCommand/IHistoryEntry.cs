namespace HistoricalReactiveCommand
{
    public interface IHistoryEntry
    {
        object? Parameter { get; }
        object? Result { get; }
        string CommandKey { get; }
    }
}

