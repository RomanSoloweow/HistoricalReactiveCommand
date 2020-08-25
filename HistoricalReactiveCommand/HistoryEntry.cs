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
        
        public object Parameter { get; set; }
        public object Result { get; set; }
        public string CommandKey { get; set; }
    }
}

