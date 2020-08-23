namespace HistoricalReactiveCommand
{
    public interface IHistoryCommandRegistry
    {
        void RegisterCommand(string commandKey, IHistoryCommand command);
        IHistoryCommand ResolveCommand(string commandKey);
    }
}
