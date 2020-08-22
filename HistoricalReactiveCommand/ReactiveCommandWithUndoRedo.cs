using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace HistoricalReactiveCommand
{
    public class ReactiveCommandWithUndoRedo<TParam, TResult> : ReactiveCommandWithUndoRedoBase<TParam, TResult>
    {
        public override IObservable<bool> CanExecute => throw new NotImplementedException();

        public override IObservable<bool> IsExecuting => throw new NotImplementedException();

        public override IObservable<Exception> ThrownExceptions => throw new NotImplementedException();

        public override IObservable<TResult> Redo()
        {
            throw new NotImplementedException();
        }

        public override IDisposable Subscribe(IObserver<TResult> observer)
        {
            throw new NotImplementedException();
        }

        public override IObservable<TResult> Undo()
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            throw new NotImplementedException();
        }
    }
}
