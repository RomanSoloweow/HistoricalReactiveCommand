using System;
using System.Collections.Generic;
using System.Text;

namespace HistoricalReactiveCommand.History
{
    public sealed class HistoryEntry : IHistoryEntry
    {
        public HistoryEntry(string commandKey, object parameter, object result)
        {
            Parameter = parameter;
            Result = result;
            CommandKey = commandKey;
        }

        public object Parameter { get; set; }
        public object Result { get; set; }
        public string CommandKey { get; }
    }
}
