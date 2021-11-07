using System;
using System.Collections.Generic;
using System.Linq;

namespace HistoricalReactiveCommand
{
    public class GroupingByParam<TParam, TResult> : IGrouping<TParam, TResult>
    {
        private readonly Action<TParam> _execute;
        private readonly Action<TParam> _discard;
        private readonly Func<List<TParam>, TParam> _groupingAction;
        private readonly List<IHistoryEntryForGroup<TParam, TResult>> _groups = new();
        public bool IsEmpty => !_groups.Any();

        public GroupingByParam(
            Action<TParam> execute,
            Action<TParam> discard,
            Func<List<TParam>, TParam> groupingAction)
        {
            _execute = execute;
            _discard = discard;
            _groupingAction = groupingAction;
        }

        public void Append(IHistoryEntryForGroup<TParam, TResult> entry)
        {
            _groups.Add(entry);
        }

        public IHistoryEntry Group()
        {
            var parameters = _groups.Select(x => x.Param).ToList();
            var groupResult = _groupingAction(parameters);
            return new HistoryEntry(
                (entry) => { _discard(groupResult); },
                (entry) => { _execute(groupResult); });
        }

        public void Rollback()
        {
            var parameters = _groups.Select(x => x.Param).ToList();
            var groupResult = _groupingAction(parameters);
            _discard(groupResult);
        }
    }
}