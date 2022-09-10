using System.Reactive.Concurrency;
using System.Reflection;
using ReactiveUI;

namespace HistoricalReactiveCommand
{
    public static class HistoryEx
    {
        public static ITransactionalHistoryContext<TParam, TResult, ITransactionalHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>> 
            RegisterTransactionalHistory<TParam, TResult>(ITransactionalHistory<TParam, TResult> history, 
            IScheduler? outputScheduler = null)
        {
            return TransactionalHistoryContext.GetContext<TParam, TResult>(history, outputScheduler ?? RxApp.MainThreadScheduler);
        }
        
        public static ITransactionalHistoryContext<TParam, TResult, ITransactionalHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>> 
            RegisterTransactionalHistory<TParam, TResult>(string historyId = "", 
            IScheduler? outputScheduler = null)
        {
            return TransactionalHistoryContext.GetContext<TParam, TResult>(historyId, outputScheduler ?? RxApp.MainThreadScheduler);
        }
        
        public static IHistoryContext<TParam, TResult, IHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>> 
            RegisterHistory<TParam, TResult>(ITransactionalHistory<TParam, TResult> history, IScheduler? outputScheduler = null)
        {
            return HistoryContext.GetContext<TParam, TResult>(history, outputScheduler ?? RxApp.MainThreadScheduler);
        }
        
        public static IHistoryContext<TParam, TResult, IHistory<TParam, TResult>, IHistoryEntry<TParam, TResult>> RegisterHistory<TParam, TResult>(string historyId = "", 
            IScheduler? outputScheduler = null)
        {
            return HistoryContext.GetContext<TParam, TResult>(historyId, outputScheduler ?? RxApp.MainThreadScheduler);
        }
    }
}