using Sulfur.Interfaces;
using static Sulfur.Main.Ast;
using Sulfur.Extensions;
using Sulfur.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using static Sulfur.Runtime.Values;

namespace Sulfur.Main
{
    public class Parser
    {
        private static List<IToken> _tokens { get; set; }
        public static IProgram CreateAST(string source)
        {
            _tokens = Lexer.Tokenize(source);
            var program = new Ast.Program();

            while (!IsEOF(_tokens[0]))
            {
                program.Body.Add(ParseStmt(false));
            }

            return program;
        }

        private static IToken Expect(TokenType tokenType, string error, bool showNearString = true)
        {
            var prev = _tokens.Pop();
            if (prev is null || prev.Type != tokenType)
            {
                var message = $"Parser error:\n {error} {prev} - Expecting: {tokenType}";
                if (showNearString == true)
                {
                    message += $"\n near: {GetNearString()}";
                }
                Console.WriteLine(message);
                throw new Exception(message);
            }

            return prev;
        }

        private static bool IsEOF(IToken token)
        {
            var isEof = token.Type == TokenType.EOF;
            return isEof;
        }

        private static IStmt ParseStmt(bool isInsideFunc)
        {
            switch (_tokens[0].Type)
            {
                case TokenType.Let:
                case TokenType.Const:
                    return ParseVarDeclarationExpr();
                case TokenType.Func:
                    return ParseFuncDeclaration();
                case TokenType.If:
                    return ParseIfDeclaration(isInsideFunc);
                case TokenType.Return:
                    return ParseReturnDeclarationExpr(isInsideFunc);

                default:
                    return ParseExpr();
            }
            //return _tokens[0].Type switch
            //{
            //    TokenType.Let or TokenType.Const => ParseVarDeclarationExpr(),
            //    TokenType.Func => ParseFuncDeclaration(),
            //    TokenType.Return => ParseReturnDeclarationExpr(isInsideFunc),
            //    TokenType.If => ParseIfDeclaration(isInsideFunc),
            //    _ => ParseExpr(),
            //};
        }

        private static IStmt ParseIfDeclaration(bool isInsideFunc)
        {
            _tokens.Pop();
            Expect(TokenType.OpenParen, "Expected open paren '(' following if keyword");

            var condition = ParseStmt(isInsideFunc);

            Expect(TokenType.CloseParen, "Expected close paren ')' on if keyword");

            Expect(TokenType.OpenBrace, "Expected 'if' body following declaration.");
            var ifBody = new List<IStmt>();
            while (!IsEOF(_tokens[0]) && _tokens[0].Type != TokenType.CloseBrace)
            {
                ifBody.Add(ParseStmt(isInsideFunc));
            }

            Expect(TokenType.CloseBrace, "Expected closing brace '}' inside 'if' declaration.");

            var ifStmt = new IfDeclaration
            {
                Condition = condition,
                Body = ifBody,
                ElseBody = new List<IStmt>()
            };

            if (_tokens[0].Type == TokenType.Else)
            {
                _tokens.Pop();
                Expect(TokenType.OpenBrace, "Expected 'else' body following declaration.");
                while (!IsEOF(_tokens[0]) && _tokens[0].Type != TokenType.CloseBrace)
                {
                    ifStmt.ElseBody.Add(ParseStmt(isInsideFunc));
                }

                Expect(TokenType.CloseBrace, "Expected closing brace '}' inside 'else' declaration.");
            }

            return ifStmt;
        }

        private static IStmt ParseFuncDeclaration()
        {
            _tokens.Pop();
            var name = Expect(TokenType.Identifier, "Expected function name following func keyword").Value;

            var args = ParseArgs();

            var param = new List<string>();

            for (int i = 0; i < args.Count; i++)
            {
                var item = args[i];
                if (item.Kind != NodeType.Identifier)
                {
                    throw new Exception("Inside function declaration expected parameters to be of type string.");
                }

                param.Add(((IIdentifier)item).Symbol);
            }

            Expect(TokenType.OpenBrace, "Expected function body following declaration.");

            var body = new List<IStmt>();
            while (!IsEOF(_tokens[0]) && _tokens[0].Type != TokenType.CloseBrace)
            {
                body.Add(ParseStmt(true));
            }

            Expect(TokenType.CloseBrace, "Expected closing brace '}' inside function declaration.");

            var func = new FuncDeclaration
            {
                Name = name,
                Params = param,
                Body = body,
            };

            return func;
        }

