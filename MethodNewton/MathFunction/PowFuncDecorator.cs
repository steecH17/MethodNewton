using System;

namespace MethodNewton
{
    public class PowFuncDecorator : IFunction
    {
        private IFunction _decoratedFunc;
        private int _degree;
        public PowFuncDecorator(IFunction function, int degree)
        {
            _decoratedFunc = function;
            _degree = degree;
        }
        public int Degree => _degree;
        public double Calculate(double[] argument, double coefficient)
        {
            return Math.Pow(_decoratedFunc.Calculate(argument, coefficient), _degree);
        }

        public override string ToString() => $"({_decoratedFunc})^{_degree}";
    }
}