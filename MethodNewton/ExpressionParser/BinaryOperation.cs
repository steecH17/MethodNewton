using System;

public class BinaryOperation : MathExpression
{
    public string Operator { get; }
    public MathExpression Left { get; }
    public MathExpression Right { get; }

    public BinaryOperation(MathExpression left, string op, MathExpression right)
    {
        Operator = op;
        Left = left;
        Right = right;
    }
    public override double Evaluate(Dictionary<string, double>? variables = null)
    {
        double leftValue = Left.Evaluate(variables);
        double rightValue = Right.Evaluate(variables);

        switch (Operator)
        {
            case "+": return leftValue + rightValue;
            case "-": return leftValue - rightValue;
            case "*": return leftValue * rightValue;
            case "/": return leftValue / rightValue;
            case "^": return Math.Pow(leftValue, rightValue);
            default:
                throw new Exception($"Неизвестный оператор : {Operator}");
        }
    }
}