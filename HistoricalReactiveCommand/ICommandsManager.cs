namespace HistoricalReactiveCommand
{
    public interface ICommandsManager
    {
        void RegisterCommand(string commandKey, ICommandExecutor command);
        ICommandExecutor ResolveCommand(string commandKey);
    }
}
