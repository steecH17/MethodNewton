using System;

namespace MethodNewton
{
    public class CosFunc : AFunction
    {
        public CosFunc(int argumentIndex = 0): base(argumentIndex){}
        protected override double CalculateFunc(double argument, double coefficient)
        {
            return coefficient * Math.Cos(argument);
        }

        public override string ToString() => $"cos(x{_argIndex + 1})";
    }
}