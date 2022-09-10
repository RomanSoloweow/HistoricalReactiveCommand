using System.Collections.Generic;
using System.Linq;
using System.Reactive;

namespace HistoricalReactiveCommand
{
    public class Transaction<TParam, TResult> : ITransaction<TParam, TResult>
    {
        private List<IHistoryEntry<TParam, TResult>> Commands { get; } = new();
        
        public void Append(IHistoryEntry<TParam, TResult> entry)
        {
            Commands.Add(entry);
        }

        public void Execute()
        {
            foreach (var operation in Commands)
            {
                operation.Redo();
            }
        }

        public void Discard()
        {
            foreach (var operation in Commands)
            {
                operation.Undo();
            }
        }

        public bool IsEmpty => !Commands.Any();
        
        public IEnumerable<IHistoryEntry<TParam, TResult>> GetCommands()
        {
            return Commands;
        }
    }
    
    public class Transaction<TParam> : Transaction<TParam, Unit>
    {
        
    }
}