using System;
using MathNet.Numerics.LinearAlgebra;
using System.Diagnostics;

namespace MethodNewton
{
    public class FunctionModel
    {
        List<IFunction> _function;
        Vector<double> _coeffFunction;
        int _iterConvergence;
        double _determinationCoeff;
        Stopwatch _solveTime;

        public FunctionModel(List<IFunction> function, Vector<double> coeffFunc, int iterConvergence, double coeffDeterm, Stopwatch solveTime)
        {
            _function = function;
            _coeffFunction = coeffFunc;
            _iterConvergence = iterConvergence;
            _determinationCoeff = coeffDeterm;
            _solveTime = solveTime;
        }

        public List<IFunction> Function => _function;
        public int CountComponents => _function.Count;
        public Vector<double> CoeffFunction => _coeffFunction;
        public int IterConvergence => _iterConvergence;
        public double DeterminationCoeff => _determinationCoeff;
        public Stopwatch SolveTime => _solveTime;

    }
}