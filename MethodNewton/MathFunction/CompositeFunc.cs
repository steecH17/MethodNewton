using System;
using MathNet.Numerics.LinearAlgebra;

namespace MethodNewton
{
    public class CompositeFunc : IFunction
    {
        private List<IFunction> _functions;
        public CompositeFunc()
        {
            _functions = new List<IFunction>();
        }
        public CompositeFunc(List<IFunction> functions)
        {
            if (functions == null)
                _functions = new List<IFunction>();
            else
                _functions = functions;
        }

        public void AddFunction(IFunction function)
        {
            if (function != null)
                _functions.Add(function);
        }

        public double Calculate(double[] argument, double coefficient)
        {
            double functionValue = 0;
            
            for(int i = 0; i < _functions.Count; i++)
            {
                functionValue += _functions[i].Calculate(argument, 1); // вынужденный хардкод коэф = 1
            }

            return functionValue;
        }

        public double Calculate(double[] argument, Vector<double> coefficient)
        {
            double functionValue = 0;
            
            for(int i = 0; i < _functions.Count; i++)
            {
                functionValue += _functions[i].Calculate(argument, coefficient[i]);
            }

            return functionValue;
        }
    }
}