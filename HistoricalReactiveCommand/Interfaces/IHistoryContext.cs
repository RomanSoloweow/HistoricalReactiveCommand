using System;
using System.Reactive;
using ReactiveUI;

namespace HistoricalReactiveCommand
{
    public interface IHistoryContext<TParam, TResult, out THistory, in THistoryEntry>
    where THistory: IHistory<TParam, TResult>
    where THistoryEntry: IHistoryEntry<TParam, TResult>
    {
        THistory History { get; }
        IObservable<bool> CanSnapshot { get; }
        ReactiveCommand<Unit, Unit> Undo { get; }
        ReactiveCommand<Unit, Unit> Redo { get; }
        ReactiveCommand<Unit, Unit> Clear { get; }
        void Snapshot(THistoryEntry entry);
    }
}