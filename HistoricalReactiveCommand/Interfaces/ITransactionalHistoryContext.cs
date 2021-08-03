namespace HistoricalReactiveCommand
{
    public interface ITransactionalHistoryContext<out THistory, in THistoryEntry>:IHistoryContext<THistory, THistoryEntry>
        where THistory : ITransactionalHistory
        where THistoryEntry : IHistoryEntry
    {
        
    }
}