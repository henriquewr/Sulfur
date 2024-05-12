using Sulfur.Enum;
using System.Collections.Generic;

namespace Sulfur.Main
{
    public static class Keywords
    {
        public static readonly Dictionary<string, TokenType> KeyWords = new()
        {
            { "let", TokenType.Let },
            { "const", TokenType.Const },
            { "func", TokenType.Func },
            { "if", TokenType.If },
            { "else", TokenType.Else },
            { "return", TokenType.Return },
        };

        public static bool IsReserved(string str)
        {
            var isReserved = KeyWords.ContainsKey(str);
            return isReserved;
        }
    }
}
