using System;

namespace HistoricalReactiveCommand
{
    public class HistoryEntryForGroup<TParam, TResult> : IHistoryEntryForGroup<TParam, TResult>
    {
        public TParam Param { get; }
        public TResult Result { get; }
        public Action Undo { get; }
        public Action Redo { get; }

        public HistoryEntryForGroup(Action<HistoryEntryForGroup<TParam, TResult>> undo,
            Action<HistoryEntryForGroup<TParam, TResult>> redo, TParam param, TResult result)
        {
            Param = param;
            Result = result;
            Undo = () => undo(this);
            Redo = () => undo(this);
        }
    }
}