using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using ReactiveUI;
using Splat;

namespace HistoricalReactiveCommand
{
    public sealed class HistoryContext : IDisposable
    {
        internal HistoryContext(IHistory history, IScheduler outputScheduler)
        {
            Undo = ReactiveCommand.CreateFromObservable<Unit, HistoryEntry>(
                unit => history.Undo(entry => ResolveCommand(entry).Discard(entry)), 
                history.CanUndo, 
                outputScheduler);
            
            Redo = ReactiveCommand.CreateFromObservable<Unit, HistoryEntry>(
                unit => history.Redo(entry => ResolveCommand(entry).Execute(entry)),
                history.CanRedo,
                outputScheduler);
            
            Clear = ReactiveCommand.CreateFromObservable(
                history.Clear, 
                history.CanClear, 
                outputScheduler);
        }
        
        public ReactiveCommand<Unit, HistoryEntry> Undo { get; }
        
        public ReactiveCommand<Unit, HistoryEntry> Redo { get; }
        
        public ReactiveCommand<Unit, Unit> Clear { get; }

        public void Dispose()
        {
            Undo.Dispose();
            Redo.Dispose();
            Clear.Dispose();
        }

        private static IReactiveCommandWithHistory ResolveCommand(HistoryEntry entry)
        {
            if (string.IsNullOrWhiteSpace(entry.CommandKey))
                throw new ArgumentException("Command key is null.", nameof(entry));
            
            var command = Locator.Current.GetService<IReactiveCommandWithHistory>(entry.CommandKey);
            if (command == null)
                throw new KeyNotFoundException($"Command with {entry.CommandKey} wasn't registered.");
            return command;
        }
    }
}
