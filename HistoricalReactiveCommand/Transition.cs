namespace HistoricalReactiveCommand
{
    public class Transition:ITransition
    {
        public void Append(HistoryEntry historyEntry)
        {
            throw new System.NotImplementedException();
        }

        public void Execute(IHistory history)
        {
            throw new System.NotImplementedException();
        }

        public void Discard(IHistory history)
        {
            throw new System.NotImplementedException();
        }
    }
}