namespace HistoricalReactiveCommand
{
    public interface IHistoryEntryForGroup<out TParam, out TResult> : IHistoryEntry<TParam, TResult> 
    {
    }
}