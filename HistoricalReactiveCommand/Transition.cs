using System;
using System.Collections.Generic;

namespace HistoricalReactiveCommand
{
    public class Transition:ITransition
    {
        private List<IHistoryEntry> Operations { get; } = new();
        public void Append(IHistoryEntry entry)
        {
            Operations.Add(entry);
        }

        public void Execute(IHistory history)
        {
            foreach (var operation in Operations)
            {
                operation.Undo();
            }
        }

        public void Discard(IHistory history)
        {
            foreach (var operation in Operations)
            {
                operation.Redo();
            }
        }
        
        
    }
}