using System.ComponentModel;
using MethodNewton;

[Serializable]
public class AppSettings
{
    // Точность
    private double _tolerance = 1e-6;
    // Порог выбора
    private double _threshold = 0.9d;
    
    public double Tolerance
    {
        get => _tolerance;
        set => _tolerance = value;
    }

    public double Threshold
    {
        get => _threshold;
        set => _threshold = value;
    }
    
    // Компоненты функции
    public bool UseSin { get; set; } = true;
    public bool UseCos { get; set; } = true;
    public bool UseTg { get; set; } = false;
    public bool UseCtg { get; set; } = false;
    public bool UseExp { get; set; } = true;
    public bool UseLn { get; set; } = false;
    
    public bool UseX { get; set; } = true;      
   public bool UseX2 { get; set; } = false;
    public bool UseX3 { get; set; } = false;
    public bool UseX4 { get; set; } = false;
    public bool UseX5 { get; set; } = false;

    // Стратегия выбора функции
    public bool BestModel { get; set; } = true;
    public bool MostCompactThreshold { get; set; } = false;
    
    public List<IFunction> GetFunctionComponents()
    {
        var components = new List<IFunction>();
        
        if (UseSin) components.Add(new SinFunc());
        if (UseCos) components.Add(new CosFunc());
        if (UseTg) components.Add(new TgFunc());
        if (UseCtg) components.Add(new CtgFunc());
        if (UseExp) components.Add(new ExpFunc());
        if (UseLn) components.Add(new LnFunc());
        if (UseX) components.Add(new LinearFunc());
        if (UseX2) components.Add(new PowFuncDecorator(new LinearFunc(), 2));
        if (UseX3) components.Add(new PowFuncDecorator(new LinearFunc(), 3));
        if (UseX4) components.Add(new PowFuncDecorator(new LinearFunc(), 4));
        if (UseX5) components.Add(new PowFuncDecorator(new LinearFunc(), 5));

        components.Add(new Constant());
        
        return components;
    }

}