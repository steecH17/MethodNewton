using System;

public class UnaryOperation : MathExpression
{
    public string Operator { get; }
    public MathExpression Expression { get; }

    public UnaryOperation(string op, MathExpression expression)
    {
        Operator = op;
        Expression = expression;
    }

    public override double Evaluate(Dictionary<string, double> variables = null)
    {
        double value = Expression.Evaluate(variables);

        switch (Operator)
        {
            case "-": return -value;
            default:
                throw new Exception($"Неизвестный унарный оператор : {Operator}");
        }
    }
}