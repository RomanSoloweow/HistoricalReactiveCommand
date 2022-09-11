using System;

namespace HistoricalReactiveCommand
{
    public interface IGrouping<TParam, TResult>
    {
        void Append(IHistoryEntry<TParam, TResult> entry);
        IHistoryEntry<TParam, TResult> Group(string commandName);
        IObservable<bool> IsEmpty { get; }
        
        //void Rollback();
    }
}