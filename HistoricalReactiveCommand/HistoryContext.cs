using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;

namespace HistoricalReactiveCommand
{
    public static class HistoryContext
    {
        internal static IHistoryContext<TParam, TResult, IHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>> GetContext<TParam, TResult>(IHistory<TParam, TResult> history, IScheduler? outputScheduler = null)
        {
            if (history == null)
            {
                throw new ArgumentNullException(nameof(history));
            }
        
            if (history.Id == null)
            {
                throw new ArgumentNullException(nameof(history.Id));
            }
        
            var context = Locator.Current.GetService<IHistoryContext<TParam, TResult, IHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>>>(history.Id);
        
            if (context != null)
            {
                return context;
            }
        
            context = new HistoryContext<TParam, TResult>(history, outputScheduler ?? RxApp.MainThreadScheduler);
            Locator.CurrentMutable.RegisterConstant(context, history.Id);
            return context;
        }

        internal static IHistoryContext<TParam, TResult, IHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>> GetContext<TParam, TResult>(string historyId = "", IScheduler? outputScheduler = null)
        {
            if (historyId == null)
            {
                throw new ArgumentNullException(nameof(historyId));
            }
        
            var context = Locator.Current.GetService<IHistoryContext<TParam, TResult, IHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>>>(historyId);
        
            if (context != null)
            {
                return context;
            }
        
            context = new HistoryContext<TParam, TResult>(new History<TParam, TResult>(historyId), outputScheduler ?? RxApp.MainThreadScheduler);
            Locator.CurrentMutable.RegisterConstant(context, historyId);
            return context;
        }
    }
    
    public sealed class HistoryContext<TParam, TResult>: IHistoryContext<TParam, TResult, IHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>>, IDisposable
    {
        private IHistory<TParam, TResult> _history;
        
        internal HistoryContext(IHistory<TParam, TResult>  history, IScheduler outputScheduler)
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

        public IHistory<TParam, TResult> History { get; }
        
        public ReactiveCommand<Unit, Unit> Undo { get;  }
        
        public ReactiveCommand<Unit, Unit> Redo { get;  }
        
        public ReactiveCommand<Unit, Unit> Clear { get;  }
        
        public void Dispose()
        {
            Undo.Dispose();
            Redo.Dispose();
            Clear.Dispose();
        }

        public void Snapshot(IHistoryEntry<TParam, TResult> entry) => History.Snapshot(entry);

        IHistory<TParam, TResult> IHistoryContext<TParam, TResult, IHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>>.History => _history;

        public IObservable<bool> CanSnapshot => History.CanSnapshot;

    }
}
