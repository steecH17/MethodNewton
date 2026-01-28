using System;

namespace MethodNewton
{
    public class CtgFunc : AFunction
    {
        public CtgFunc(int argumentIndex = 0): base(argumentIndex){}
        protected override double CalculateFunc(double argument, double coefficient)
        {
            return coefficient * (Math.Cos(argument) / Math.Sin(argument));
        }

        public override string ToString() => $"ctg(x{_argIndex + 1})";
    }
}