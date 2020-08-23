using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Windows.Input;
using HistoricalReactiveCommand.Command;
using HistoricalReactiveCommand.History;
using HistoricalReactiveCommand.Imports;
using Xunit;

namespace HistoricalReactiveCommand.Tests
{
    public class ReactiveCommandWithHistoryTest
    {
        private readonly IHistory _history = new ReactiveHistory();
        private readonly IScheduler sheduler = Scheduler.Immediate;
        private readonly ICommandsManager _commandsManager = new DefaultCommandsManager();
       
        [Fact]
        public void CanExecuteChangedIsAvailableViaICommand()
        {
            HistoryCommandsExecutor commandExecutor = new HistoryCommandsExecutor(_commandsManager, _history, sheduler);
            Subject<bool> canExecuteSubject = new Subject<bool>(); 
            Subject<bool> canDiscardSubject = new Subject<bool>();
            ICommand fixture = new ReactiveCommandWithHistory<Unit, Unit>(
                (parameter, result) => Observables.Unit,
                (parameter, result) => Observables.Unit,
                commandExecutor,
                "CommandKey",
                canExecuteSubject,
                canDiscardSubject,
                sheduler);
            
            List<bool> canExecuteChanged = new List<bool>();
            fixture.CanExecuteChanged += (s, e) => canExecuteChanged.Add(fixture.CanExecute(null));

            canExecuteSubject.OnNext(true);
            canExecuteSubject.OnNext(false);

            Assert.Equal(2, canExecuteChanged.Count);
            Assert.True(canExecuteChanged[0]);
            Assert.False(canExecuteChanged[1]);
        }
    }
}
