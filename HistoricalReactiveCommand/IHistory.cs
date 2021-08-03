using System;
using System.Reactive;

namespace HistoricalReactiveCommand
{
    public interface IHistory: IDisposable
    {
        string Id { get; }
        IObservable<bool> CanUndo { get; }
        IObservable<bool> CanRedo { get; }
        IObservable<bool> CanRecord { get; }
        IObservable<bool> CanClear { get; }

        
        void Undo();
        void Redo();
        void Snapshot(Action undo, Action redo);
        void Clear();
        // IObservable<HistoryEntry> Undo(Func<HistoryEntry, IObservable<HistoryEntry>> discard);
        // IObservable<HistoryEntry> Redo(Func<HistoryEntry, IObservable<HistoryEntry>> execute);
        // IObservable<HistoryEntry> Snapshot(HistoryEntry entry, Func<HistoryEntry, IObservable<HistoryEntry>> execute);
        // IObservable<Unit> Clear();
    }
}
