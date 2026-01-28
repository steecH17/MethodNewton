using System;

namespace MethodNewton
{
    public class Constant : IFunction
    {
        public double Calculate(double[] argument, double coefficient)
        {
            return coefficient;
        }
        public override string ToString() => $"";
    }
}