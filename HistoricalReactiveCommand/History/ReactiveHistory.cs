using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using HistoricalReactiveCommand.Imports;
using ReactiveUI;

namespace HistoricalReactiveCommand.History
{
    public class ReactiveHistory: IReactiveHistory, IDisposable
    {
        private readonly Subject<bool> _canUndo = new Subject<bool>();
        private readonly Subject<bool> _canRedo = new Subject<bool>();
        private readonly Subject<bool> _canClear = new Subject<bool>();

        public Stack<IReactiveHistoryElement> StackRedo { get; set; } = new Stack<IReactiveHistoryElement>();

        public Stack<IReactiveHistoryElement> StackUndo { get; set; } = new Stack<IReactiveHistoryElement>();

        public IObservable<bool> CanUndo => _canUndo.AsObservable();

        public IObservable<bool> CanRedo => _canRedo.AsObservable();

        public IObservable<bool> CanClear => _canClear.AsObservable();

        public IObservable<Unit> Undo()
        {
            if (StackUndo.Count > 0)
            {
                IReactiveHistoryElement last = StackUndo.Pop();

                //Do undo with using other special class
                //last.Undo();
            }

            return Observables.Unit;
        }

        public IObservable<Unit> Redo()
        {
            if (StackRedo.Count > 0)
            {
                IReactiveHistoryElement last = StackRedo.Pop();

                //Do redo with using other special class
                //last.Redo();
            }

            return Observables.Unit;
        }
        public IObservable<Unit> Clear()
        {
            StackRedo.Clear();
            StackUndo.Clear();

            return Observables.Unit;
        }

        public void Snapshot(IReactiveHistoryElement historyElement)
        {
            StackUndo.Push(historyElement);
        }

        public void Dispose()
        {
            StackRedo.Clear();
            StackUndo.Clear();
        }

    }
}
