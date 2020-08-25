using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Splat;
using Xunit;

namespace HistoricalReactiveCommand.Tests
{
    public class HistoryContextTests
    {
        private readonly IScheduler _scheduler = Scheduler.Immediate;

        [Fact]
        public void ShouldUseDefaultHistoryFromLocator()
        {
            string historyKey = Guid.NewGuid().ToString();
            History.RegistryHistory(new DefaultHistory(), historyKey, _scheduler);
            var context = History.GetContext(historyKey);
            
            Assert.NotNull(context);
        }

        [Fact]
        public void ShouldResolveDifferentContextsForDifferentHistories()
        {
            string historyKey = Guid.NewGuid().ToString();
            History.RegistryHistory(new DefaultHistory(), historyKey,  _scheduler);
            var context = History.GetContext(historyKey);

            historyKey = Guid.NewGuid().ToString();
            History.RegistryHistory(new DefaultHistory(), historyKey, _scheduler);
            var nextContext = History.GetContext(historyKey);

            Assert.NotNull(context);
            Assert.NotNull(nextContext);
            Assert.NotEqual(context, nextContext);
        }

        [Fact]
        public void CanUndoShouldRespectHistoryState()
        {
            string historyKey = Guid.NewGuid().ToString();
            var canUndo = false;
            var canRedo = false;
            var history = new DefaultHistory();

            History.RegistryHistory(history, historyKey, _scheduler);
            var context = History.GetContext(historyKey);
            context.Undo.CanExecute.Subscribe(can => canUndo = can);
            context.Redo.CanExecute.Subscribe(can => canRedo = can);
            
            Assert.False(canUndo);
            Assert.False(canRedo);
            
            history.Record(
                    new HistoryEntry(Unit.Default, 42, "awesome"), 
                    entry => Observable.Return(new HistoryEntry(entry.Parameter, entry.Result, entry.CommandKey)))
                .Subscribe();
            
            Assert.True(canUndo);
            Assert.False(canRedo);
        }
    }
}