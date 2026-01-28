using System;

namespace MethodNewton
{
    public interface IFunction
    {
        double Calculate(double[] arguments, double coefficient);
    }
}