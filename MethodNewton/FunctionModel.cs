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
        public override string ToString()
        {
            string resultFunc = "";
            string coefChar = "*";

            for (int i = 0; i < _function.Count; i++)
            {
                string sign = _coeffFunction[i] >= 0 ? "+" : "-";
                if (i != 0)
                    resultFunc += $"{sign}";

                if (_function[i] is Constant)
                    coefChar = "";
                else
                    coefChar = "*";

                resultFunc += $"{Math.Abs(Math.Round(_coeffFunction[i], 4))}{coefChar}{_function[i]}";
            }

            return resultFunc;
        }

        public string ToLatex()
        {
            string result = "f(x) = ";
            bool isFirstVisible = true; // Флаг для отслеживания первого отображаемого элемента

            for (int i = 0; i < _function.Count; i++)
            {
                double c = _coeffFunction[i];

                // Если коэффициент очень мал (фактически 0), пропускаем этот член
                if (Math.Abs(c) < 1e-6) continue;

                // Определяем знак
                string sign = "";
                if (isFirstVisible)
                {
                    if (c < 0) sign = "-"; // Если первое число отрицательное, ставим минус
                }
                else
                {
                    sign = c >= 0 ? " + " : " - ";
                }

                string val = Math.Abs(Math.Round(c, 4)).ToString();

                // Преобразование имени функции и индексов x
                string funcName = _function[i].ToString();

                // Заменяем x1, x2... на x_{1}, x_{2}... для LaTeX
                funcName = System.Text.RegularExpressions.Regex.Replace(funcName, @"x(\d+)", "x_{$1}");

                // Оформляем математические функции
                funcName = funcName.Replace("sin", "\\sin")
                                   .Replace("cos", "\\cos")
                                   .Replace("tg", "\\tan")
                                   .Replace("ctg", "\\cot")
                                   .Replace("ln", "\\ln")
                                   .Replace("exp", "e^")
                                   .Replace("^2", "^{2}")
                                   .Replace("^3", "^{3}");

                if (_function[i] is Constant)
                    result += $"{sign}{val}";
                else
                    result += $"{sign}{val} \\cdot {funcName}";

                isFirstVisible = false; // Последующие элементы уже не первые
            }

            // Если все коэффициенты оказались нулевыми
            if (isFirstVisible) result += "0";

            return result;
        }

    }
}