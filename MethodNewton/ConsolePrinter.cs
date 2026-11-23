using System;
using MathNet.Numerics.LinearAlgebra;

namespace MethodNewton
{
    public static class ConsolePrinter
    {
        private const int ColWidth = 12;

        public static void PrintInputData(List<List<double>> xData, List<double> yData)
        {
            Console.WriteLine("---INPUT DATA---");
            for (int i = 0; i < yData.Count; i++)
            {
                for (int j = 0; j < xData[0].Count; j++)
                    Console.Write($"{xData[i][j]}\t");
                Console.Write($"{yData[i]}\n");
            }
        }

        public static void PrintDataTable(FunctionModel model, List<List<double>> xList, List<double> yList)
        {
            CompositeFunc function = new CompositeFunc(model.Function);
            int dimension = xList[0].Count;

            Console.Write("\nOUTPUT DATA\n");

            for (int i = 0; i < dimension; i++)
            {
                Console.Write($"{"x" + (i + 1),-ColWidth} | ");
            }
            Console.Write($"{"Y",-ColWidth} | {"f(x)",-ColWidth} |\n");

            string separator = new string('-', (dimension + 2) * (ColWidth + 3));

            for (int i = 0; i < xList.Count; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    Console.Write($"{Math.Round(xList[i][j], 2),-ColWidth} | ");
                }

                double calcValue = function.Calculate(xList[i].ToArray(), model.CoeffFunction);
                
                Console.Write($"{Math.Round(yList[i], 2),-ColWidth} | ");
                Console.Write($"{Math.Round(calcValue, 2),-ColWidth} |\n");
            }
            Console.WriteLine(separator);
            Console.WriteLine($"Сходимость достигнута на итерации : {model.IterConvergence}");

            Console.WriteLine($"Коэффициент детерминации : {Math.Round(model.DeterminationCoeff, 5)}");
        }

        public static void PrintFunction(List<IFunction> function, Vector<double> coeff)
        {
            Console.WriteLine("RESULTING FUNCTION:");

            for (int i = 0; i < function.Count; i++)
            {
                string sign = coeff[i] >= 0 ? "+" : "-";
                if (i != 0)
                    Console.Write($"{sign}");

                if (function[i] is Constant)
                    Console.Write($"{Math.Abs(Math.Round(coeff[i], 4))}");
                else
                    Console.Write($"{Math.Abs(Math.Round(coeff[i], 4))}*{function[i]}");
            }

            Console.WriteLine("");
        }
    }
}