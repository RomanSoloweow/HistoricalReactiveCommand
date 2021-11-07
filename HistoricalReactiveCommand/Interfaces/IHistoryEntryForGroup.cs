namespace HistoricalReactiveCommand
{
    public interface IHistoryEntryForGroup<out TParam, out TResult> : IHistoryEntry
    {
        TParam Param { get; }
        TResult Result { get; }
    }
}