using System;
using System.Reactive;
using ReactiveUI;

namespace HistoricalReactiveCommand
{
    public interface IHistoryContext<out THistory, in THistoryEntry>
    where THistory: IHistory
    where THistoryEntry: IHistoryEntry
    {
        THistory History { get; }
        IObservable<bool> CanSnapshot { get; }
        ReactiveCommand<Unit, Unit> Undo { get; }
        ReactiveCommand<Unit, Unit> Redo { get; }
        ReactiveCommand<Unit, Unit> Clear { get; }
        void Snapshot(THistoryEntry entry);
    }
}