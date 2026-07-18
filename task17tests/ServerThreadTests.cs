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

            server.AddCommand(new TestCommand(() => Interlocked.Increment(ref counter)));
            server.AddCommand(new TestCommand(() => Interlocked.Increment(ref counter)));
            server.AddCommand(new SoftStopCommand(server));

            server.Start();
            Thread.Sleep(2000);

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
            Thread.Sleep(2000);

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
}
