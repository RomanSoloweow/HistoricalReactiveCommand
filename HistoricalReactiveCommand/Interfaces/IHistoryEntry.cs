using System;

namespace HistoricalReactiveCommand
{
    public interface IHistoryEntry<out TParam, out TResult>
    {
        TParam Param { get; }
        TResult Result { get; }
        string CommandKey { get; }
    }
}