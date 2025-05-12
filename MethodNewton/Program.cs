using MethodNewton;
using System;
using MathNet.Numerics.LinearAlgebra;
using Aspose.Cells.Drawing.Equations;

class Program
{
    static void Main(string[] args)
    {
        string fileName = "Data.xlsx";
        string dataPath = "../data/" + fileName;
        ExcelReader excelReader = new ExcelReader(dataPath);
        List<double> xList = new List<double>();
        List<double> yList = new List<double>();
        List<double> xData;
        List<double> yData;
        (xData, yData) = excelReader.ExcelFileRead();

        for (int i = 0; i < xData.Count; i++)
        {
            //функция не определена при x = 0
            if (xData[i] != 0d)
            {
                xList.Add(xData[i]);
                yList.Add(yData[i]);
            }
        }
        // Параметры метода
        double tolerance = 1e-6;
        int maxIterations = 1000;
        Vector<double> initGuess = Vector<double>.Build.Dense(new double[] { 1.0, 1.0, 1.0 });
        Vector<double> coef = MethodNewtons.Newton(xList.ToArray(), yList.ToArray(), initGuess, tolerance, maxIterations);

        string sign = coef[2] >=0 ? "+" : "-";
        Console.WriteLine($"Функция f(x) = x^({Math.Round(coef[0], 5)}) + ({Math.Round(coef[1], 5)})*sin(x) {sign} ({Math.Abs(Math.Round(coef[2], 5))})\n");
        //title
        Console.WriteLine($"X\t| Y\t| f(X)\t|");
        Console.WriteLine($"--------|-------|-------|");
        for (int i = 0; i < xList.Count; i++)
        {
            Console.WriteLine($"{Math.Round(xList[i], 2)}\t|{Math.Round(yList[i], 2)}\t|{Math.Round(MethodNewtons.Function(xList[i], coef[0], coef[1], coef[2]), 2)}\t|");
        }
        Console.WriteLine($"------------------------|");

        
        Console.WriteLine($"Коэффициент детерминации : {Math.Round(MethodNewtons.DeterminationCoeff(xList.ToArray(), yList.ToArray(), coef), 5)}");

        ChartsGenerator.PlotAndSave(xData, yData, coef);
    }
    
}
