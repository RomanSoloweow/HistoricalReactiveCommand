namespace HistoricalReactiveCommand
{
    public interface IGrouping<TParam, TResult>
    {
        public void Append(IHistoryEntryForGroup<TParam, TResult> entry);
        IHistoryEntryForGroup<TParam, TResult> Group();
        void Rollback();
        bool IsEmpty { get; }
    }
}