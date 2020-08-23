using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Windows.Input;
using HistoricalReactiveCommand.History;
using HistoricalReactiveCommand.Imports;
using Xunit;

namespace HistoricalReactiveCommand.Tests
{
    public class ReactiveCommandWithHistoryTest
    {
        private readonly IHistoryCommandRegistry _registry = new DefaultHistoryCommandRegistry();
        private readonly IHistory _history = new InMemoryHistory();
        
        [Fact]
        public void CanExecuteChangedIsAvailableViaICommand()
        {
            Subject<bool> canExecuteSubject = new Subject<bool>();
            ICommand fixture = new ReactiveCommandWithHistory<Unit, Unit>(
                (parameter, result) => Observables.Unit,
                (parameter, result) => Observables.Unit,
                _registry,
                _history,
                "CommandKey",
                canExecuteSubject,
                Scheduler.Immediate);
            
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
