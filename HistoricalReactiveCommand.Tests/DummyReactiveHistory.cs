using System;
using System.Reactive;
using HistoricalReactiveCommand.History;
using ReactiveUI;

namespace HistoricalReactiveCommand.Tests
{
    public class DummyReactiveHistory : IReactiveHistory
    {
        public IObservable<bool> CanUndo { get; }
        public IObservable<bool> IsUndoing { get; }
        public IObservable<bool> CanRedo { get; }
        public IObservable<bool> IsRedoing { get; }
        public IObservable<bool> CanClear { get; }
        public IObservable<bool> IsClearing { get; }
        public ReactiveCommand<Unit, Unit> Undo { get; }
        public ReactiveCommand<Unit, Unit> Redo { get; }
        public ReactiveCommand<Unit, Unit> Clear { get; }
        public IReactiveCommandWithHistory AddInRedo(IReactiveCommandWithHistory command) => command;
        public IReactiveCommandWithHistory AddInUndo(IReactiveCommandWithHistory command) => command;
    }
}