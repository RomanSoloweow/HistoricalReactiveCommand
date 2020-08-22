using System;
using System.Collections.Generic;
using System.Text;

namespace HistoricalReactiveCommand.History
{
    public interface IHistoryCommandRegistry
    {
        void RegisterCommand(string commandKey);
    }
}
