using Sulfur.Enum;
using Sulfur.Extensions;
using Sulfur.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sulfur.Main
{
    public static class Lexer
    {
        public static List<IToken> Tokenize(string source)
        {
            var tokens = new List<IToken>();
            var src = source.ToCharArray().Select(x => x.ToString()).ToList();
            while (src.Count > 0)
            {
                if (src[0] == "(")
                {
                    tokens.Add(CreateToken(src.Pop(), TokenType.OpenParen));
                }
                else if (src[0] == ")")
                {
                    tokens.Add(CreateToken(src.Pop(), TokenType.CloseParen));
                }
                else if (src[0] == "{")
                {
                    tokens.Add(CreateToken(src.Pop(), TokenType.OpenBrace));
                }
                else if (src[0] == "}")
                {
                    tokens.Add(CreateToken(src.Pop(), TokenType.CloseBrace));
                }
                else if (src[0] == "[")
                {
                    tokens.Add(CreateToken(src.Pop(), TokenType.OpenBracket));
                }
                else if (src[0] == "]")
                {
                    tokens.Add(CreateToken(src.Pop(), TokenType.CloseBracket));
                }
                else if (src[0] == "+" || src[0] == "-" || src[0] == "*" || src[0] == "/" || src[0] == "%" || src[0] == "<" || src[0] == ">" || src[0] == "!")
                {
                    if (src.Count > 1 && src[1] == "=")
                    {
                        tokens.Add(CreateToken(src.Pop() + src.Pop(), TokenType.BinaryOperator));
                    }
                    else
                    {
                        tokens.Add(CreateToken(src.Pop(), TokenType.BinaryOperator));
                    }
                }
                else if (src[0] == "=")
                {
                    if (src.Count > 1 && src[1] == "=")
                    {
                        tokens.Add(CreateToken(src.Pop() + src.Pop(), TokenType.BinaryOperator));
                    }
                    else
                    {
                        tokens.Add(CreateToken(src.Pop(), TokenType.Equals));
                    }
                }
                else if (src.Count > 1 && src[0] == "&" && src[1] == "&" || src[0] == "|" && src[1] == "|")
                {
                    tokens.Add(CreateToken(src.Pop() + src.Pop(), TokenType.LogicalOperator));
                }
                else if (src[0] == "!")
                {
                    tokens.Add(CreateToken(src.Pop(), TokenType.UnaryOperator));
                }
                else if (src[0] == ";")
                {
                    tokens.Add(CreateToken(src.Pop(), TokenType.Semicolon));
                }
                else if (src[0] == ":")
                {
                    tokens.Add(CreateToken(src.Pop(), TokenType.Colon));
                }
                else if (src[0] == ",")
                {
                    tokens.Add(CreateToken(src.Pop(), TokenType.Comma));
                }
                else if (src[0] == ".")
                {
                    tokens.Add(CreateToken(src.Pop(), TokenType.Dot));
                }
                else if (src[0] == "\"")// "
                {
                    src.Pop();
                    var str = "";
                    while (src[0] != "\"")
                    {
                        str += src.Pop();
                    }
                    src.Pop();
                    tokens.Add(CreateToken(str, TokenType.String));
                }
                else
                {
                    if (src[0].IsDigits())
                    {
                        var num = "";
                        while (src.Count > 0 && src[0].IsDigits())
                        {
                            num += src.Pop();
                        }
                        tokens.Add(CreateToken(num, TokenType.Int));
                    }
                    else if (src[0].IsAlphabetic())
                    {
                        var ident = "";
                        while (src.Count > 0 && src[0].IsAlphabetic())
                        {
                            ident += src.Pop();
                        }

                        if (Keywords.KeyWords.TryGetValue(ident, out TokenType tokenType))
                        {
                            tokens.Add(CreateToken(ident, tokenType));
                        }
                        else
                        {
                            tokens.Add(CreateToken(ident, TokenType.Identifier));
                        }
                    }
                    else if (IsSkippable(src[0]))
                    {
                        src.Pop();
                    }
                    else
                    {
                        var message = $"Unrecognized character found in source: {src[0]}";
                        Console.WriteLine(message);
                        throw new Exception(message);
                    }
                }
            }
            tokens.Add(CreateToken("EndOfFile", TokenType.EOF));
            return tokens;
        }

        private static bool IsSkippable(string str)
        {
            var isSkippable = str == " " || str == "\n" || str == "\t" || str == "\r";
            return isSkippable;
        }

        private static IToken CreateToken(string val, TokenType tokenType)
        {
            return new Token.Token(val, tokenType);
        }
    }
}
