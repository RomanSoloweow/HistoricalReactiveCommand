using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;

namespace HistoricalReactiveCommand
{
    public static class TransactionalHistoryContext
    {
        internal static ITransactionalHistoryContext<TParam, TResult, ITransactionalHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>> GetContext<TParam, TResult>(ITransactionalHistory<TParam, TResult> history, IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
            }
        
            if (history.Id == null)
            {
                throw new ArgumentNullException(nameof(history.Id));
            }
        
            var context = Locator.Current.GetService<ITransactionalHistoryContext<TParam, TResult, ITransactionalHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>>>(history.Id);
        
            if (context != null)
            {
                return context;
            }
        
            context = new TransactionalHistoryContext<TParam, TResult>(history, outputScheduler ?? RxApp.MainThreadScheduler);
            
            Locator.CurrentMutable.RegisterConstant(context, typeof(ITransactionalHistoryContext<TParam, TResult,ITransactionalHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>>), history.Id);
            Locator.CurrentMutable.RegisterConstant(context, typeof(IHistoryContext<TParam, TResult,IHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>>), history.Id);
            
            return context;
        }

        internal static ITransactionalHistoryContext<TParam, TResult, ITransactionalHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>> GetContext<TParam, TResult>(string historyId = "", IScheduler? outputScheduler = null)
        {
            if (historyId == null)
            {
                throw new ArgumentNullException(nameof(historyId));
            }
        
            var context = Locator.Current.GetService<ITransactionalHistoryContext<TParam, TResult, ITransactionalHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>>>(historyId);
        
            if (context != null)
            {
                return context;
            }
        
            context = new TransactionalHistoryContext<TParam, TResult>(new TransactionalHistory<TParam, TResult>(historyId), outputScheduler ?? RxApp.MainThreadScheduler);
            
            Locator.CurrentMutable.RegisterConstant(context, typeof(ITransactionalHistoryContext<TParam, TResult, ITransactionalHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>>), historyId);
            Locator.CurrentMutable.RegisterConstant(context, typeof(IHistoryContext<TParam, TResult, IHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>>), historyId);
            return context;
        }
    }
    
    public class TransactionalHistoryContext<TParam, TResult> : ITransactionalHistoryContext<TParam, TResult, ITransactionalHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>>
    {
        internal TransactionalHistoryContext(ITransactionalHistory<TParam, TResult> history, IScheduler outputScheduler)
        {
            History = history;
        
            var canUndo = CanSnapshot
                .CombineLatest(history.CanUndo, (recordable, executable) => recordable && executable);
        
            Undo = ReactiveCommand.Create(history.Undo,  canUndo, outputScheduler);
        
            var canRedo = CanSnapshot
                .CombineLatest(history.CanRedo, (recordable, executable) => recordable && executable);
        
            Redo = ReactiveCommand.Create(history.Redo, canRedo, outputScheduler);
            
            Clear = ReactiveCommand.Create(history.Clear, history.CanClear, outputScheduler);
        }
        
        public ITransactionalHistory<TParam, TResult> History { get; }
        public ReactiveCommand<Unit, Unit> Undo { get; }
        public ReactiveCommand<Unit, Unit> Redo { get; }
        public ReactiveCommand<Unit, Unit> Clear { get; }
        public IObservable<bool> CanSnapshot => History.CanSnapshot;
        public void Snapshot(IHistoryEntry<TParam, TResult> entry) => History.Snapshot(entry);
        public void BeginTransaction(ITransaction<TParam, TResult> transaction) => History.BeginTransaction(transaction);
        public void CommitTransaction() => History.CommitTransaction();
        public void RollbackTransaction() => History.RollbackTransaction();
        
        public void Dispose()
        {
            Undo.Dispose();
            Redo.Dispose();
            Clear.Dispose();
        }

    }
}