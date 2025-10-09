using System;

public class Variable : MathExpression
{
    public string Name { get; }
    public Variable(string name)
    {
        Name = name;
    }
    public override double Evaluate(Dictionary<string, double>? variables = null)
    {
        if (variables != null && variables.ContainsKey(Name))
        {
            return variables[Name];
        }

        throw new Exception($"Неизвестная переменная : {Name}");
    }
}