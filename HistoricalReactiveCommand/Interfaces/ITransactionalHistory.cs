namespace HistoricalReactiveCommand
{
    public interface ITransactionalHistory<TParam, TResult> : IHistory<TParam, TResult>
    {
        void BeginTransaction(ITransaction<TParam, TResult> transaction);
        void CommitTransaction();
        void RollbackTransaction();
    }
}