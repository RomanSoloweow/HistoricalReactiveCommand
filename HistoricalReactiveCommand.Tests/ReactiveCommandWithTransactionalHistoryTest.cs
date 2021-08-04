using System;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using HistoricalReactiveCommand.Imports;
using Xunit;

namespace HistoricalReactiveCommand.Tests
{
    public class ReactiveCommandWithTransactionalHistoryTest
    {
        private readonly Subject<bool> _canExecuteSubject = new();
        private readonly IScheduler _scheduler = Scheduler.Immediate;
        
        [Fact]
        public void TransactionCommitSuccess()
        {
            int myNumber = 0;
            var command = ReactiveCommandWithTransactionalHistory.CreateWithHistory<int>(
                (number) => { myNumber += number; },
                (number) => { myNumber -= number; },
                Observables.True, _scheduler);

            command.History.BeginTransaction(new Transition());
            command.Execute(25).Subscribe();
            command.Execute(25).Subscribe();
            command.Execute(25).Subscribe();
            command.Execute(25).Subscribe();
            command.History.CommitTransaction();
            Assert.Equal(100, myNumber);
            
        }
        
        [Fact]
        public void TransactionRollbackSuccess()
        {
            int myNumber = 0;
            var command = ReactiveCommandWithTransactionalHistory.CreateWithHistory<int>(
                (number) => { myNumber += number; },
                (number) => { myNumber -= number; },
                Observables.True, _scheduler);

            command.History.BeginTransaction(new Transition());
            command.Execute(25).Subscribe();
            command.Execute(25).Subscribe();
            command.Execute(25).Subscribe();
            command.Execute(25).Subscribe();
            command.History.RollbackTransaction();
            Assert.Equal(0, myNumber);
        }
        
        [Fact]
        public void TransactionUndoRedoSuccess()
        {
            int myNumber = 0;
            var command = ReactiveCommandWithTransactionalHistory.CreateWithHistory<int>(
                (number) => { myNumber += number; },
                (number) => { myNumber -= number; },
                Observables.True, _scheduler);

            command.History.BeginTransaction(new Transition());
            command.Execute(25).Subscribe();
            command.Execute(25).Subscribe();
            command.Execute(25).Subscribe();
            command.Execute(25).Subscribe();
            command.History.CommitTransaction();
            Assert.Equal(100, myNumber);
            command.History.Undo.Execute().Subscribe();
            Assert.Equal(0, myNumber);
            command.History.Redo.Execute().Subscribe();
            Assert.Equal(100, myNumber);
        }
        
        
    }
}