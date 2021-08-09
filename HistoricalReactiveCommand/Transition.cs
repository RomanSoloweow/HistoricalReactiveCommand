using System.Collections.Generic;
using System.Linq;

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
                operation.Redo();
            }
        }

        public void Discard(IHistory history)
        {
            foreach (var operation in Operations)
            {
                operation.Undo();
            }
        }

        public bool IsEmpty => !Operations.Any();
    }
}