using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;

namespace MethodNewton
{
    public class OptimalFunctionFinder
    {
        List<IFunction> functionsComponent;//компоненты для составления комбинаций
        // double _determinationCoeff;
        // Vector<double> _coeffFunction;
        // List<IFunction> _optimalFunction;
        // NewtonMethodSolver _solversForOptimalFunc;
        // int _optimalIndex;
        List<double> xData;
        List<double> yData;
        double _tolerance;
        List<FunctionModel> functionModels = new List<FunctionModel>();

        public OptimalFunctionFinder(List<IFunction> components)
        {
            functionsComponent = components;
        }

        // public List<IFunction> OptimalFunction => _optimalFunction;
        // public double DeterminationCoeff => _determinationCoeff;
        // public Vector<double> CoeffFunctions => _coeffFunction;
        // public NewtonMethodSolver NewtonSolver => _solversForOptimalFunc;

        public List<FunctionModel> FindOptimalFunction(List<double> xData, List<double> yData, double tolerance)
        {
            this.xData = xData;
            this.yData = yData;
            _tolerance = tolerance;

            List<List<IFunction>> combinations = GenerateCombinations();
            // List<double> determinationCoeff = new List<double>();
            // List<Vector<double>> coeffFunction = new List<Vector<double>>();
            // List<NewtonMethodSolver> solvers = new List<NewtonMethodSolver>();

            for(int i = 0; i < combinations.Count; i++)
            {
                NewtonMethodSolver solver = new NewtonMethodSolver(combinations[i]);

                Vector<double> intiGuess = GenerateInitGuess(combinations[i].Count);
                Vector<double> coeffFunction = solver.FindCoeff(this.xData.ToArray(), this.yData.ToArray(), intiGuess, _tolerance, 1000);
                // coeffFunction.Add(solver.FindCoeff(this.xData.ToArray(), this.yData.ToArray(), intiGuess, _tolerance, 1000));

                // determinationCoeff.Add(solver.DeterminationCoeff(this.xData.ToArray(), this.yData.ToArray(), coeffFunction[i]));

                // solvers.Add(solver); 
                functionModels.Add(new FunctionModel(
                    combinations[i],
                    coeffFunction,
                    solver.IterConvergence,
                    solver.DeterminationCoeff(this.xData.ToArray(), this.yData.ToArray(), coeffFunction))
                    );
            }

            return functionModels;

            // _optimalIndex = determinationCoeff.IndexOf(determinationCoeff.Max());

            // _optimalFunction = combinations[_optimalIndex];
            // _determinationCoeff = determinationCoeff[_optimalIndex];
            // _coeffFunction = coeffFunction[_optimalIndex];
            // _solversForOptimalFunc = solvers[_optimalIndex];

        }

        private Vector<double> GenerateInitGuess(int length)
        {
            Vector<double> intiGuess = Vector<double>.Build.Dense(length);

            for(int i = 0; i < length; i++)
            {
                intiGuess.Add(1.0);
            }

            return intiGuess;
        }

        // Универсальный метод для генерации всех комбинаций
        private List<List<IFunction>> GenerateCombinations()
        {
            var result = new List<List<IFunction>>();
            int count = functionsComponent.Count;

            // Генерируем все комбинации кроме пустой
            for (int i = 1; i < (1 << count); i++)
            {
                var combination = new List<IFunction>();

                for (int j = 0; j < count; j++)
                {
                    if ((i & (1 << j)) != 0)
                    {
                        combination.Add(functionsComponent[j]);
                    }
                }

                result.Add(combination);
            }

            return result;
        }
    }
}