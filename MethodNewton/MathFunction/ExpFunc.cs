using System;

namespace MethodNewton
{
    public class ExpFunc : IFunction
    {
        public double Calculate(double argument, double coefficient)
        {
            return coefficient * Math.Exp(argument);
        }

        public override string ToString() => $"exp(x)";
    }
}