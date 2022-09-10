using System;

namespace HistoricalReactiveCommand
{
    public interface IHistory<in TParam, in TResult>: IDisposable
    {
        string Id { get; }
        IObservable<bool> CanUndo { get; }
        IObservable<bool> CanRedo { get; }
        IObservable<bool> CanSnapshot { get; }
        IObservable<bool> CanClear { get; }
        
        void Undo();
        void Redo();
        void Snapshot(IHistoryEntry<TParam, TResult> entry);
        void Clear();

    }
}
