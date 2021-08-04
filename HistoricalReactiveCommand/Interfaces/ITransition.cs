namespace HistoricalReactiveCommand
{
    public interface ITransition
    {
        public void Append(IHistoryEntry entry);
        void Execute(IHistory history);
        void Discard(IHistory history);
        bool IsEmpty { get; }
    }
}