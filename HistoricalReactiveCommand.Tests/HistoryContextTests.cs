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
        public void ShouldResolveSameContextForSameHistory()
        {
            var history = new DefaultHistory();
            var context = History.GetContext(history, _scheduler);
            var nextContext = History.GetContext(history, _scheduler);
            
            Assert.NotNull(context);
            Assert.NotNull(nextContext);
            Assert.Equal(context, nextContext);
        }

        [Fact]
        public void ShouldResolveDifferentContextsForDifferentHistories()
        {
            var context = History.GetContext(new DefaultHistory(), _scheduler);
            var nextContext = History.GetContext(new DefaultHistory(), _scheduler);
            
            Assert.NotNull(context);
            Assert.NotNull(nextContext);
            Assert.NotEqual(context, nextContext);
        }

        [Fact]
        public void ShouldUseDefaultHistoryFromLocator()
        {
            Locator.CurrentMutable.RegisterConstant(new DefaultHistory(), typeof(IHistory));
            var context = History.GetContext();
            Assert.NotNull(context);
        }

        [Fact]
        public void CanUndoShouldRespectHistoryState()
        {
            var canUndo = false;
            var canRedo = false;
            var history = new DefaultHistory();
            
            var context = History.GetContext(history, _scheduler);
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