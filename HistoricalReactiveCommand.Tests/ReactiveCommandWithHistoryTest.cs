using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Windows.Input;
using HistoricalReactiveCommand.Imports;
using Xunit;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace HistoricalReactiveCommand.Tests
{
    public class ReactiveCommandWithHistoryTest
    {
        private readonly Subject<bool> _canExecuteSubject = new();
        private readonly IScheduler _scheduler = Scheduler.Immediate;
        
        
        [Fact]
        public void CanExecuteChangedIsAvailableViaICommand()
        {
            ICommand fixture = ReactiveCommandWithHistory.CreateWithHistoryFromObservable<Unit, Unit>(
                (parameter, result) => Observables.Unit,
                (parameter, result) => Observables.Unit,
                _canExecuteSubject,
                _scheduler);

            List<bool> canExecuteChanged = new List<bool>();
            fixture.CanExecuteChanged += (s, e) => canExecuteChanged.Add(fixture.CanExecute(null));

            _canExecuteSubject.OnNext(true);
            _canExecuteSubject.OnNext(false);

            Assert.Equal(2, canExecuteChanged.Count);
            Assert.True(canExecuteChanged[0]);
            Assert.False(canExecuteChanged[1]);
        }

        [Fact]
        public void ShouldManageCommandHistory()
        {
            int myNumber = 0;
            var command = ReactiveCommandWithHistory.CreateWithHistory<int>(
             (number) => { myNumber += number; },
             (number) => { myNumber -= number; },
              Observables.True, _scheduler);

            command.Execute(25).Subscribe();
            Assert.Equal(25, myNumber);
            command.Execute(25).Subscribe();
            Assert.Equal(50, myNumber);
            command.History.Undo.Execute().Subscribe();
            Assert.Equal(25, myNumber);
            command.History.Undo.Execute().Subscribe();
            Assert.Equal(0, myNumber);
            command.History.Redo.Execute().Subscribe();
            Assert.Equal(25, myNumber);
            command.History.Redo.Execute().Subscribe();
            Assert.Equal(50, myNumber);
        }

        [Fact]
        public async Task CanExecuteChangeOnExecutingCommandAsync()
        {

            ICommand fixture = ReactiveCommandWithHistory.CreateWithHistoryFromObservable<Unit, Unit>(
              (parameter, result) => Observables.Unit,
              (parameter, result) => Observables.Unit,
              _canExecuteSubject,
              _scheduler);

            List<bool> canExecuteChanged = new List<bool>();
            fixture.CanExecuteChanged += (s, e) => canExecuteChanged.Add(fixture.CanExecute(null));

            var count = 0;
            await _canExecuteSubject.FirstAsync();
            _canExecuteSubject.OnNext(true);
            _canExecuteSubject.OnCompleted();
            count = canExecuteChanged.Count;

            //var myTask = _canExecuteSubject.FirstAsync().ToTask();
            //_canExecuteSubject.OnNext(true);
            //Assert.Equal(1, canExecuteChanged.Count);
            //var b = await myTask;
            //_canExecuteSubject.OnCompleted();
            //Assert.Equal(2, canExecuteChanged.Count);    

        }

        [Fact]
        public async Task CommandRespectCanExecute()
        {
           
            // ICommand fixture = ReactiveCommandWithHistory.CreateWithHistory<Unit>(
            //     (_) => {
            //         executed = true;
            //     },
            //     (_) => { },
            //     _canExecuteSubject,
            //     _scheduler);
            
            var executed = false;
            ICommand fixture = ReactiveCommand.Create<Unit>(
                (_) => { executed = true;}, _canExecuteSubject);
            
            _canExecuteSubject.OnNext(false);
            fixture.Execute(null);
            Assert.False(executed);
            
            
            
            var canExecuteChanged = new List<bool>();
            fixture.CanExecuteChanged += (s, e) => canExecuteChanged.Add(fixture.CanExecute(null));
            
    
            Assert.Single(canExecuteChanged);

        }
    }
}
