using System;

namespace MethodNewton
{
    public class CtgFunc : IFunction
    {
        public double Calculate(double argument, double coefficient)
        {
            return coefficient * (Math.Cos(argument) / Math.Sin(argument));
        }

        public override string ToString() => $"ctg(x)";
    }
}