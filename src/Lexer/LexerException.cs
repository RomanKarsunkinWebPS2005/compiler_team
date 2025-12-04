using System;

namespace Lexer
{
    #pragma warning disable RCS1194
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