using System;

namespace HistoricalReactiveCommand
{
    public class HistoryEntryForGroup<TParam, TResult> : IHistoryEntryForGroup<TParam, TResult>
    {
        public HistoryEntryForGroup(string commandKey, TParam param, TResult result)
        {
            CommandKey = commandKey;
            Param = param;
            Result = result;
        }
                
        public TParam Param { get; }
        public TResult Result { get; }
        public string CommandKey { get; }
    }
}