using System;

public abstract class MathExpression
{
    public abstract double Evaluate(Dictionary<string, double>? variables = null);
}