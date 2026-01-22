using System;
using MathNet.Numerics.LinearAlgebra;

namespace MethodNewton
{
    public class FunctionModel
    {
        List<IFunction> _function;
        Vector<double> _coeffFunction;
        int _iterConvergence;
        double _determinationCoeff;

        public FunctionModel(List<IFunction> function, Vector<double> coeffFunc, int iterConvergence, double coeffDeterm)
        {
            _function = function;
            _coeffFunction = coeffFunc;
            _iterConvergence = iterConvergence;
            _determinationCoeff = coeffDeterm;
        }

        public List<IFunction> Function => _function;
        public int CountComponents => _function.Count;
        public Vector<double> CoeffFunction => _coeffFunction;
        public int IterConvergence => _iterConvergence;
        public double DeterminationCoeff => _determinationCoeff;

    }
}