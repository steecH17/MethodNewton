using System;

namespace MethodNewton
{
    public class LnFunc : AFunction
    {
        public LnFunc(int argumentIndex = 0): base(argumentIndex){}
        protected override double CalculateFunc(double argument, double coefficient)
        {
            if(argument <= 0)
                return 0; //доопределяем нулем
            return coefficient * Math.Log(argument);
        }

        public override string ToString() => $"ln(x{_argIndex + 1})";
    }
}