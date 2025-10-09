using System;
using System.Globalization;

public class MathParser
{
    private List<Token> _tokens;
    private int _currentTokenIndex;
    
    private Token CurrentToken => _currentTokenIndex < _tokens.Count ? _tokens[_currentTokenIndex] : new Token(TokenType.EOF);
    private List<string> _variablesName;
    public List<string> VariablesName => _variablesName;

    public MathParser()
    {
        _tokens = new List<Token>();
        _variablesName = new List<string>();
    }
    public MathExpression Parse(string expression)
    {
        var tokenizer = new Tokenizer(expression);
        _tokens = tokenizer.Tokenize();
        _currentTokenIndex = 0;
        _variablesName = new List<string>();

        var parsedExpression = ParseExpression();

        if (CurrentToken.Type != TokenType.EOF)
        {
            throw new Exception("Неожиданный токен в конце выражения");
        }

        return parsedExpression;
    }
    
    private MathExpression ParseExpression()
    {
        var left = ParseTerm();
        
        while (CurrentToken.Type == TokenType.Operator && 
               (CurrentToken.Value == "+" || CurrentToken.Value == "-"))
        {
            string op = CurrentToken.Value;
            Eat(TokenType.Operator);
            var right = ParseTerm();
            left = new BinaryOperation(left, op, right);
        }
        
        return left;
    }
    
    private MathExpression ParseTerm()
    {
        var left = ParseFactor();
        
        while (CurrentToken.Type == TokenType.Operator && 
               (CurrentToken.Value == "*" || CurrentToken.Value == "/"))
        {
            string op = CurrentToken.Value;
            Eat(TokenType.Operator);
            var right = ParseFactor();
            left = new BinaryOperation(left, op, right);
        }
        
        return left;
    }
    
    private MathExpression ParseFactor()
    {
        var left = ParsePower();
        
        while (CurrentToken.Type == TokenType.Operator && CurrentToken.Value == "^")
        {
            string op = CurrentToken.Value;
            Eat(TokenType.Operator);
            var right = ParseFactor(); // Правая ассоциативность для степени
            left = new BinaryOperation(left, op, right);
        }
        
        return left;
    }
    
    private MathExpression ParsePower()
    {
        return ParsePrimary();
    }
    
    private MathExpression ParsePrimary()
    {
        if (CurrentToken.Type == TokenType.Number)
        {
            var number = new Number(double.Parse(CurrentToken.Value, CultureInfo.InvariantCulture));
            Eat(TokenType.Number);
            return number;
        }
        else if (CurrentToken.Type == TokenType.Variable)
        {
            var variable = new Variable(CurrentToken.Value);
            if (!_variablesName.Contains(CurrentToken.Value))
            {
                _variablesName.Add(CurrentToken.Value);
            }
            Eat(TokenType.Variable);
            return variable;
        }
        else if (CurrentToken.Type == TokenType.Function)
        {
            string functionName = CurrentToken.Value;
            Eat(TokenType.Function);
            Eat(TokenType.LeftParenthesis);
            var argument = ParseExpression();
            Eat(TokenType.RightParenthesis);
            return new MathFunction(functionName, argument);
        }
        else if (CurrentToken.Type == TokenType.LeftParenthesis)
        {
            Eat(TokenType.LeftParenthesis);
            var expression = ParseExpression();
            Eat(TokenType.RightParenthesis);
            return expression;
        }
        else if (CurrentToken.Type == TokenType.Operator && CurrentToken.Value == "-")
        {
            Eat(TokenType.Operator);
            var expression = ParsePrimary();
            return new UnaryOperation("-", expression);
        }
        
        throw new Exception($"Неожиданный токен: {CurrentToken}");
    }
    
    private void Eat(TokenType expectedType)
    {
        if (CurrentToken.Type == expectedType)
        {
            _currentTokenIndex++;
        }
        else
        {
            throw new Exception($"Ожидался {expectedType}, но получен {CurrentToken.Type}");
        }
    }
}