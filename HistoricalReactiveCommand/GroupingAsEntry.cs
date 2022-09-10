using System;
using System.Collections.Generic;
using System.Linq;

namespace HistoricalReactiveCommand
{
    public class GroupingAsEntry<TParam, TResult> : IGrouping<TParam, TResult>
    {
        private readonly Func<List<IHistoryEntryForGroup<TParam, TResult>>, IHistoryEntryForGroup<TParam, TResult>> _groupingAction;
        private readonly List<IHistoryEntryForGroup<TParam, TResult>> _groups = new();
        
        public GroupingAsEntry(Func<List<IHistoryEntryForGroup<TParam, TResult>>, IHistoryEntryForGroup<TParam, TResult>> groupingAction)
        {
            _groupingAction = groupingAction;
        }
        
        public void Append(IHistoryEntryForGroup<TParam, TResult> entry)
        {
            _groups.Add(entry);
        }

        public IHistoryEntryForGroup<TParam, TResult> Group()
        {
           return _groupingAction(_groups);
        }

        public void Rollback()
        {
            foreach (var group in _groups)
            {
                group.Undo();
            }
        }

        public bool IsEmpty => !_groups.Any();
    }
}