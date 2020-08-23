using System;
using System.Reactive;

namespace HistoricalReactiveCommand
{
    public interface IHistory
    {
        IObservable<bool> CanUndo { get; }
        IObservable<bool> CanRedo { get; }
        IObservable<bool> CanClear { get; }

        IObservable<Unit> Undo(ICommandExecutor executor);
        IObservable<Unit> Redo(ICommandExecutor executor);
        IObservable<Unit> Clear();

        void Snapshot(IHistoryEntry entry);
       
    }
}
