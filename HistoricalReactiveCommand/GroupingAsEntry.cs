using System;
using System.Collections.Generic;

namespace HistoricalReactiveCommand
{
    public class GroupingAsEntry<TParam, TResult>:IGrouping<TParam, TResult>
    {
        private Func<List<IHistoryEntryForGroup<TParam, TResult>>, IHistoryEntry> _groupingAction;
        public GroupingAsEntry(Func<List<IHistoryEntryForGroup<TParam, TResult>>, IHistoryEntry> groupingAction)
        {
            _groupingAction = groupingAction;
        }
     
        private List<IHistoryEntryForGroup<TParam, TResult>> Groups = new();
        public void Append(IHistoryEntryForGroup<TParam, TResult> entry)
        {
            Groups.Add(entry);
        }

        public IHistoryEntry Group()
        {
           return _groupingAction(Groups);
        }

        public void Rollback()
        {
            foreach (var group in Groups)
            {
                group.Undo();
            }
        }
    }
}