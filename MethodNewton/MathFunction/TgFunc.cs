using System;

namespace MethodNewton
{
    public class TgFunc : IFunction
    {
        public double Calculate(double argument, double coefficient)
        {
            return coefficient * Math.Tan(argument);
        }

        public override string ToString() => $"tg(x)";
    }
}