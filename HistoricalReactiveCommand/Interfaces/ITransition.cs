using System.Collections.Generic;

namespace HistoricalReactiveCommand
{
    public interface ITransaction<TParam, TResult>
    {
        void Append(IHistoryEntry<TParam, TResult> entry);
        void Execute();
        void Discard();
        bool IsEmpty { get; }
        IEnumerable<IHistoryEntry<TParam, TResult>> GetCommands();
    }
}