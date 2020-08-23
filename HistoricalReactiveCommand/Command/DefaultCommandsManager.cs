using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HistoricalReactiveCommand.Command
{
    public sealed class DefaultCommandsManager : ICommandsManager
    {
        private readonly IDictionary<string, ICommandExecutor> _commands = new ConcurrentDictionary<string, ICommandExecutor>();
        
        public void RegisterCommand(string commandKey, ICommandExecutor command)
        {
            if (_commands.ContainsKey(commandKey))
                throw new ArgumentException($"The key {commandKey} was already registered.", nameof(commandKey));
            _commands[commandKey] = command;
        }

        public ICommandExecutor ResolveCommand(string commandKey)
        {
            if (!_commands.ContainsKey(commandKey))
                throw new ArgumentException($"The key {commandKey} wasn't registered.", nameof(commandKey));
            return _commands[commandKey];
        }
    }
}