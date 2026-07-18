using System;
using System.Collections.Concurrent;
using System.Threading;
using task17;

namespace task17
{
    public class ServerThread
    {
        private readonly Thread _thread;
        private readonly BlockingCollection<ICommand> _queue = new BlockingCollection<ICommand>();
        private readonly IScheduler _scheduler = new RoundRobinScheduler();
        private volatile bool _hardStopRequested;
        private volatile bool _softStopRequested;

        public ServerThread()
        {
            _thread = new Thread(Run)
            {
                IsBackground = true
            };
        }

        public void Start()
        {
            _thread.Start();
        }

        public void AddCommand(ICommand command)
        {
            _queue.Add(command);
        }

        private void Run()
        {
            while (true)
            {
                if (_hardStopRequested)
                    break;

                ICommand command = null;

                if (_scheduler.HasCommand())
                {
                    command = _scheduler.Select();
                }
                else if (_queue.TryTake(out command, 100))
                {
                }
                else
                {
                    continue;
                }

                if (command == null)
                    continue;

                try
                {
                    command.Execute();
                }
                catch (ThreadInterruptedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(command, ex);
                }

                if (command is IRepeatable repeatable && !repeatable.IsFinished())
                {
                    _scheduler.Add(command);
                }

                if (_softStopRequested && !_scheduler.HasCommand() && _queue.Count == 0)
                    break;
            }
        }

        public void HardStop()
        {
            if (Thread.CurrentThread != _thread)
                throw new InvalidOperationException(
                    "HardStop must be called from the server thread");

            _hardStopRequested = true;
            _thread.Interrupt();
        }

        public void SoftStop()
        {
            if (Thread.CurrentThread != _thread)
                throw new InvalidOperationException(
                    "SoftStop must be called from the server thread");

            _softStopRequested = true;
            _queue.CompleteAdding();
        }
    }
}
