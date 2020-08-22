using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;

namespace HistoricalReactiveCommand
{
    public abstract class ReactiveCommandWithUndoRedoBase<TParam, TResult> : ReactiveCommandBase<TParam, TResult>, IReactiveCommandWithUndoRedo
    {
        public ReactiveCommandWithUndoRedoBase(IReactiveCommandHistory history=null)
        {
            _history = history;
        }

        private IReactiveCommandHistory _history;
        public IObservable<bool> IsUndoing => throw new NotImplementedException();

        public IObservable<bool> IsRedoing => throw new NotImplementedException();

        public IObservable<bool> CanUndo => throw new NotImplementedException();

        public IObservable<bool> CanRedo => throw new NotImplementedException();

        public override IObservable<TResult> Execute(TParam parameter = default(TParam))
        {
            return null;
        }

        void IReactiveCommandWithUndoRedo.Undo()
        {
            IUndoExecute();
        }

        void IReactiveCommandWithUndoRedo.Redo()
        {
            IRedoExecute();
        }

        public abstract IObservable<TResult> Undo();
        public abstract IObservable<TResult> Redo();

        protected virtual void IUndoExecute()
        {
            Undo().Catch(Observable<TResult>.Empty)
                .Subscribe();
        }
        protected virtual void IRedoExecute()
        {
            Redo().Catch(Observable<TResult>.Empty)
                .Subscribe();
        }
    }
}
