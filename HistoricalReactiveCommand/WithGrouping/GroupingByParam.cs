using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace HistoricalReactiveCommand.WithGrouping
{
    public class GroupingByParam<TParam, TResult> : IGrouping<TParam, TResult>
    {
        private readonly Subject<bool> _isEmpty = new Subject<bool>();

        private readonly Func<IEnumerable<TParam>, (TParam, TResult)> _groupingAction;
        private readonly List<IHistoryEntry<TParam, TResult>> _groups = new List<IHistoryEntry<TParam, TResult>>();

        public IObservable<bool> IsEmpty => _isEmpty.AsObservable().DistinctUntilChanged();

        public GroupingByParam(Func<IEnumerable<TParam>, (TParam, TResult)> groupingAction)
        {
            _groupingAction = groupingAction;
        }

        public void Append(IHistoryEntry<TParam, TResult> entry)
        {
            _groups.Add(entry);
        }

        public IHistoryEntry<TParam, TResult> Group(string commandName)
        {
            var parameters = _groups.Select(x => x.Parameter).ToList();
            var (param, result) = _groupingAction(parameters);

            return new HistoryEntry<TParam, TResult>(commandName, param, result);
        }

        // public void Rollback()
        // {
        //     var parameters = _groups.Select(x => x.Parameter).ToList();
        //     var (param, result) = _groupingAction(parameters);
        //     _discard(param, result);
        // }
    }
}