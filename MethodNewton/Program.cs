using MethodNewton;
using System;
using MathNet.Numerics.LinearAlgebra;
using Aspose.Cells.Drawing.Equations;

class Program
{
    static void Main(string[] args)
    {
        TestParser();

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
        string expression = "x^a + b*sin(x) - c"; ;
        Vector<double> initGuess = Vector<double>.Build.Dense(new double[] {1.0, 1.0, 1.0 });

        var solver = new NewtonMethodSolver(expression);
        Vector<double> coef = solver.FindCoeff(xList.ToArray(), yList.ToArray(), initGuess, tolerance, maxIterations);
        
        PrintDataTable(solver, coef, xList, yList);

        var chartsGen = new ChartsGenerator(expression);
        chartsGen.PlotAndSave(xData, yData, coef);
    }

    static void PrintDataTable(NewtonMethodSolver solver, Vector<double> coef, List<double> xList, List<double> yList)
    {
         string sign = coef[2] >=0 ? "+" : "-";
        Console.WriteLine($"Функция f(x) = x^({Math.Round(coef[0], 5)}) + ({Math.Round(coef[1], 5)})*sin(x) {sign} ({Math.Abs(Math.Round(coef[2], 5))})\n");
        //title
        Console.WriteLine($"X\t| Y\t| f(X)\t|");
        Console.WriteLine($"--------|-------|-------|");
        for (int i = 0; i < xList.Count; i++)
        {
            Console.WriteLine($"{Math.Round(xList[i], 2)}\t|{Math.Round(yList[i], 2)}\t|{Math.Round(solver.Function(xList[i], coef), 2)}\t|");
        }
        Console.WriteLine($"------------------------|");
        Console.WriteLine($"Сходимость достигнута на итерации : {solver.IterConvergence}");

        Console.WriteLine($"Коэффициент детерминации : {Math.Round(solver.DeterminationCoeff(xList.ToArray(), yList.ToArray(), coef), 5)}");
    }
    static void TestParser()
    {
        string expression = "x^a + b*sin(x) - c";
        var parser = new MathParser();
        var mathExpr = parser.Parse(expression);

        var varName = parser.VariablesName;
        foreach (var item in varName)
        {
            Console.WriteLine(item);
        }

        Dictionary<string, double> var1 = new Dictionary<string, double> { { "x", 57 } };
        Dictionary<string, double> var2 = new Dictionary<string, double> { { "x", 57 },
            {"a", 0.99909}, {"b", 0.7092}, {"c", 5.96401}};


        Console.WriteLine(mathExpr.Evaluate(var2).ToString());
    }
    
}
