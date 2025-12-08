using System;

namespace Lexer
{
    public class LexerException : Exception
    {
        public LexerException(string message, int position)
            : base($"{message} (pos={position})")
        {
            Position = position;
        }

        public int Position { get; }
    }
}