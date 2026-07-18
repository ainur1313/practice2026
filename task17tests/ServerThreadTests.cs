using System.Threading;
using task17;
using Xunit;

namespace task17tests
{
    public class ServerThreadTests
    {
        [Fact]
        public void SoftStop_Should_ProcessAllCommands_BeforeStopping()
        {
            var server = new ServerThread();
            int counter = 0;
            var done = new ManualResetEventSlim(false);

            server.AddCommand(new TestCommand(() => Interlocked.Increment(ref counter)));
            server.AddCommand(new TestCommand(() => Interlocked.Increment(ref counter)));
            server.AddCommand(new TestCommand(() => done.Set()));
            server.AddCommand(new SoftStopCommand(server));

            server.Start();
            done.Wait(5000);

            Assert.Equal(2, counter);
        }

        [Fact]
        public void HardStop_Should_InterruptProcessing_Immediately()
        {
            var server = new ServerThread();
            int counter = 0;

            server.AddCommand(new TestCommand(() => Interlocked.Increment(ref counter)));
            server.AddCommand(new HardStopCommand(server));
            server.AddCommand(new TestCommand(() => Interlocked.Increment(ref counter)));

            server.Start();
            Thread.Sleep(1000);

            Assert.Equal(1, counter);
        }

        [Fact]
        public void HardStop_Should_Throw_WhenCalledFromWrongThread()
        {
            var server = new ServerThread();
            server.Start();

            Assert.Throws<InvalidOperationException>(() => server.HardStop());
        }

        [Fact]
        public void SoftStop_Should_Throw_WhenCalledFromWrongThread()
        {
            var server = new ServerThread();
            server.Start();

            Assert.Throws<InvalidOperationException>(() => server.SoftStop());
        }

        [Fact]
        public void RepeatableCommand_Should_BeRescheduled()
        {
            var server = new ServerThread();
            var repeatable = new RepeatableCommand(3);
            int counter = 0;
            var done = new ManualResetEventSlim(false);

            server.AddCommand(repeatable);
            server.AddCommand(new TestCommand(() => Interlocked.Increment(ref counter)));
            server.AddCommand(new TestCommand(() => done.Set()));
            server.AddCommand(new SoftStopCommand(server));

            server.Start();
            done.Wait(5000);

            Assert.Equal(1, counter);
            Assert.True(repeatable.CallCount >= 3);
        }

        
    }

    internal class TestCommand : ICommand
    {
        private readonly System.Action _action;

        public TestCommand(System.Action action)
        {
            _action = action;
        }

        public void Execute()
        {
            _action();
        }
    }

    internal class RepeatableCommand : ICommand, IRepeatable
    {
        private readonly int _totalCalls;
        public int CallCount { get; private set; }

        public RepeatableCommand(int totalCalls)
        {
            _totalCalls = totalCalls;
        }

        public void Execute()
        {
            CallCount++;
        }

        public bool IsFinished()
        {
            return CallCount >= _totalCalls;
        }
    }
}