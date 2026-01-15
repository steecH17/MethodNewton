using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MethodNewton
{
    public class NewtonMethodSolver
    {
        private List<IFunction> _function;
        private int _iterConvergence;
        public int IterConvergence => _iterConvergence;

        public NewtonMethodSolver(List<IFunction> function)
        {
            _iterConvergence = -1;
            _function = function;
        }

        public Vector<double> FindCoeff(double[] x, double[] y, Vector<double> a, double tolerance, int maxIterations)
        {
            // Проверка на соответствие размерностей (защита от дурака)
            if (a.Count != _function.Count)
            {
                throw new ArgumentException($"Количество коэффициентов ({a.Count}) должно совпадать с количеством функций ({_function.Count}).");
            }

            int n = x.Length; // Количество точек данных
            int m = a.Count;  // Количество искомых коэффициентов
            double eps = 0.00001;
            
            // Начальная оценка детерминации
            double determCoef = DeterminationCoeff(x, y, a);

            for (int iter = 0; iter < maxIterations; iter++)
            {
                // Матрица Якоби (n строк, m столбцов) и вектор невязок (n строк)
                Matrix<double> J = Matrix<double>.Build.Dense(n, m);
                Vector<double> F = Vector<double>.Build.Dense(n);

                for (int i = 0; i < n; i++)
                {
                    // 1. Считаем невязку для i-й точки
                    F[i] = Residuals(x[i], a, y[i]);

                    // 2. ВЫЧИСЛЯЕМ ЧАСТНЫЕ ПРОИЗВОДНЫЕ (Заполняем строку матрицы Якоби)
                    // Теперь это работает для любого количества коэффициентов m
                    for (int j = 0; j < m; j++)
                    {
                        // df/d(coeff_j)
                        J[i, j] = DerivativeByCoefficient(x[i], a, j); 
                    }
                }

                Matrix<double> JTJ = J.TransposeThisAndMultiply(J);
                Vector<double> JTF = J.TransposeThisAndMultiply(-F);

                // Регуляризация Левенберга-Марквардта (немного улучшает стабильность)
                Matrix<double> regularized = JTJ + Matrix<double>.Build.DenseIdentity(m) * 1e-8;

                // Решение системы линейных уравнений
                Vector<double> deltaA;
                try
                {
                    deltaA = regularized.Svd().Solve(JTF);
                }
                catch
                {
                    // Если SVD не справился (редко), выходим
                    break; 
                }

                // Обновление коэффициентов
                a += deltaA;

                // Новое значение детерминации
                double newDetermCoef = DeterminationCoeff(x, y, a);

                // Проверка на сходимость:
                // Если изменение вектора коэффициентов мало И изменение R^2 мало
                if (deltaA.L2Norm() < tolerance && Math.Abs(determCoef - newDetermCoef) < eps)
                {
                    _iterConvergence = iter + 1;
                    break;
                }

                determCoef = newDetermCoef;
            }

            return a;
        }

        public double DeterminationCoeff(double[] x, double[] y, Vector<double> a)
        {
            int k = x.Length; // Количество наблюдений
            int p = a.Count;  // Количество параметров модели (раньше было захардкожено n=1)

            double ssRes = SumSquareResiduals(x, y, a);
            double ssTot = SumSquareAverage(y);
            
            // Обычный R^2
            double rSquared = 1 - (ssRes / ssTot);

            // Скорректированный R^2 (Adjusted R-Squared)
            // Формула: 1 - (1 - R^2) * (N - 1) / (N - p - 1)
            // Если k <= p + 1, знаменатель будет <= 0, возвращаем обычный R^2 или 0 во избежание NaN
            if (k <= p + 1) return rSquared;

            double adjustedRSquared = 1 - (1 - rSquared) * (double)(k - 1) / (k - p - 1);
            return adjustedRSquared;
        }

        public double Function(double x, Vector<double> coeff)
        {
            double functionValue = 0;
            
            // ВАЖНО: Предполагается, что coeff[i] соответствует _function[i]
            for(int i = 0; i < _function.Count; i++)
            {
                functionValue += _function[i].Calculate(x, coeff[i]);
            }

            return functionValue;
        }

        private double Residuals(double x, Vector<double> a, double y)
        {
            return Function(x, a) - y;
        }

        private double SumSquareResiduals(double[] x, double[] y, Vector<double> a)
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

        // Убрал параметр 'y' из аргументов, так как он сокращается при взятии производной
        private double DerivativeByCoefficient(double x, Vector<double> coeff, int coeffIndex, double delta = 1e-6)
        {
            // Используем клонирование, чтобы не менять оригинальный вектор
            Vector<double> coeffPlus = coeff.Clone();
            Vector<double> coeffMinus = coeff.Clone();

            coeffPlus[coeffIndex] += delta;
            coeffMinus[coeffIndex] -= delta;

            double fPlus = Function(x, coeffPlus);
            double fMinus = Function(x, coeffMinus);

            // Центральная разностная схема
            return (fPlus - fMinus) / (2 * delta);
        }
    }
}