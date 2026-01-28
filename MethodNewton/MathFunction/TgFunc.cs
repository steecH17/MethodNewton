using System;

namespace MethodNewton
{
    public class TgFunc : AFunction
    {
        public TgFunc(int argumentIndex = 0): base(argumentIndex){}
        protected override double CalculateFunc(double argument, double coefficient)
        {
            return coefficient * Math.Tan(argument);
        }

        public override string ToString() => $"tg(x{_argIndex + 1})";
    }
}