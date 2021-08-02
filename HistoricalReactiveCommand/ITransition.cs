namespace HistoricalReactiveCommand
{
    public interface ITransition
    {
        public void Append(HistoryEntry historyEntry);
        void Execute(IHistory history);
        void Discard(IHistory history);
    }
}