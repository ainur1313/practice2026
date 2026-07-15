using System;
using Xunit;
using task11;

namespace task11tests
{
    public class CalculatorTests
    {
        [Fact]
        public void CreateCalculator_DivisionByZero_ShouldThrowException()
        {
            string code = @"
        public class Calculator
        {
            public int Add(int a, int b) => a + b;
            public int Minus(int a, int b) => a - b;
            public int Mul(int a, int b) => a * b;
            public int Div(int a, int b) => a / b;
        }";

            ICalculator calculator = ClassGenerator.CreateCalculator(code);

            Assert.Throws<DivideByZeroException>(() => calculator.Div(10, 0));
        }

        [Fact]
        public void CreateCalculator_CodeWithProperties_ShouldCompile()
        {
            string code = @"
        public class Calculator
        {
            private int _lastResult;
            
            public int LastResult => _lastResult;
            
            public int Add(int a, int b) 
            { 
                _lastResult = a + b; 
                return _lastResult; 
            }
            
            public int Minus(int a, int b) 
            { 
                _lastResult = a - b; 
                return _lastResult; 
            }
            
            public int Mul(int a, int b) 
            { 
                _lastResult = a * b; 
                return _lastResult; 
            }
            
            public int Div(int a, int b) 
            { 
                _lastResult = a / b; 
                return _lastResult; 
            }
        }";

            ICalculator calculator = ClassGenerator.CreateCalculator(code);

            Assert.Equal(15, calculator.Add(7, 8));
            Assert.Equal(20, calculator.Mul(4, 5));
        }

        [Fact]
        public void CreateCalculator_CodeWithMultipleConstructors_ShouldCompile()
        {
            string code = @"
        public class Calculator
        {
            private int _multiplier;
            
            public Calculator()
            {
                _multiplier = 1;
            }
            
            public Calculator(int multiplier)
            {
                _multiplier = multiplier;
            }
            
            public int Add(int a, int b) => (a + b) * _multiplier;
            public int Minus(int a, int b) => (a - b) * _multiplier;
            public int Mul(int a, int b) => (a * b) * _multiplier;
            public int Div(int a, int b) => (a / b) * _multiplier;
        }";

            ICalculator calculator = ClassGenerator.CreateCalculator(code);
            Assert.Equal(10, calculator.Add(4, 6));
            Assert.Equal(20, calculator.Mul(4, 5));
        }
    }
}
