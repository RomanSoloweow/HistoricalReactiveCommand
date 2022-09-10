using System;

namespace HistoricalReactiveCommand
{
    public interface IHistoryEntry<out TParam, out TResult>
    {
        Action Undo { get; }
        Action Redo { get; }
        
        TParam Param { get; }
        TResult Result { get; }
    }
}