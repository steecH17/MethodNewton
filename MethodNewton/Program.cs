using MethodNewton;
using System;
using MathNet.Numerics.LinearAlgebra;
using Aspose.Cells.Drawing.Equations;

class Program
{
    static void Main(string[] args)
    {
        // TestMethod();

        TestSolution();

        // TestParser();
        // Console.WriteLine("TEST DATAGENERATOR");
        // DataGenerator dataGenerator = new DataGenerator("x^2 + 1", 3.0);
        // List<double> xGen, yGen;
        // (xGen, yGen) = dataGenerator.GenerateData2D(20);
        // PrintInputData(xGen, yGen);

        // string fileName = "Data.xlsx";
        // string dataPath = "../data/" + fileName;

        // ExcelReader excelReader = new ExcelReader(dataPath);
        // List<double> xList = new List<double>();
        // List<double> yList = new List<double>();
        // List<double> xData;
        // List<double> yData;
        // (xData, yData) = excelReader.ExcelFileRead();

        // for (int i = 0; i < xData.Count; i++)
        // {
        //     //функция не определена при x = 0
        //     if (xData[i] != 0d)
        //     {
        //         xList.Add(xData[i]);
        //         yList.Add(yData[i]);
        //     }
        // }

        // // Параметры метода
        // double tolerance = 1e-6;
        // int maxIterations = 1000;
        // string expression = "x^a + b*sin(x) - c";
        // Vector<double> initGuess = Vector<double>.Build.Dense(new double[] {1.0, 1.0, 1.0 });

        // var solver = new NewtonMethodSolver(expression);
        // Vector<double> coef = solver.FindCoeff(xList.ToArray(), yList.ToArray(), initGuess, tolerance, maxIterations);
        
        // PrintDataTable(solver, coef, xList, yList);

        // var chartsGen = new ChartsGenerator(expression);
        // chartsGen.PlotAndSave(xData, yData, coef);
    }

    static void TestSolution()
    {

        string fileName = "Data.xlsx";
        string dataPath = "../data/" + fileName;

        ExcelReader excelReader = new ExcelReader(dataPath);
        List<double> xGen = new List<double>();
        List<double> yGen = new List<double>();
        (xGen, yGen) = excelReader.ExcelFileRead();

        double tolerance = 1e-6;

        // DataGenerator dataGenerator = new DataGenerator("4*sin(x) + 2*cos(x)", 0.5);
        // List<double> xGen, yGen;
        // (xGen, yGen) = dataGenerator.GenerateData2D(1, 30);

        PrintInputData(xGen, yGen);

        List<IFunction> functionComponents = new List<IFunction> {new SinFunc(), new CosFunc(), new TgFunc(),
            new CtgFunc(), new LnFunc(), new ExpFunc(), new LinearFunc(),
            new PowFuncDecorator(new LinearFunc(), 2), new PowFuncDecorator(new LinearFunc(), 3), new Constant()};

        OptimalFunctionFinder finder = new OptimalFunctionFinder(functionComponents);
        finder.FindOptimalFunction(xGen, yGen, tolerance);

        PrintDataTable(finder.NewtonSolver, finder.CoeffFunctions, xGen, yGen);
        PrintFunction(finder.OptimalFunction, finder.CoeffFunctions);

        ChartsGenerator chartsGenerator = new ChartsGenerator(finder.OptimalFunction);
        chartsGenerator.PlotAndSave(xGen, yGen, finder.CoeffFunctions);
    }

    static void TestMethod()
    {
        double tolerance = 1e-6;

        DataGenerator dataGenerator = new DataGenerator("2*sin(x) + cos(x)", 0);
        List<double> xGen, yGen;
        (xGen, yGen) = dataGenerator.GenerateData2D(1, 100);

        PrintInputData(xGen, yGen);

        List<IFunction> functionComponents = new List<IFunction> {new SinFunc(), new CosFunc(), new TgFunc(),
            new CtgFunc(), new LnFunc(), new ExpFunc()};

        OptimalFunctionFinder finder = new OptimalFunctionFinder(functionComponents);

        NewtonMethodSolver solver = new NewtonMethodSolver(new List<IFunction> {new SinFunc(), new CosFunc()});

        Vector<double> initGuess = Vector<double>.Build.Dense(new double[] {1.0, 1.0});
        Vector<double> coef = solver.FindCoeff(xGen.ToArray(), yGen.ToArray(), initGuess, tolerance, 1000);
        Console.WriteLine($"Determ = {solver.DeterminationCoeff(xGen.ToArray(), yGen.ToArray(), coef)}");
        foreach (var item in coef)
        {
            Console.WriteLine($"{item}");
        }

    }

    static void PrintFunction(List<IFunction> function, Vector<double> coeff)
    {
        Console.WriteLine("RESULTING FUNCTION:");

        for(int i = 0; i < function.Count; i++)
        {
            string sign =  coeff[i] >=0 ? "+" : "-";
            if(i!=0)
                Console.Write($"{sign}");

            Console.Write($"{Math.Abs(Math.Round(coeff[i], 4))}*{function[i]}");   
        }

        Console.WriteLine("");
    }

    static void PrintDataTable(NewtonMethodSolver solver, Vector<double> coef, List<double> xList, List<double> yList)
    {
        
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

    static void PrintInputData(List<double> xData, List<double> yData)
    {
        Console.WriteLine("---INPUT DATA---");
        for(int i = 0; i < xData.Count; i++)
        {
            Console.WriteLine($"{xData[i]}\t{yData[i]}");
        }
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