        private static IStmt ParseReturnDeclarationExpr(bool isInsideFunc)
        {
            if (isInsideFunc == false)
            {
                throw new InvalidOperationException("return outside of function");
            }

            _tokens.Pop();

            var retStmt = new ReturnDeclaration();

            if (_tokens[0].Type == TokenType.Semicolon)
            {
                _tokens.Pop();
                retStmt.Arg = new Identifier { Symbol = "null" };
                return retStmt;
            }

            var arg = ParseStmt(isInsideFunc);

            Expect(TokenType.Semicolon, "Expected semicolon  ';'  following return declaration.");

            retStmt.Arg = arg;

            return retStmt;
        }

        private static IStmt ParseVarDeclarationExpr()
        {
            var type = _tokens.Pop().Type;

            var isConstant = type == TokenType.Const;
            var identifier = Expect(TokenType.Identifier, "Expected identifier name following let | const keywords").Value;

            var variableDeclaration = new VarDeclaration
            {
                Identifier = identifier,
                IsConstant = isConstant,
            };

            if (_tokens[0].Type == TokenType.Semicolon)
            {
                if (isConstant)
                {
                    throw new Exception("Must assigne a value to constant expression.");
                }

                variableDeclaration.IsConstant = false;

                return variableDeclaration;
            }

            Expect(TokenType.Equals, "Expected equals token following identifier in var declaration");

            variableDeclaration.Value = ParseExpr();

            Expect(TokenType.Semicolon, "Variable declaration statement must end with semicolon.");

            return variableDeclaration;
        }

        private static IExpr ParseExpr()
        {
            var parsed = ParseLogicalOrExpr();
            return parsed;
        }

        private static IExpr ParseLogicalOrExpr()
        {
            var left = ParseLogicalAndExpr();
            while (_tokens[0].Value == "||")
            {
                var operation = _tokens.Pop().Value;
                var right = ParseLogicalAndExpr();
                left = new LogicalExpr
                {
                    Left = left,
                    Right = right,
                    Operator = operation,
                };
            }

            return left;
        }

        private static IExpr ParseLogicalAndExpr()
        {
            var left = ParseAssignmentExpr();
            while (_tokens[0].Value == "&&")
            {
                var operation = _tokens.Pop().Value;
                var right = ParseAssignmentExpr();
                left = new LogicalExpr
                {
                    Left = left,
                    Right = right,
                    Operator = operation,
                };
            }

            return left;
        }

        private static IExpr ParseAssignmentExpr()
        {
            var left = ParseObjectExpr();
            if (_tokens[0].Type == TokenType.Equals)
            {
                _tokens.Pop();
                var value = ParseAssignmentExpr();
                var assignmentExpr = new AssignmentExpr
                {
                    Assigne = left,
                    Value = value,
                };

                Expect(TokenType.Semicolon, "Variable assignment statement must end with semicolon.");

                return assignmentExpr;
            }

            return left;
        }

        private static IExpr ParseObjectExpr()
        {
            if (_tokens[0].Type != TokenType.OpenBrace)
            {
                return ParseComparisonExpr();
            }

            _tokens.Pop();

            var properties = new List<IProperty>();

            while (!IsEOF(_tokens[0]) && _tokens[0].Type != TokenType.CloseBrace)
            {
                var key = Expect(TokenType.Identifier, "Object literal key expected.").Value;

                if (_tokens[0].Type is TokenType.Comma)
                {
                    _tokens.Pop();
                    var prop = new Property
                    {
                        Key = key,
                    };
                    properties.Add(prop);
                    continue;
                }
                else if (_tokens[0].Type is TokenType.CloseBrace)
                {
                    var prop = new Property
                    {
                        Key = key,
                    };
                    properties.Add(prop);
                    continue;
                }

                Expect(TokenType.Colon, "Missing colon following identifier in ObjectExpr");

                var value = ParseExpr();

                properties.Add(new Property
                {
                    Key = key,
                    Value = value
                });

                if (_tokens[0].Type is not TokenType.CloseBrace)
                {
                    Expect(TokenType.Comma, "Expected comma or closing bracket following property");
                }
            }

            Expect(TokenType.CloseBrace, "Object literal missing closing brace.");

            var objectLiteral = new ObjectLiteral
            {
                Properties = properties,
            };

            return objectLiteral;
        }

        private static IExpr ParseComparisonExpr()
        {
            var left = ParseAdditiveExpr();
            while (_tokens[0].Value == "==" || _tokens[0].Value == "!=" || _tokens[0].Value == ">" || _tokens[0].Value == "<" || _tokens[0].Value == "<=" || _tokens[0].Value == ">=")
            {
                var operation = _tokens.Pop().Value;
                var right = ParseAdditiveExpr();
                left = new BinaryExpr
                {
                    Left = left,
                    Right = right,
                    Operator = operation,
                };
            }

            return left;
        }

