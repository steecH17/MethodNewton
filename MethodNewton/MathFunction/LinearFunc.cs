using System;

namespace MethodNewton
{
    public class LinearFunc : AFunction
    {
        public LinearFunc(int argumentIndex = 0): base(argumentIndex){}
        protected override double CalculateFunc(double argument, double coefficient)
        {
            return coefficient * argument;
        }

        public override string ToString() => $"x{_argIndex + 1}";
    }
}