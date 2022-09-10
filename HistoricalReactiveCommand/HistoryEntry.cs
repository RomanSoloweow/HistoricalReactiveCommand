using System;

namespace HistoricalReactiveCommand
{
    public class HistoryEntry<TParam, TResult> : IHistoryEntry<TParam, TResult>
    {
        public HistoryEntry(Action<HistoryEntry<TParam, TResult>> undo, Action<HistoryEntry<TParam, TResult> > redo, TParam param, TResult result)
        {
            Param = param;
            Result = result;
            Undo = () => undo(this);
            Redo = () => redo(this);
        }
        public Action Undo { get; }
        public Action Redo { get; }
        public TParam Param { get; }
        public TResult Result { get; }
    }
}

