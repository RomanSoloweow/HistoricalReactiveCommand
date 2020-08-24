namespace HistoricalReactiveCommand
{
    public class HistoryEntry
    {
        public HistoryEntry(object parameter, object result, string commandKey)
        {
            Parameter = parameter;
            Result = result;
            CommandKey = commandKey;
        }
        
        public object Parameter { get; }
        public object Result { get; }
        public string CommandKey { get; }
    }
}

