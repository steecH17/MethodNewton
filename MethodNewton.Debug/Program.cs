using MethodNewton;
using System;
using MathNet.Numerics.LinearAlgebra;
using Aspose.Cells.Drawing.Equations;
using MathNet.Numerics.LinearAlgebra.Solvers;

class Program
{
    static void Main(string[] args)
    {
        // TestMethod();

        // TestSolutionScript2();

        TestSolutionScript2();
    }

    // static void TestSolutionScript1()
    // {

    //     string fileName = "Data.xlsx";
    //     string dataPath = "../data/" + fileName;

    //     ExcelReader excelReader = new ExcelReader(dataPath);
    //     List<List<double>> xGen = new List<List<double>>();
    //     List<double> yGen = new List<double>();
    //     (xGen, yGen) = excelReader.ExcelFileRead();

    //     double tolerance = 1e-6;

    //     // DataGenerator dataGenerator = new DataGenerator("4*sin(x) + 2*cos(x)", 0.5);
    //     // List<double> xGen, yGen;
    //     // (xGen, yGen) = dataGenerator.GenerateData2D(1, 30);

    //     PrintInputData(xGen, yGen);

    //     List<IFunction> functionComponents = new List<IFunction> {new SinFunc(), new CosFunc(), new TgFunc(),
    //         new CtgFunc(), new LnFunc(), new ExpFunc(), new LinearFunc(),
    //         new PowFuncDecorator(new LinearFunc(), 2), new PowFuncDecorator(new LinearFunc(), 3), new Constant()};

    //     OptimalFunctionFinder finder = new OptimalFunctionFinder(functionComponents);
    //     StatisticModels statisticModels = new StatisticModels(finder.FindOptimalFunction(xGen, yGen, tolerance));
    //     FunctionModel model = statisticModels.GetMaxDetermModel();

    //     // PrintDataTable(model, xGen, yGen);
    //     // PrintFunction(model.Function, model.CoeffFunction);

    //     // ChartsGenerator chartsGenerator = new ChartsGenerator(model.Function);
    //     // chartsGenerator.PlotAndSave(xGen, yGen, model.CoeffFunction);
    // }

    static void TestSolutionScript2()
    {

        string fileName = "Data3D.xlsx";
        string dataPath = "../data/" + fileName;

        ExcelReader excelReader = new ExcelReader(dataPath);
        List<List<double>> xGen = new List<List<double>>();
        List<double> yGen = new List<double>();
        (xGen, yGen) = excelReader.ExcelFileRead();

        double tolerance = 1e-6;

        // DataGenerator dataGenerator = new DataGenerator("4*sin(x) + 2*cos(x)", 0.5);
        // List<List<double>> xGen;
        // List<double> yGen;
        // (xGen, yGen) = dataGenerator.GenerateData2D(1, 30);

        ConsolePrinter.PrintInputData(xGen, yGen);
        Console.WriteLine($"{xGen.Count} - CountX");

        // List<IFunction> functionComponents = new List<IFunction> {new SinFunc(), new CosFunc(), new TgFunc(),
        //     new CtgFunc(), new LnFunc(), new ExpFunc(), new LinearFunc(),
        //     new PowFuncDecorator(new LinearFunc(), 2), new PowFuncDecorator(new LinearFunc(), 3), new Constant()};

        List<IFunction> functionComponents = new List<IFunction> {new SinFunc(),
            new LinearFunc(), new ExpFunc(),
            new Constant()};

        OptimalFunctionFinder finder = new OptimalFunctionFinder(functionComponents);
        StatisticModels statisticModels = new StatisticModels(finder.FindOptimalFunction(xGen, yGen, tolerance));
        FunctionModel model = statisticModels.GetMaxDetermModel();

        ConsolePrinter.PrintDataTable(model, xGen, yGen);
        Console.WriteLine($"Время поиска оптимальной функции: {finder.SearchTime.ElapsedMilliseconds} ms");
        ConsolePrinter.PrintFunction(model.Function, model.CoeffFunction);
        Console.WriteLine($"Время поиска коэффициентов методом Ньютона: {model.SolveTime.ElapsedMilliseconds} ms");
        // Console.WriteLine($"Коэффициент детерминации : {Math.Round(model.DeterminationCoeff, 5)}");

        // ChartsGenerator chartsGenerator = new ChartsGenerator(model.Function);
        // chartsGenerator.PlotAndSave(xGen, yGen, model.CoeffFunction);
    }

    static void TestSolution3D()
    {
        string fileName = "Data.xlsx";
        string dataPath = "../data/" + fileName;

        ExcelReader excelReader = new ExcelReader(dataPath);
        List<List<double>> xGen = new List<List<double>>();
        List<double> yGen = new List<double>();
        (xGen, yGen) = excelReader.ExcelFileRead();

        double tolerance = 1e-6;

        ConsolePrinter.PrintInputData(xGen, yGen);
    }

    // static void TestMethod()
    // {
    //     double tolerance = 1e-6;

    //     DataGenerator dataGenerator = new DataGenerator("2*sin(x) + cos(x)", 0);
    //     List<double> xGen, yGen;
    //     (xGen, yGen) = dataGenerator.GenerateData2D(1, 100);

    //     PrintInputData(xGen, yGen);

    //     List<IFunction> functionComponents = new List<IFunction> {new SinFunc(), new CosFunc(), new TgFunc(),
    //         new CtgFunc(), new LnFunc(), new ExpFunc()};

    //     OptimalFunctionFinder finder = new OptimalFunctionFinder(functionComponents);

    //     NewtonMethodSolver solver = new NewtonMethodSolver(new List<IFunction> {new SinFunc(), new CosFunc()});

    //     Vector<double> initGuess = Vector<double>.Build.Dense(new double[] {1.0, 1.0});
    //     Vector<double> coef = solver.FindCoeff(xGen.ToArray(), yGen.ToArray(), initGuess, tolerance, 1000);
    //     Console.WriteLine($"Determ = {solver.DeterminationCoeff(xGen.ToArray(), yGen.ToArray(), coef)}");
    //     foreach (var item in coef)
    //     {
    //         Console.WriteLine($"{item}");
    //     }

    // }

    

    // static void PrintInputData(List<double> xData, List<double> yData)
    // {
    //     Console.WriteLine("---INPUT DATA---");
    //     for(int i = 0; i < xData.Count; i++)
    //     {
    //         Console.WriteLine($"{xData[i]}\t{yData[i]}");
    //     }
    // }

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
