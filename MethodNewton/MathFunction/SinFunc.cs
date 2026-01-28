using System;

namespace MethodNewton
{
    public class SinFunc : AFunction
    {
        public SinFunc(int argumentIndex = 0): base(argumentIndex){}
        protected override double CalculateFunc(double argument, double coefficient)
        {
            return coefficient * Math.Sin(argument);
        }

        public override string ToString() => $"sin(x{_argIndex + 1})";
    }
}