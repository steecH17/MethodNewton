using System;

namespace MethodNewton
{
    public class ExpFunc : AFunction
    {
        public ExpFunc(int argumentIndex = 0): base(argumentIndex){}
        protected override double CalculateFunc(double argument, double coefficient)
        {
            return coefficient * Math.Exp(argument);
        }

        public override string ToString() => $"exp(x{_argIndex + 1})";
    }
}