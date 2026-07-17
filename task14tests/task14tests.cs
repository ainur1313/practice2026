using System;
using Xunit;
using task14;

namespace task14tests
{
    public class IntegralTests
    {
        [Fact]
        public void Test_LinearFunction_MultipleThreads()
        {
            double expected = 0.5;
            double result = DefiniteIntegral.Solve(0, 1, x => x, 1e-5, 8);
            Assert.Equal(expected, result, 1e-4);
        }

        [Fact]
        public void Test_SineFunction_SingleThread()
        {
            double expected = 2.0;
            double result = DefiniteIntegral.Solve(0, Math.PI, x => Math.Sin(x), 1e-5, 1);
            Assert.Equal(expected, result, 1e-4);
        }

        [Fact]
        public void Test_NegativeBounds()
        {
            double expected = 2.0 / 3.0;
            double result = DefiniteIntegral.Solve(-1, 1, x => x * x, 1e-5, 4);
            Assert.Equal(expected, result, 1e-4);
        }
    }
}
