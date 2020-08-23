using System;

namespace HistoricalReactiveCommand
{
    public interface IHistory
    {
        IObservable<bool> CanUndo { get; }
        IObservable<bool> CanRedo { get; }
        IObservable<bool> CanClear { get; }

        IHistoryEntry Undo();
        IHistoryEntry Redo();

        void Snapshot(object parameter, object result, string commandKey);
        void Clear();
    }
}
