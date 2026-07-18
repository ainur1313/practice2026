using System.Collections.Generic;
using task17;

namespace task17
{
    public class RoundRobinScheduler : IScheduler
    {
        private readonly Queue<ICommand> _commands = new Queue<ICommand>();
        private readonly object _lock = new object();

        public bool HasCommand()
        {
            lock (_lock)
            {
                return _commands.Count > 0;
            }
        }

        public ICommand Select()
        {
            lock (_lock)
            {
                var command = _commands.Dequeue();
                return command;
            }
        }

        public void Add(ICommand cmd)
        {
            lock (_lock)
            {
                _commands.Enqueue(cmd);
            }
        }
    }
}