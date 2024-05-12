using Sulfur.Enum;
using Sulfur.Interfaces;

namespace Sulfur.Token
{
    public class Token : IToken
    {
        public Token(string value, TokenType tokenType) 
        {
            Value = value;
            Type = tokenType;
        }
        public string Value { get; set; }
        public TokenType Type { get; set ; }
    }
}
