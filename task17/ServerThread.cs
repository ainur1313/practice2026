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
        private volatile bool _hardStopRequested;

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
            foreach (var command in _queue.GetConsumingEnumerable())
            {
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

                if (_hardStopRequested)
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

            _queue.CompleteAdding();
        }
    }
}
