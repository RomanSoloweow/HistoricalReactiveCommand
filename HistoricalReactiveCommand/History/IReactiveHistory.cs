using System;
using System.Reactive;
using ReactiveUI;

namespace HistoricalReactiveCommand.History
{
    public interface IReactiveHistory
    {
        IObservable<bool> CanUndo { get; }
        IObservable<bool> CanRedo { get; }
        IObservable<bool> CanClear { get; }

        IObservable<Unit> Undo();
        IObservable<Unit> Redo();
        IObservable<Unit> Clear();

        void Snapshot(IReactiveHistoryElement historyElement);
    }

}
