using System;

public class MathFunction : MathExpression
{
    public string FunctionName { get; }
    public MathExpression Argument { get; }
    
    public MathFunction(string functionName, MathExpression argument)
    {
        FunctionName = functionName;
        Argument = argument;
    }
    
    public override double Evaluate(Dictionary<string, double>? variables = null)
    {
        double argValue = Argument.Evaluate(variables);
        
        return FunctionName.ToLower() switch
        {
            "sin" => Math.Sin(argValue),
            "cos" => Math.Cos(argValue),
            "tan" => Math.Tan(argValue),
            "log" => Math.Log(argValue),
            "exp" => Math.Exp(argValue),
            "sqrt" => Math.Sqrt(argValue),
            _ => throw new Exception($"Неизвестная функция: {FunctionName}")
        };
    }
}