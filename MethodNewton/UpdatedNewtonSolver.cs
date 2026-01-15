// using MathNet.Numerics.LinearAlgebra;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;

// namespace MethodNewton
// {
//     public class NewtonMethodSolver
//     {
//         private MathParser _parser;
//         private MathExpression _function;
//         private List<string> _variablesName;
//         private int _iterConvergence;
//         public int IterConvergence => _iterConvergence;

//         public NewtonMethodSolver(string expression)
//         {
//             _parser = new MathParser();
//             _function = _parser.Parse(expression);
//             _iterConvergence = -1;
//             _variablesName = _parser.VariablesName;
//             _variablesName.Remove("x");
//         }

//         public Vector<double> FindCoeff(double[] x, double[] y, Vector<double> a, double tolerance, int maxIterations)
//         {
//             int n = x.Length;
//             int m = a.Count;
//             double eps = 0.00001;
//             double determCoef = DeterminationCoeff(x, y, a);

//             for (int iter = 0; iter < maxIterations; iter++)
//             {
//                 // Матрица Якоби и вектор невязок
//                 Matrix<double> J = Matrix<double>.Build.Dense(n, m);
//                 Vector<double> F = Vector<double>.Build.Dense(n);

//                 for (int i = 0; i < n; i++)
//                 {
//                     // Невязка
//                     F[i] = Residuals(x[i], a, y[i]);

//                     // ВЫЧИСЛЯЕМ ЧАСТНЫЕ ПРОИЗВОДНЫЕ ЧИСЛЕННО через парсер
//                     J[i, 0] = DerivativeByCoefficient(x[i], a, 0, y[i]); // df/da
//                     J[i, 1] = DerivativeByCoefficient(x[i], a, 1, y[i]); // df/db  
//                     J[i, 2] = DerivativeByCoefficient(x[i], a, 2, y[i]); // df/dc
//                 }

//                 Matrix<double> JTJ = J.TransposeThisAndMultiply(J);
//                 Vector<double> JTF = J.TransposeThisAndMultiply(-F);

//                 // Добавляем регуляризацию для устойчивости
//                 Matrix<double> regularized = JTJ + Matrix<double>.Build.DenseIdentity(m) * 1e-8;

//                 // Решение системы с SVD
//                 Vector<double> deltaA = regularized.Svd().Solve(JTF);

//                 // Обновление коэффициентов
//                 a += deltaA;

//                 // Проверка на сходимость
//                 if (deltaA.L2Norm() < tolerance && Math.Abs(determCoef - DeterminationCoeff(x, y, a)) < eps)
//                 {
//                     _iterConvergence = iter + 1;
//                     break;
//                 }
               
//                 determCoef = DeterminationCoeff(x, y, a);
//             }

//             return a;
//         }

//         public double DeterminationCoeff(double[] x, double[] y, Vector<double> a)
//         {
//             int k = x.Length;
//             int n = 1;
//             double rSquared = 1 - (SumSquareResiduals(x, y, a)) / (SumSquareAverage(y));
//             double adjustedRSquared = 1 - (1 - rSquared) * (k - 1) / (k - n - 1);
//             return adjustedRSquared;
//         }

//         public double Function(double x, Vector<double> coeff)
//         {
//             Dictionary<string, double> variables = new Dictionary<string, double>
//             {
//                 { "x", x }
//             };

//             for (int i = 0; i < coeff.Count; i++)
//             {
//                 variables.Add(_variablesName[i], coeff[i]);
//             }

//             return _function.Evaluate(variables);
//         }

//         private double Residuals(double x, Vector<double> a, double y)
//         {
//             return Function(x, a) - y;
//         }

//         private double SumSquareResiduals(double[] x, double[] y, Vector<double> a)
//         {
//             double sumSquareResiduals = 0;

//             for (int i = 0; i < x.Length; i++) sumSquareResiduals += Math.Pow(y[i] - Function(x[i], a), 2);

//             return sumSquareResiduals;
//         }

//         private double SumSquareAverage(double[] y)
//         {
//             double sumSquareAverage = 0;
//             double yAverage = y.Average();
//             for (int i = 0; i < y.Length; i++)
//             {
//                 sumSquareAverage += Math.Pow(y[i] - yAverage, 2);
//             }
//             return sumSquareAverage;
//         }

//         private double DerivativeByCoefficient(double x, Vector<double> coeff, int coeffIndex, double y, double delta = 1e-6)
//         {
//             Vector<double> coeffPlus = coeff.Clone();
//             Vector<double> coeffMinus = coeff.Clone();

//             coeffPlus[coeffIndex] += delta;
//             coeffMinus[coeffIndex] -= delta;

//             double fPlus = Function(x, coeffPlus);
//             double fMinus = Function(x, coeffMinus);

//             return (fPlus - fMinus) / (2 * delta);
//         }

//         /*
//         public double CalculateDeterminationCoefficient(double[] x, double[] y, Vector<double> a)
//         {
//             double ssRes = SumSquareResiduals(x, y, a);
//             double ssTot = SumSquareAverage(y);
//             Console.WriteLine($"ssRes = {SumSquareResiduals(x, y, a)}");
//             Console.WriteLine($"ssTot = {SumSquareAverage(y)}");


//             // Избегаем деления на ноль (если все y одинаковы, R² не имеет смысла)
//             if (Math.Abs(ssTot) < 1e-10)
//                 return double.NaN;

//             return 1.0 - (ssRes / ssTot);
//         }*/

//     }
// }
