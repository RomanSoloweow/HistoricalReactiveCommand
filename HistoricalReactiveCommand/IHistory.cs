using System;
using System.Reactive;

namespace HistoricalReactiveCommand
{
    public interface IHistory
    {
        string Id { get; }
        IObservable<bool> CanUndo { get; }
        IObservable<bool> CanRedo { get; }
        IObservable<bool> CanRecord { get; }
        IObservable<bool> CanClear { get; }

        IObservable<HistoryEntry> Undo(Func<HistoryEntry, IObservable<HistoryEntry>> discard);
        IObservable<HistoryEntry> Redo(Func<HistoryEntry, IObservable<HistoryEntry>> execute);
        IObservable<HistoryEntry> Snapshot(HistoryEntry entry, Func<HistoryEntry, IObservable<HistoryEntry>> execute);
        IObservable<Unit> Clear();
    }
}
