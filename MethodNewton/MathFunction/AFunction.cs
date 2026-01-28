using System;

namespace MethodNewton
{
    public abstract class AFunction : IFunction
    {
        protected int _argIndex;
        public AFunction(int argumentIndex = 0)
        {
            _argIndex = argumentIndex;
        }
        public virtual double Calculate(double[] arguments, double coefficient)
        {
            if(_argIndex >= arguments.Length)
                throw new ArgumentOutOfRangeException();

            return CalculateFunc(arguments[_argIndex], coefficient);
        }

        protected abstract double CalculateFunc(double arguments, double coefficient);
    }
}