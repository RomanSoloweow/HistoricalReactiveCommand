using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace HistoricalReactiveCommand.History
{
    public sealed class DefaultHistoryCommandRegistry : IHistoryCommandRegistry
    {
        private readonly IDictionary<string, IHistoryCommand> _commands = new ConcurrentDictionary<string, IHistoryCommand>();
        
        public void RegisterCommand(string commandKey, IHistoryCommand command)
        {
            if (_commands.ContainsKey(commandKey))
                throw new ArgumentException($"The key {commandKey} was already registered.", nameof(commandKey));
            _commands[commandKey] = command;
        }

        public IHistoryCommand ResolveCommand(string commandKey)
        {
            if (!_commands.ContainsKey(commandKey))
                throw new ArgumentException($"The key {commandKey} wasn't registered.", nameof(commandKey));
            return _commands[commandKey];
        }
    }
}