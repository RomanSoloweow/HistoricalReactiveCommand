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

        IObservable<THistoryEntry> Undo<THistoryEntry>(Func<IHistoryEntryBase, IObservable<THistoryEntry>> discard)
            where THistoryEntry : IHistoryEntryBase;
        
        IObservable<THistoryEntry> Redo<THistoryEntry>(Func<IHistoryEntryBase, IObservable<THistoryEntry>> execute)
            where THistoryEntry : IHistoryEntryBase;
        
        IObservable<THistoryEntry> Record<THistoryEntry>(THistoryEntry entry, Func<THistoryEntry, IObservable<THistoryEntry>> execute)
            where THistoryEntry : IHistoryEntryBase;
        
        IObservable<Unit> Clear();
    }
}
