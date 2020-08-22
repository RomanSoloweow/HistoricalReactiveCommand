using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;

namespace HistoricalReactiveCommand.History
{
    public class ReactiveHistory: IReactiveHistory, IDisposable
    {
        public ReactiveHistory(IObservable<bool>? canUndo, IObservable<bool>? canRedo, IObservable<bool>? canClear, IScheduler? scheduler)
        {
            Undo = ReactiveCommand.Create(UndoCommand, CanUndo, scheduler);
            Redo = ReactiveCommand.Create(RedoCommand, CanRedo, scheduler);
            Clear = ReactiveCommand.Create(ClearHistory, CanClear, scheduler);



            var commandsForUndoCanExecute = Observable
                .CombineLatest(StackUndo.Select(x => x.CanExecute))
                .Select(x => x.All(y => y));

            var commandsForRedoCanExecute = Observable
               .CombineLatest(StackRedo.Select(x => x.CanExecute))
               .Select(x => x.All(y => y));

            var commandsForUndoCanUndo = Observable
                .CombineLatest(StackUndo.Select(x => x.CanUndo))
                .Select(x => x.All(y => y));

            var commandsForRedoCanUndo = Observable
                  .CombineLatest(StackRedo.Select(x => x.CanUndo))
                  .Select(x => x.All(y => y));

            var commandsForUndoCanRedo = Observable
                .CombineLatest(StackUndo.Select(x => x.CanRedo))
                .Select(x => x.All(y => y));

            var commandsForRedoCanRedo = Observable
              .CombineLatest(StackRedo.Select(x => x.CanRedo))
              .Select(x => x.All(y => y));



            var commandsForUndoIsExecuting = Observable
                .CombineLatest(StackUndo.Select(x => x.IsExecuting))
                .Select(x => x.All(y => y));

            var commandsForRedoIsExecuting = Observable
               .CombineLatest(StackRedo.Select(x => x.IsExecuting))
               .Select(x => x.All(y => y));

            var commandsForUndoIsUndoing = Observable
                .CombineLatest(StackUndo.Select(x => x.IsUndoing))
                .Select(x => x.All(y => y));

            var commandsForRedoIsUndoing = Observable
                .CombineLatest(StackRedo.Select(x => x.IsUndoing))
                .Select(x => x.All(y => y));

            var commandsForUndoIsRedoing = Observable
                .CombineLatest(StackUndo.Select(x => x.IsRedoing))
                .Select(x => x.All(y => y));

            var commandsForRedoIsRedoing = Observable
                .CombineLatest(StackRedo.Select(x => x.IsRedoing))
                .Select(x => x.All(y => y));



        }

        public Stack<IReactiveCommandWithHistory> StackRedo { get; set; } = new Stack<IReactiveCommandWithHistory>();

        public Stack<IReactiveCommandWithHistory> StackUndo { get; set; } = new Stack<IReactiveCommandWithHistory>();

        public ReactiveCommand<Unit, Unit> Undo { get; }

        public ReactiveCommand<Unit, Unit> Redo { get; }

        public ReactiveCommand<Unit, Unit> Clear { get; }
        
        public void AddInRedo<TParam, TResult>(IReactiveHistoryElement<TParam, TResult> element)
        {
            throw new NotImplementedException();
        }

        public void AddInUndo<TParam, TResult>(IReactiveHistoryElement<TParam, TResult> element)
        {
            throw new NotImplementedException();
        }

        public IObservable<bool> CanUndo { get; }
        public IObservable<bool> CanRedo { get; }
        public IObservable<bool> CanClear { get; }

        public IObservable<bool> IsUndoing { get; }
        public IObservable<bool> IsRedoing { get; }
        public IObservable<bool> IsClearing { get; }

        public IReactiveCommandWithHistory AddInRedo(IReactiveCommandWithHistory command)
        {
            StackRedo.Push(command);
            return command;
        }

        public IReactiveCommandWithHistory AddInUndo(IReactiveCommandWithHistory command)
        {
            StackUndo.Push(command);
            return command;
        }

        protected void RedoCommand()
        {
            if (StackRedo.Count > 0)
            {
                IReactiveCommandWithHistory last = StackRedo.Pop();
                last.Redo();
            }
        }
        protected void UndoCommand()
        {
            if (StackUndo.Count > 0)
            {
                IReactiveCommandWithHistory last = StackUndo.Pop();
                last.Undo();
            }
        }
        protected void ClearHistory()
        {
            StackRedo.Clear();
            StackUndo.Clear();
        }

        public void Dispose()
        {
            StackRedo.Clear();
            StackUndo.Clear();
            Undo.Dispose();
            Redo.Dispose();
            Clear.Dispose();
        }
    }
}
