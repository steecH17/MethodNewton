using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;

namespace MethodNewton
{
    public class OptimalFunctionFinder
    {
        List<IFunction> _baseFunctionsComponent;//набор компонентов для составления комбинаций
        List<IFunction> _functionsComponent;//набор компонентов исходя из количества аргументов функции
        List<List<double>> xData;
        List<double> yData;
        double _tolerance;
        int _countArguments;
        List<FunctionModel> functionModels = new List<FunctionModel>();

        public OptimalFunctionFinder(List<IFunction> components)
        {
            _baseFunctionsComponent = components;
            _functionsComponent = new List<IFunction>();
            _countArguments = 1;
        }

        public List<FunctionModel> FindOptimalFunction(List<List<double>> xData, List<double> yData, double tolerance)
        {
            this.xData = xData;
            this.yData = yData;
            _tolerance = tolerance;
            _countArguments = xData[0].Count;
            InitFunctionComponent();

            double[][] xArrayData = this.xData.Select(list => list.ToArray()).ToArray();
            List<List<IFunction>> combinations = GenerateCombinations();

            for (int i = 0; i < combinations.Count; i++)
            {
                NewtonMethodSolver solver = new NewtonMethodSolver(combinations[i]);

                Vector<double> intiGuess = GenerateInitGuess(combinations[i].Count);
                Vector<double> coeffFunction = solver.FindCoeff(xArrayData, this.yData.ToArray(), intiGuess, _tolerance, 1000);

                functionModels.Add(new FunctionModel(
                    combinations[i],
                    coeffFunction,
                    solver.IterConvergence,
                    solver.DeterminationCoeff(xArrayData, this.yData.ToArray(), coeffFunction))
                    );
            }

            return functionModels;
        }

        private Vector<double> GenerateInitGuess(int length)
        {
            Vector<double> intiGuess = Vector<double>.Build.Dense(length);

            for (int i = 0; i < length; i++)
            {
                intiGuess.Add(1.0);
            }

            return intiGuess;
        }

        private void InitFunctionComponent()
        {
            for (int i = 0; i < _countArguments; i++)
            {
                foreach (var func in _baseFunctionsComponent)
                {
                    if (func is CosFunc)
                        _functionsComponent.Add(new CosFunc(i));
                    if (func is CtgFunc)
                        _functionsComponent.Add(new CtgFunc(i));
                    if (func is ExpFunc)
                        _functionsComponent.Add(new ExpFunc(i));
                    if (func is LinearFunc)
                        _functionsComponent.Add(new LinearFunc(i));
                    if (func is LnFunc)
                        _functionsComponent.Add(new LnFunc(i));
                    if (func is SinFunc)
                        _functionsComponent.Add(new SinFunc(i));
                    if (func is TgFunc)
                        _functionsComponent.Add(new TgFunc(i));
                    if (func is PowFuncDecorator funcDecorator)
                    {
                        if (funcDecorator.Degree == 2)
                            _functionsComponent.Add(new PowFuncDecorator(new LinearFunc(i), 2));
                        if (funcDecorator.Degree == 3)
                            _functionsComponent.Add(new PowFuncDecorator(new LinearFunc(i), 3));
                    }

                }
            }

            foreach (var func in _baseFunctionsComponent)
            {
                if(func is Constant)
                    _functionsComponent.Add(new Constant());
            }
        }

        // Универсальный метод для генерации всех комбинаций
        private List<List<IFunction>> GenerateCombinations()
        {
            var result = new List<List<IFunction>>();
            int count = _functionsComponent.Count;

            // Генерируем все комбинации кроме пустой
            for (int i = 1; i < (1 << count); i++)
            {
                var combination = new List<IFunction>();

                for (int j = 0; j < count; j++)
                {
                    if ((i & (1 << j)) != 0)
                    {
                        combination.Add(_functionsComponent[j]);
                    }
                }

                result.Add(combination);
            }

            return result;
        }
    }
}