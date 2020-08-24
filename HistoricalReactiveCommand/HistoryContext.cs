using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using ReactiveUI;
using Splat;

namespace HistoricalReactiveCommand
{
    public static class History
    {
        public static HistoryContext GetContext(IHistory? history = null, IScheduler? outputScheduler = null)
        {
            var resolvedHistory = history ?? Locator.Current.GetService<IHistory>();
            var existingContext = Locator.Current.GetService<HistoryContext>(resolvedHistory.Name);
            if (existingContext != null) return existingContext;
            
            var context = new HistoryContext(resolvedHistory, outputScheduler ?? RxApp.MainThreadScheduler);
            Locator.CurrentMutable.RegisterConstant(context, typeof(HistoryContext), resolvedHistory.Name);
            return context;
        }
    }
    
    public sealed class HistoryContext : IDisposable
    {
        internal HistoryContext(IHistory history, IScheduler outputScheduler)
        {
            CanRecord = history.CanRecord;
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
        
        public IObservable<bool> CanRecord { get; }
        
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
