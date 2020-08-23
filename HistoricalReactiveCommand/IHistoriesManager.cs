using System;
using System.Collections.Generic;
using System.Text;

namespace HistoricalReactiveCommand
{
    public interface IHistoriesManager
    {
        void RegisterHistory(string historyKey, IHistory history);
        IHistory ResolveHistory(string historyKey);
    }
}
