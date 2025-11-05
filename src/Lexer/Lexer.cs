using System;
using System.Collections.Generic;
using System.Text;

namespace Lexer
{
    public class Lexer
    {
        private readonly string _source;
        private int _position;
        private readonly List<Token> _tokens = new();

        public Lexer(string source)
        {
            _source = (source ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n');
            _position = 0;
        }

        public IReadOnlyList<Token> Tokenize()
        {
            while (!IsAtEnd())
            {
                if (char.IsWhiteSpace(Peek()))
                {
                    ReadWhitespace();
                    continue;
                }

                if (IsAtEnd()) break;

                char c = Peek();

                if (IsStartOfSingleLineComment())
                {
                    ReadSingleLineComment();
                }
                else if (IsStartOfMultiLineComment())
                {
                    ReadMultiLineComment();
                }
                else if (char.IsLetter(c) || c == '_')
                {
                    ReadWordLike();
                }
                else if (char.IsDigit(c) || (c == '-' && char.IsDigit(Peek(1))))
                {
                    ReadNumber();
                }
                else if (c == '!')
                {
                    ReadBangString();
                }
                else if (c == '"')
                {
                    ReadQuotedString();
                }
                else
                {
                    ReadDelimiterOrSymbol();
                }
            }

            return _tokens;
        }

        private void ReadWhitespace()
        {
            int start = _position;
            while (!IsAtEnd() && char.IsWhiteSpace(Peek())) Advance();
            _tokens.Add(new Token(TokenType.Whitespace, _source[start.._position], start));
        }

        private void ReadWordLike()
        {
            int start = _position;
            while (!IsAtEnd() && (char.IsLetterOrDigit(Peek()) || Peek() == '_' || Peek() == '-'))
                Advance();

            string word = _source[start.._position];

            if (Peek() == '!')
            {
                string withBang = word + "!";
                if (IsKeyword(withBang))
                {
                    Advance(); // consume '!'
                    _tokens.Add(new Token(TokenType.Keyword, withBang, start));
                    return;
                }
            }

            if (IsOperator(word))
            {
                _tokens.Add(new Token(TokenType.Operator, word, start));
                return;
            }

            if (IsKeyword(word))
            {
                _tokens.Add(new Token(TokenType.Keyword, word, start));
                return;
            }

            _tokens.Add(new Token(TokenType.Identifier, word, start));
        }

        private void ReadNumber()
        {
            int start = _position;
            if (Peek() == '-') Advance();

            while (!IsAtEnd() && char.IsDigit(Peek())) Advance();

            if (!IsAtEnd() && Peek() == '.' && char.IsDigit(Peek(1)))
            {
                Advance();
                while (!IsAtEnd() && char.IsDigit(Peek())) Advance();
            }

            _tokens.Add(new Token(TokenType.NumberLiteral, _source[start.._position], start));
        }

        private void ReadBangString()
        {
            int start = _position;
            Advance();

            if (Peek() == ')')
            {
                string single = _source.Substring(start, 1);
                _tokens.Add(new Token(TokenType.StringLiteral, single, start));
                return;
            }

            while (!IsAtEnd())
            {
                if (Peek() == '!' && Peek(1) == '!')
                {
                    Advance(2);
                    continue;
                }
                if (Peek() == '!')
                {
                    break;
                }
                Advance();
            }

            if (!IsAtEnd())
            {
                Advance();
            }

            string lexeme = _source[start.._position];
            _tokens.Add(new Token(TokenType.StringLiteral, lexeme, start));
        }

        private void ReadQuotedString()
        {
            int start = _position;
            StringBuilder sb = new StringBuilder();
            sb.Append('"');
            Advance();

            while (!IsAtEnd())
            {
                if (Peek() == '"')
                {
                    sb.Append('"');
                    Advance();
                    break;
                }

                sb.Append(Peek());
                Advance();
            }

            _tokens.Add(new Token(TokenType.StringLiteral, sb.ToString(), start));
        }

        private void ReadSingleLineComment()
        {
            int start = _position;
            Advance(2);
            while (!IsAtEnd() && Peek() != '\n') Advance();
            _tokens.Add(new Token(TokenType.Comment, "//" + _source[(start + 2).._position], start));
        }

        private void ReadMultiLineComment()
        {
            int start = _position;
            Advance(2);
            while (!IsAtEnd() && !(Peek() == '*' && Peek(1) == '/')) Advance();
            if (!IsAtEnd()) Advance(2);
            string text = _source[start.._position];
            _tokens.Add(new Token(TokenType.Comment, text, start));
        }

        private void ReadDelimiterOrSymbol()
        {
            int pos = _position;
            char c = Peek();
            if (c == '(' || c == ')')
            {
                Advance();
                _tokens.Add(new Token(TokenType.Delimiter, c.ToString(), pos));
                return;
            }

            int start = _position;
            while (!IsAtEnd() && !char.IsLetterOrDigit(Peek()) && !char.IsWhiteSpace(Peek()))
            {
                Advance();
            }
            string sym = _source[start.._position];
            _tokens.Add(new Token(TokenType.Delimiter, sym, start));
        }

        private bool IsStartOfSingleLineComment() => Peek() == '/' && Peek(1) == '/';
        private bool IsStartOfMultiLineComment() => Peek() == '/' && Peek(1) == '*';

        private char Peek(int offset = 0)
        {
            int idx = _position + offset;
            return idx < _source.Length ? _source[idx] : '\0';
        }

        private void Advance(int count = 1)
        {
            _position = Math.Min(_position + count, _source.Length);
        }

        private bool IsAtEnd() => _position >= _source.Length;

        private static bool IsKeyword(string word)
        {
            ReadOnlySpan<string> keywords = new[]
            {
                "bello!", "oca!", "stopa", "bapple", "poop", "trusela",
                "bi-do", "uh-oh", "again", "kemari", "aspetta",
                "tulalilloo", "ti", "amo", "guoleila", "tank", "yu", "boo-ya", "naidu!",
                "loka", "Da", "No", "da", "no",
            };
            foreach (string k in keywords)
            {
                if (string.Equals(k, word, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsOperator(string word)
        {
            ReadOnlySpan<string> ops = new[]
            {
                "lumai", "beedo", "dibotada", "poopaye", "pado",
                "melomo", "flavuk", "con", "looka", "too", "la",
                "makoroni", "tropa", "bo-ca",
            };
            foreach (string o in ops)
            {
                if (string.Equals(o, word, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }
    }
}


