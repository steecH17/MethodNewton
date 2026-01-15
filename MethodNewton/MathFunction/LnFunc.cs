using System;

namespace MethodNewton
{
    public class LnFunc : IFunction
    {
        public double Calculate(double argument, double coefficient)
        {
            if(argument <= 0)
                return 0; //доопределяем нулем
            return coefficient * Math.Log(argument);
        }

        public override string ToString() => $"ln(x)";
    }
}