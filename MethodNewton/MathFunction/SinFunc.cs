using System;

namespace MethodNewton
{
    public class SinFunc : IFunction
    {
        public double Calculate(double argument, double coefficient)
        {
            return coefficient * Math.Sin(argument);
        }

        public override string ToString() => $"sin(x)";
    }
}