using System.Text;

namespace Lexer;

public struct LexicalData
{
    public int Keywords { get; set; }
    public int Identifiers { get; set; }
    public int NumberLiterals { get; set; }
    public int StringLiterals { get; set; }
    public int Operators { get; set; }
    public int OtherLexemes { get; set; }

    public override readonly string ToString()
    {
        return $"""
            keywords: {Keywords}
            identifier: {Identifiers}
            number literals: {NumberLiterals}
            string literals: {StringLiterals}
            operators: {Operators}
            other lexemes: {OtherLexemes}
        """;
    }
}

public static class LexicalStats
{
    public static string CollectFromFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Файл не найден", path);

        string text = File.ReadAllText(path, Encoding.UTF8);
        Lexer lexer = new Lexer(text);
        IReadOnlyList<Token> tokens = lexer.Tokenize();

        LexicalData stats = new LexicalData();
        HashSet<string> seenIdentifiers = new HashSet<string>(StringComparer.Ordinal);

        foreach (Token token in tokens)
        {
            switch (token.Type)
            {
                case TokenType.Keyword:
                    if (string.Equals(token.Lexeme, "bello!", StringComparison.Ordinal))
                    {
                        break;
                    }
                    if (string.Equals(token.Lexeme, "loka", StringComparison.Ordinal))
                    {
                        stats.OtherLexemes++;
                    }
                    else
                    {
                        stats.Keywords++;
                    }
                    break;
                case TokenType.Identifier:
                    if (IsTypeName(token.Lexeme))
                    {
                        stats.OtherLexemes++;
                    }
                    else if (seenIdentifiers.Add(token.Lexeme))
                    {
                        stats.Identifiers++;
                    }
                    break;
                case TokenType.NumberLiteral:
                    stats.NumberLiterals++;
                    break;
                case TokenType.StringLiteral:
                    stats.StringLiterals++;
                    break;
                case TokenType.Operator:
                    stats.Operators++;
                    break;
                case TokenType.Whitespace:
                case TokenType.Comment:
                    break;
                default:
                    stats.OtherLexemes++;
                    break;
            }
        }

        return stats.ToString();
    }

    private static bool IsTypeName(string lexeme)
    {
        return string.Equals(lexeme, "Banana", StringComparison.Ordinal)
            || string.Equals(lexeme, "Papaya", StringComparison.Ordinal)
            || string.Equals(lexeme, "Gelato", StringComparison.Ordinal)
            || string.Equals(lexeme, "Spaghetti", StringComparison.Ordinal);
    }
}


