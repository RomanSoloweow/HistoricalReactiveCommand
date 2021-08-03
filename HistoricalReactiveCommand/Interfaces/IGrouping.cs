namespace HistoricalReactiveCommand
{
    public interface IGrouping<in TParam, in TResult>
    {
        public void Append(IHistoryEntryForGroup<TParam, TResult> entry);
        IHistoryEntry Group();
        void Rollback();
    }
}