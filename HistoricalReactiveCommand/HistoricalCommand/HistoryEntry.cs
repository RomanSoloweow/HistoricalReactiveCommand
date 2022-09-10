namespace HistoricalReactiveCommand
{
    public class HistoryEntry<TParam, TResult> : IHistoryEntry<TParam, TResult>
    {
        public HistoryEntry(string commandKey, TParam parameter, TResult result)
        {
            CommandKey = commandKey;
            Parameter = parameter;
            Result = result;
        }
        
        public TParam Parameter { get; }
        public TResult Result { get; set; }
        public string CommandKey { get; set; }
    }
}

