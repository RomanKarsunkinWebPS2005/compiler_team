namespace Lexer;

public readonly record struct Token(TokenType Type, string Lexeme, int Position)
{
    public override string ToString() => $"{Type}: '{Lexeme}' @ {Position}";
}


