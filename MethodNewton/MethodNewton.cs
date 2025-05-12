using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethodNewton
{
    public class MethodNewtons
    {
        public static double Function(double x, double a, double b, double c)
        {
            return Math.Pow(x, a) + b * Math.Sin(x) + c;
        }

        static double Residuals(double x, Vector<double> a, double y)
        {
            return Function(x, a[0], a[1], a[2]) - y;
        }

        public static double SumSquareResiduals(double[] x, double[] y, Vector<double> a)
        {
            double sumSquareResiduals = 0;

            for (int i = 0; i < x.Length; i++) sumSquareResiduals += Math.Pow(y[i] - Function(x[i], a[0], a[1], a[2]), 2);

            return sumSquareResiduals;
        }

        public static double SumSquareAverage(double[] y)
        {
            double sumSquareAverage = 0;
            double yAverage = y.Average();
            for (int i = 0; i < y.Length; i++)
            {
                sumSquareAverage += Math.Pow(y[i] - yAverage, 2);
            }
            return sumSquareAverage;
        }

        public static double DeterminationCoeff(double[] x, double[] y, Vector<double> a)
        {
            int k = x.Length;
            int n = 1;
            double rSquared = 1 - (SumSquareResiduals(x, y, a)) / (SumSquareAverage(y));
            double adjustedRSquared = 1 - (1 - rSquared) * (k - 1) / (k - n - 1);
            return adjustedRSquared;
        }

        public static double CalculateDeterminationCoefficient(double[] x, double[] y, Vector<double> a)
        {
            double ssRes = SumSquareResiduals(x, y, a);
            double ssTot = SumSquareAverage(y);
            Console.WriteLine($"ssRes = {SumSquareResiduals(x, y, a)}");
            Console.WriteLine($"ssTot = {SumSquareAverage(y)}");
            

            // Избегаем деления на ноль (если все y одинаковы, R² не имеет смысла)
            if (Math.Abs(ssTot) < 1e-10)
                return double.NaN;

            return 1.0 - (ssRes / ssTot);
        }

        public static Vector<double> Newton(double[] x, double[] y, Vector<double> a, double tolerance, int maxIterations)
        {
            int n = x.Length;
            int m = a.Count;
            double eps = 0.001;
            double determCoef = DeterminationCoeff(x, y, a);

            for (int iter = 0; iter < maxIterations; iter++)
            {
                // Матрица Якоби и вектор невязок
                Matrix<double> J = Matrix<double>.Build.Dense(n, m);
                Vector<double> F = Vector<double>.Build.Dense(n);

                for (int i = 0; i < n; i++)
                {
                    // Невязка
                    F[i] = Residuals(x[i], a, y[i]);

                    // Частные производные
                    J[i, 0] = Math.Pow(x[i], a[0]) * Math.Log(x[i]); // df/da = x^a * ln(x)
                    J[i, 1] = Math.Sin(x[i]);                   // df/db = sin(x)
                    J[i, 2] = 1;                     // df/dc = 1
                }

                // Решение системы J * Δa = -F
                Vector<double> deltaA = J.TransposeThisAndMultiply(J).Cholesky().Solve(J.TransposeThisAndMultiply(-F));

                // Обновление коэффициентов
                a += deltaA;

                // Проверка на сходимость
                if (deltaA.L2Norm() < tolerance && Math.Abs(determCoef - DeterminationCoeff(x, y, a)) < eps)
                {
                    Console.WriteLine($"Сходимость достигнута на итерации {iter + 1}");
                    break;
                }

                determCoef = DeterminationCoeff(x, y, a);
            }

            return a;
        }


    }
}
