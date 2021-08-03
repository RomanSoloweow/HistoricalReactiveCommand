using System;

namespace HistoricalReactiveCommand
{
    public interface ITransition
    {
        public void Append(Action undo, Action redo);
        void Execute(IHistory history);
        void Discard(IHistory history);
    }
}