        private static IExpr ParseAdditiveExpr()
        {
            var left = ParseMultiplicativeExpr();
            while (_tokens[0].Value == "+" || _tokens[0].Value == "-")
            {
                var operation = _tokens.Pop().Value;
                var right = ParseMultiplicativeExpr();
                left = new BinaryExpr
                {
                    Left = left,
                    Right = right,
                    Operator = operation,
                };
            }

            return left;
        }

        private static IExpr ParseMultiplicativeExpr()
        {
            var left = ParseUnaryExpr();

            while (_tokens[0].Value == "*" || _tokens[0].Value == "/" || _tokens[0].Value == "%")
            {
                var operation = _tokens.Pop().Value;
                var right = ParseUnaryExpr();
                left = new BinaryExpr
                {
                    Left = left,
                    Right = right,
                    Operator = operation,
                };
            }

            return left;
        }

        private static IExpr ParseUnaryExpr()
        {
            while (_tokens[0].Value == "!")
            {
                var operation = _tokens.Pop().Value;
                var arg = ParseCallMemberExpr();
                var unaryExpr = new UnaryExpr
                {
                    Arg = arg,
                    Operator = operation,
                };
                return unaryExpr;
            }

            return ParseCallMemberExpr();
        }

        private static IExpr ParseCallMemberExpr()
        {
            var member = ParseMemberExpr();
            if (_tokens[0].Type == TokenType.OpenParen)
            {
                return ParseCallExpr(member);
            }

            return member;
        }

        private static IExpr ParseCallExpr(IExpr caller)
        {
            var callExpr = new CallExpr
            {
                Caller = caller,
                Args = ParseArgs()
            };

            if (_tokens[0].Type == TokenType.OpenParen)
            {
                return ParseCallExpr(callExpr);
            }

            return callExpr;
        }

        private static List<IExpr> ParseArgs()
        {
            Expect(TokenType.OpenParen, "Expected open parenthesis.");

            var args = _tokens[0].Type == TokenType.CloseParen ? new List<IExpr>() : ParseArgsList();

            Expect(TokenType.CloseParen, "Missing closing parenthesis inside arguments list.");

            return args;
        }

        private static List<IExpr> ParseArgsList()
        {
            var args = new List<IExpr>
            {
                ParseAssignmentExpr(),
            };

            while (_tokens[0].Type == TokenType.Comma && _tokens.Pop() is not null)
            {
                args.Add(ParseAssignmentExpr());
            }

            return args;
        }

        private static IExpr ParseMemberExpr()
        {
            var obj = ParsePrimaryExpr();

            while (_tokens[0].Type == TokenType.Dot || _tokens[0].Type == TokenType.OpenBracket)
            {
                var oper = _tokens.Pop();

                IExpr prop;
                bool computed;

                if (oper.Type == TokenType.Dot)
                {
                    computed = false;
                    prop = ParsePrimaryExpr();
                    if (prop.Kind != NodeType.Identifier)
                    {
                        throw new InvalidOperationException($"Cannot use dot operator without right hand side being a identifier   near:   {GetNearString()}");
                    }
                }
                else
                {
                    computed = true;
                    prop = ParseExpr();
                    Expect(TokenType.CloseBracket, "Missing closing bracket in computed value.");
                }

                obj = new MemberExpr
                {
                    Object = obj,
                    Computed = computed,
                    Property = prop,
                };
            }

            return obj;
        }

        private static IExpr ParsePrimaryExpr()
        {
            switch (_tokens[0].Type)
            {
                case TokenType.Identifier:
                    var identifier = new Identifier { Symbol = _tokens.Pop().Value };
                    return identifier;

                case TokenType.Int:
                    var numericLiteral = new IntLiteral { Value = Convert.ToInt32(_tokens.Pop().Value) };
                    return numericLiteral;

                case TokenType.Long:
                    var longLiteral = new LongLiteral { Value = Convert.ToInt64(_tokens.Pop().Value) };
                    return longLiteral;

                case TokenType.String:
                    var stringLiteral = new StringLiteral { Value = _tokens.Pop().Value };
                    return stringLiteral;

                case TokenType.OpenParen:
                    _tokens.Pop();
                    var value = ParseExpr();
                    Expect(TokenType.CloseParen, "Unexpected token found inside parenthesised expression. Expected closing parenthesis.");
                    return value;

                default:
                    throw new Exception($"Not recognized:   {_tokens[0].Type}   near:   {GetNearString()}");
            }
        }

        private static string GetNearString(int count = 15)
        {
            var nearString = _tokens.Take(count).Where(x => !IsEOF(x)).Select(x => x.Value).Aggregate((acc, x) => acc + x);

            return nearString;
        }
    }
}
