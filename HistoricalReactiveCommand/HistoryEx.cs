using System.Reactive.Concurrency;
using ReactiveUI;

namespace HistoricalReactiveCommand
{
    public static class HistoryEx
    {
        public static TransactionalHistoryContext RegisterTransactionalHistory(ITransactionalHistory history, 
            IScheduler? outputScheduler = null)
        {
            return TransactionalHistoryContext.GetContext(history, outputScheduler ?? RxApp.MainThreadScheduler);
        }
        
        public static TransactionalHistoryContext RegisterTransactionalHistory(string historyId = "", 
            IScheduler? outputScheduler = null)
        {
            return TransactionalHistoryContext.GetContext(historyId, outputScheduler ?? RxApp.MainThreadScheduler);
        }
        
        public static HistoryContext RegisterHistory(ITransactionalHistory history, 
            IScheduler? outputScheduler = null)
        {
            return HistoryContext.GetContext(history, outputScheduler ?? RxApp.MainThreadScheduler);
        }
        
        public static HistoryContext RegisterHistory(string historyId = "", 
            IScheduler? outputScheduler = null)
        {
            return HistoryContext.GetContext(historyId, outputScheduler ?? RxApp.MainThreadScheduler);
        }
    }
}