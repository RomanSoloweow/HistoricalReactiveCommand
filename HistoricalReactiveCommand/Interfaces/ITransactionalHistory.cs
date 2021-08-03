namespace HistoricalReactiveCommand
{
    public interface ITransactionalHistory:IHistory
    {
        void BeginTransaction(ITransition transition);
        void CommitTransaction();
        void RollbackTransaction();
    }
}