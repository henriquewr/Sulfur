using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sulfur.Enum
{
    public enum TokenType
    {
        Int,
        String,
        Long,

        Identifier,

        Let,

        Const,
        Func,

        If,
        Else,
        Return,

        BinaryOperator,
        UnaryOperator,
        LogicalOperator,

        Equals,
        Comma, Dot, Colon,
        Semicolon,
        OpenParen,
        CloseParen,
        OpenBrace,
        CloseBrace,
        OpenBracket,
        CloseBracket,
        EOF,
    }
}
