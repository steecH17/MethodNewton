using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace MethodNewton
{
    public class NewtonMethodSolver
    {
        private List<IFunction> _function;
        private int _iterConvergence;
        private Stopwatch timer = new Stopwatch();
        public int IterConvergence => _iterConvergence;

        public NewtonMethodSolver(List<IFunction> function)
        {
            _iterConvergence = -1;
            _function = function;
        }

        public Stopwatch SolveTime => timer;

        /// <summary>
        /// Метод для поиска коэффициентов
        /// </summary>
        /// <param name="x">Массив векторов аргументов. x[i] - это набор аргументов (x1, x2...) для i-го наблюдения</param>
        /// <param name="y">Массив значений функции</param>
        /// <param name="a">Начальное приближение коэффициентов</param>
        public Vector<double> FindCoeff(double[][] x, double[] y, Vector<double> a, double tolerance, int maxIterations)
        {
            timer.Start();

            if (a.Count != _function.Count)
                throw new ArgumentException($"Количество коэффициентов ({a.Count}) не совпадает с количеством функций ({_function.Count}).");

            if (x.Length != y.Length)
                throw new ArgumentException("Количество точек данных X и значений Y должно совпадать.");

            int n = x.Length; // Количество наблюдений
            int m = a.Count;  // Количество коэффициентов
            double eps = 0.00001;
            
            double determCoef = DeterminationCoeff(x, y, a);

            for (int iter = 0; iter < maxIterations; iter++)
            {
                Matrix<double> J = Matrix<double>.Build.Dense(n, m);
                Vector<double> F = Vector<double>.Build.Dense(n);

                for (int i = 0; i < n; i++)
                {
                    // Считаем невязку. Передаем ВЕСЬ вектор аргументов x[i]
                    F[i] = Residuals(x[i], a, y[i]);

                    // Заполняем строку матрицы Якоби
                    for (int j = 0; j < m; j++)
                    {
                        // Считаем производную по j-му коэффициенту
                        J[i, j] = DerivativeByCoefficient(x[i], a, j); 
                    }
                }

                Matrix<double> JTJ = J.TransposeThisAndMultiply(J);
                Vector<double> JTF = J.TransposeThisAndMultiply(-F);

                // Регуляризация (для стабильности при m > n или линейно зависимых функциях)
                Matrix<double> regularized = JTJ + Matrix<double>.Build.DenseIdentity(m) * 1e-8;

                Vector<double> deltaA;
                try
                {
                    deltaA = regularized.Svd().Solve(JTF);
                }
                catch
                {
                    break; 
                }

                a += deltaA;

                double newDetermCoef = DeterminationCoeff(x, y, a);

                // Условие выхода
                if (deltaA.L2Norm() < tolerance && Math.Abs(determCoef - newDetermCoef) < eps)
                {
                    _iterConvergence = iter + 1;
                    break;
                }

                determCoef = newDetermCoef;
            }

            timer.Stop();

            return a;
        }

        public double DeterminationCoeff(double[][] x, double[] y, Vector<double> a)
        {
            int k = x.Length; // Число точек
            int p = a.Count;  // Число параметров

            double ssRes = SumSquareResiduals(x, y, a);
            double ssTot = SumSquareAverage(y);

            // Если дисперсия данных 0 (все y одинаковые), R2 не определен, возвращаем 1 (идеально) или 0
            if (Math.Abs(ssTot) < 1e-9) return 1.0; 

            double rSquared = 1 - (ssRes / ssTot);

            // Скорректированный R^2
            if (k <= p + 1) return rSquared;

            double adjustedRSquared = 1 - (1 - rSquared) * (double)(k - 1) / (k - p - 1);
            return adjustedRSquared;
        }

        public double Function(double[] x, Vector<double> coeff)
        {
            double functionValue = 0;
            for(int i = 0; i < _function.Count; i++)
            {
                functionValue += _function[i].Calculate(x, coeff[i]);
            }
            return functionValue;
        }

        private double Residuals(double[] x, Vector<double> a, double y)
        {
            return Function(x, a) - y;
        }

        private double SumSquareResiduals(double[][] x, double[] y, Vector<double> a)
        {
            double sumSquareResiduals = 0;
            for (int i = 0; i < x.Length; i++) 
            {
                sumSquareResiduals += Math.Pow(y[i] - Function(x[i], a), 2);
            }
            return sumSquareResiduals;
        }

        private double SumSquareAverage(double[] y)
        {
            double sumSquareAverage = 0;
            double yAverage = y.Average();
            for (int i = 0; i < y.Length; i++)
            {
                sumSquareAverage += Math.Pow(y[i] - yAverage, 2);
            }
            return sumSquareAverage;
        }

        private double DerivativeByCoefficient(double[] x, Vector<double> coeff, int coeffIndex, double delta = 1e-6)
        {
            Vector<double> coeffPlus = coeff.Clone();
            Vector<double> coeffMinus = coeff.Clone();

            coeffPlus[coeffIndex] += delta;
            coeffMinus[coeffIndex] -= delta;

            double fPlus = Function(x, coeffPlus);
            double fMinus = Function(x, coeffMinus);

            return (fPlus - fMinus) / (2 * delta);
        }
    }
}