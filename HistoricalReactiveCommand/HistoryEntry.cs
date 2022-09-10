using System;

namespace HistoricalReactiveCommand
{
    public class HistoryEntry<TParam, TResult> : IHistoryEntry<TParam, TResult>
    {
        public HistoryEntry(string commandKey, TParam param, TResult result)
        {
            Param = param;
            Result = result;
            CommandKey = commandKey;
        }
        
        public TParam Param { get; }
        public TResult Result { get; }
        public string CommandKey { get; }
    }
}

