namespace HistoricalReactiveCommand
{
    public interface ITransactionalHistoryContext<TParam, TResult, out THistory, in THistoryEntry> 
        : IHistoryContext<TParam, TResult, THistory, THistoryEntry>
    
        where THistory : ITransactionalHistory<TParam, TResult>
        where THistoryEntry : IHistoryEntry<TParam, TResult>
    {
        void BeginTransaction(ITransaction<TParam, TResult> transaction);
        void CommitTransaction();
        void RollbackTransaction();
    }
}