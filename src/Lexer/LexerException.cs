using System;

namespace Lexer
{
    public class LexerException : Exception
    {
        public int Position { get; }

        public LexerException(string message, int position)
            : base($"{message} (pos={position})")
        {
            Position = position;
        }
    }
}


