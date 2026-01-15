using System;

namespace MethodNewton
{
    public class CosFunc : IFunction
    {
        public double Calculate(double argument, double coefficient)
        {
            return coefficient * Math.Cos(argument);
        }

        public override string ToString() => $"cos(x)";
    }
}