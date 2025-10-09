using System;

public class Number : MathExpression
{
    public double Value { get; }
    public Number(double value)
    {
        Value = value;
    }

    public override double Evaluate(Dictionary<string, double>? variables = null)
    {
        return Value;
    }
}