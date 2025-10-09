using System;

public class Tokenizer
{
    private readonly string _input;
    private int _position;

    public Tokenizer(string input)
    {
        _input = input.Replace(" ", "");
        _position = 0;
    }

    public List<Token> Tokenize()
    {
        var tokens = new List<Token>();

        while (_position < _input.Length)
        {
            char current = _input[_position];

            if (char.IsDigit(current) || current == ',')
            {
                tokens.Add(ReadNumber());
            }
            else if (char.IsLetter(current))
            {
                tokens.Add(ReadFunctionOrVariable());
            }
            else if (IsOperator(current))
            {
                tokens.Add(new Token(TokenType.Operator, current.ToString()));
                _position++;
            }
            else if (current == '(')
            {
                tokens.Add(new Token(TokenType.LeftParenthesis));
                _position++;
            }
            else if (current == ')')
            {
                tokens.Add(new Token(TokenType.RightParenthesis));
                _position++;
            }
            else if (current == ',')
            {
                tokens.Add(new Token(TokenType.Comma));
                _position++;
            }
            else
            {
                throw new Exception($"Неизвестный символ: {current}");
            }
        }

        tokens.Add(new Token(TokenType.EOF));
        return tokens;
    }

    private Token ReadNumber()
    {
        int start = _position;

        while (_position < _input.Length && (char.IsDigit(_input[_position]) || _input[_position] == ','))
        {
            _position++;
        }

        string number = _input.Substring(start, _position - start);
        return new Token(TokenType.Number, number.Replace(',', '.'));
    }

    private Token ReadFunctionOrVariable()
    {
        int start = _position;

        while (_position < _input.Length && char.IsLetter(_input[_position]))
        {
            _position++;
        }

        string value = _input.Substring(start, _position - start);

        string[] functions = { "sin", "cos", "tan", "log", "exp", "sqrt" };
        if (functions.Contains(value))
        {
            return new Token(TokenType.Function, value);
        }
        else
        {
            return new Token(TokenType.Variable, value);
        }
    }

    private bool IsOperator(char c)
    {
        return c == '+' || c == '-' || c == '*' || c == '/' || c == '^';
    }
}