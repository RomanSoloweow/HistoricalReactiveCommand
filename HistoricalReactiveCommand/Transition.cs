using System;

namespace HistoricalReactiveCommand
{
    public class Transition:ITransition
    {
        public void Append(Action undo, Action redo)
        {
            throw new NotImplementedException();
        }

        public void Execute(IHistory history)
        {
            throw new NotImplementedException();
        }

        public void Discard(IHistory history)
        {
            throw new NotImplementedException();
        }
    }
}