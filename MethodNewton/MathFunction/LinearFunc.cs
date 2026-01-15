using System;

namespace MethodNewton
{
    public class LinearFunc : IFunction
    {
        public double Calculate(double argument, double coefficient)
        {
            return coefficient * argument;
        }

        public override string ToString() => $"x";
    }
}